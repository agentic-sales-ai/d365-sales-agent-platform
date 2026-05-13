using Application.Common.Interfaces;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlannerController : ControllerBase
{
    private readonly IWorkflowStateService
        _workflowStateService;

    private readonly IWorkflowStateRepository
        _repository;

    private readonly IWorkflowExecutionQueue
        _queue;

    private readonly IWorkflowTelemetryService
        _telemetryService;

    private readonly IWorkflowApprovalRepository
        _approvalRepository;

    private readonly IPlannerRuntime
        _plannerRuntime;

    public PlannerController(
        IWorkflowStateService workflowStateService,
        IWorkflowStateRepository repository,
        IWorkflowExecutionQueue queue,
        IWorkflowTelemetryService telemetryService,
        IWorkflowApprovalRepository approvalRepository,
        IPlannerRuntime plannerRuntime)
    {
        _workflowStateService =
            workflowStateService;

        _repository =
            repository;

        _queue =
            queue;

        _telemetryService =
            telemetryService;

        _approvalRepository =
            approvalRepository;

        _plannerRuntime =
            plannerRuntime;
    }

    [HttpPost("execute")]
    public async Task<IActionResult>
        Execute(
            AiToolExecutionRequestModel request)
    {
        var workflow =
            _workflowStateService
                .CreateState(
                    request.UserPrompt);

        await _repository
            .SaveAsync(workflow);

        _queue.Enqueue(
            workflow.WorkflowId);

        return Ok(new
        {
            workflow.WorkflowId,

            workflow.Status
        });
    }

    [HttpGet("workflow/{workflowId}")]
    public async Task<IActionResult>
        GetWorkflow(Guid workflowId)
    {
        var workflow =
            await _repository
                .GetAsync(workflowId);

        if (workflow == null)
        {
            return NotFound();
        }

        return Ok(workflow);
    }

    [HttpGet("workflow/{workflowId}/events")]
    public async Task<IActionResult>
        GetWorkflowEvents(
            Guid workflowId)
    {
        var events =
            await _telemetryService
                .GetEventsAsync(workflowId);

        return Ok(events);
    }

    [HttpPost(
        "workflow/{workflowId}/approve")]
    public async Task<IActionResult>
        ApproveWorkflow(
            Guid workflowId,
            ResolveApprovalRequestModel request)
    {
        var workflow =
            await _repository
                .GetAsync(workflowId);

        if (workflow == null)
        {
            return NotFound();
        }

        var pendingApproval =
            await _approvalRepository
                .GetPendingAsync(
                    workflowId);

        if (pendingApproval == null)
        {
            return BadRequest(
                "No pending approval found.");
        }

        pendingApproval.Status =
            request.Approved
                ? "Approved"
                : "Rejected";

        pendingApproval.ResolvedAtUtc =
            DateTime.UtcNow;

        pendingApproval.DecisionNotes =
            request.Notes;

        await _approvalRepository
            .UpdateAsync(
                pendingApproval);

        if (!request.Approved)
        {
            workflow.Status =
                "Rejected";

            await _repository
                .SaveAsync(workflow);

            return Ok(workflow);
        }

        workflow.Status =
            "Running";

        await _repository
            .SaveAsync(workflow);

        await _telemetryService
            .TrackEventAsync(
            new WorkflowExecutionEventModel
            {
                WorkflowId =
                    workflow.WorkflowId,

                TimestampUtc =
                    DateTime.UtcNow,

                EventType =
                    "WorkflowResumed",

                Message =
                    "Workflow resumed after approval."
            });

        _queue.Enqueue(
            workflow.WorkflowId);

        return Ok(new
        {
            workflow.WorkflowId,

            workflow.Status
        });
    }
}
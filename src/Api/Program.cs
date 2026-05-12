using Application.DependencyInjection;
using Infrastructure.DependencyInjection;
using Application.Common.Options;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAI options
builder.Services.Configure<OpenAIOptions>(builder.Configuration.GetSection(OpenAIOptions.SectionName));

// Add Dataverse options
builder.Services.Configure<DataverseOptions>(builder.Configuration.GetSection(DataverseOptions.SectionName));

builder.Services.AddApplication();
builder.Services.AddInfrastructure();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
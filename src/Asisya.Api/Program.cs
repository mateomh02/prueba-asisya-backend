using Asisya.Api.Extensions;
using Asisya.Application;
using Asisya.Infrastructure;
using Asisya.Infrastructure.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFrontendCors(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddGlobalExceptionHandling();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(CorsExtensions.PolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

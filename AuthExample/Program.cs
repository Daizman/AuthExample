using AuthExample;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddInfrastructure(builder.Configuration)
    .AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

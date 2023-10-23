using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mongo REST Api", Version = "v1" });
});

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseSwagger(options =>
{
    options.RouteTemplate = "{documentName}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.Run();
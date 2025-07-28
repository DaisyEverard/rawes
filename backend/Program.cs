using DotNetEnv;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using MainApi.Clients;
using MainApi.Data;
using MainApi.Data.Survey;
using MainApi.Repositories;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var AllowAll = "_AllowAllOrigins";

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        var origins = builder.Configuration["CorsEndpoints"].Split(",");

        policy.WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials();
    });
    options.AddPolicy(name: AllowAll,
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
builder.Services.AddAuthorization();



DotNetEnv.Env.Load();

builder.Services.AddSingleton<BigQueryClientFactory>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseCors(AllowAll);
}
else
{
    app.UseCors(MyAllowSpecificOrigins);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getTable", () =>
{

    BigQueryClient client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT"));
    BigQueryTable table = client.GetTable(Environment.GetEnvironmentVariable("GOOGLE_DATASET"), Environment.GetEnvironmentVariable("GOOGLE_SURVEY_VIEW"));
    //TableSchema schema = table.Schema;
    //return Results.Ok(schema);

    string sql = $"SELECT * FROM {table} WHERE survey_id = 1";
    BigQueryParameter[] parameters = null;
    BigQueryResults results = client.ExecuteQuery(sql, parameters);
    return Results.Ok(results);

    //var surveyDto = SurveyDTO.ConvertResultToSurveyDTO(results);
    //return Results.Ok(surveyDto);
})
    .WithName("getTable")
    .WithOpenApi();

app.MapGet("/getSchema", () =>
{
    BigQueryClient client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT"));
    BigQueryTable table = client.GetTable(Environment.GetEnvironmentVariable("GOOGLE_DATASET"), Environment.GetEnvironmentVariable("GOOGLE_SURVEY_VIEW"));
    TableSchema schema = table.Schema;
    return Results.Ok(schema);
})
    .WithName("getSchema")
    .WithOpenApi();

app.MapControllers();
//xEndpoints.MapXendpoints(app)

app.Run();



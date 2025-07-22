using backend.src.Repositories;
using DotNetEnv;
using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using MainApi.Clients;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

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
builder.Services.AddScoped<IBigQueryRepository, BigQueryRepository>();

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

app.MapGet("/helloWorld", () =>
{

    BigQueryClient client = BigQueryClient.Create("peerless-garage-466612-a2");
    BigQueryTable table = client.GetTable("rawes", "surveys");
    string sql = $"SELECT * FROM {table}";
    //    BigQueryParameter[] parameters = new[]
    BigQueryParameter[] parameters = null;
//    {
//    new BigQueryParameter("level", BigQueryDbType.Int64, 2),
//    new BigQueryParameter("score", BigQueryDbType.Int64, 1500)
//};
    BigQueryResults results = client.ExecuteQuery(sql, parameters);
    foreach (BigQueryRow row in results)
    {
        Console.WriteLine($"{row}");
    }


    return Results.Ok(results);
})
    .WithName("HelloWorld")
    .WithOpenApi();

app.MapControllers();
//xEndpoints.MapXendpoints(app)

app.Run();



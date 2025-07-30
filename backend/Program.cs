using MainApi.Clients;
using DotNetEnv;
using MainApi.Controllers;
using Google.Cloud.BigQuery.V2;
using MainApi.Data;
using MainApi.Data.Survey;
using MainApi.Repositories;



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

SurveysController controller = new SurveysController(app);
controller.MapEndpoints(app);


app.Run();



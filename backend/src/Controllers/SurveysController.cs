using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using MainApi.Data.Queries;
using MainApi.Data.Survey;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace MainApi.Controllers;

public class SurveysController
{
    private WebApplication app;
    public SurveysController(WebApplication myApp)
    {
        app = myApp;
    }

    public void MapEndpoints(WebApplication app) {
        app.MapGet("/getTable", ([FromQuery] int surveyID) =>
        {
            BigQueryClient client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT"));
            BigQueryTable allSurveysTable = client.GetTable(Environment.GetEnvironmentVariable("GOOGLE_DATASET"), Environment.GetEnvironmentVariable("GOOGLE_SURVEY_VIEW"));
            BigQueryTable allRowsTable = client.GetTable(Environment.GetEnvironmentVariable("GOOGLE_DATASET"), Environment.GetEnvironmentVariable("GOOGLE_ROWS_VIEW"));

            BigQueryParameter[] parameters = new[]
            {
                new BigQueryParameter("survey_id", BigQueryDbType.Int64, surveyID),
            };

            BigQueryResults metadataResult = client.ExecuteQuery(BigQueryQueries.GetMetadataSql(allSurveysTable), parameters);
            BigQueryResults assessorsResult = client.ExecuteQuery(BigQueryQueries.GetAssessorsSql(allSurveysTable), parameters);
            BigQueryResults rowsResult = client.ExecuteQuery(BigQueryQueries.GetRowsSql(allRowsTable), parameters);
            BigQueryResults scalesResult = client.ExecuteQuery(BigQueryQueries.GetScalesSql(allRowsTable), parameters);

            var survey = SurveyDTO.ConvertResultsToSurvey(surveyID, metadataResult, assessorsResult, rowsResult, scalesResult);

            return Results.Ok(survey);
        })
        .WithName("getTable")
        .WithOpenApi();
    }
}

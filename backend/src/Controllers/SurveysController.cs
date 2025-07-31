using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using MainApi.Data.Queries;
using MainApi.Data.Survey;
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
        app.MapGet("/getTable", () =>
        {
            BigQueryClient client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT"));
            BigQueryTable allSurveysTable = client.GetTable(Environment.GetEnvironmentVariable("GOOGLE_DATASET"), Environment.GetEnvironmentVariable("GOOGLE_SURVEY_VIEW"));
            BigQueryTable allRowsTable = client.GetTable(Environment.GetEnvironmentVariable("GOOGLE_DATASET"), Environment.GetEnvironmentVariable("GOOGLE_ROWS_VIEW"));
            int surveyID = 1;

            BigQueryParameter[] nullParameters = null;
            BigQueryResults metadataResult = client.ExecuteQuery(BigQueryQueries.GetMetadataSql(allSurveysTable, surveyID), nullParameters);
            BigQueryResults assessorsResult = client.ExecuteQuery(BigQueryQueries.GetAssessorsSql(allSurveysTable, surveyID), nullParameters);
            BigQueryResults rowsResult = client.ExecuteQuery(BigQueryQueries.GetRowsSql(allRowsTable, surveyID), nullParameters);
            BigQueryResults scalesResult = client.ExecuteQuery(BigQueryQueries.GetScalesSql(allRowsTable, surveyID), nullParameters);

            var survey = SurveyDTO.ConvertResultsToSurvey(surveyID, metadataResult, assessorsResult, rowsResult, scalesResult);

            return Results.Ok(survey);
        })

        app.MapPost("/newSurvey", () =>
        {
//        SET new_survey_id = (
//SELECT IFNULL(MAX(survey_id), 0) + 1 FROM `rawes.surveys`
//  );
    })
    .WithName("getTable")
    .WithOpenApi();
    }
}

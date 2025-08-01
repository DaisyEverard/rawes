using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using MainApi.Data.Queries;
using MainApi.Data.Survey;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MainApi.Controllers;

public class SurveysController
{
    private WebApplication app;
    public SurveysController(WebApplication myApp)
    {
        app = myApp;
    }

    public void MapEndpoints(WebApplication app) {
        app.MapGet("survey/{surveyID}", ([FromQuery] int surveyID) =>
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

        app.MapPost("survey/new", (SurveyDTO survey) =>{
            BigQueryClient client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT"));
            BigQueryTable allSurveysTable = client.GetTable(Environment.GetEnvironmentVariable("GOOGLE_DATASET"), Environment.GetEnvironmentVariable("GOOGLE_SURVEY_VIEW"));
            BigQueryParameter[] nullParameters = null;
            BigQueryResults newIDTable = client.ExecuteQuery($"SELECT MAX(survey_id) + 1 AS id FROM {allSurveysTable} LIMIT 1", nullParameters);

            var newSurveyRow = newIDTable.FirstOrDefault();

            string newSurveyID = newSurveyRow["id"].ToString();

            string newSurveySql = @"
            CALL rawes.new_survey(
                @new_survey_id,
                @new_wetland_name,
                @new_gps_x,
                @new_gps_y,
                @new_assessors,
                @new_date_completed
            );";

            var newSurveyParameters = new List<BigQueryParameter>
            {
                new BigQueryParameter("new_survey_id", BigQueryDbType.Int64, Int64.Parse(newSurveyID)),
                new BigQueryParameter("new_wetland_name", BigQueryDbType.String, survey.WetlandName),
                new BigQueryParameter("new_gps_x", BigQueryDbType.Float64, survey.GPSCoordinates.x),
                new BigQueryParameter("new_gps_y", BigQueryDbType.Float64, survey.GPSCoordinates.y),
                new BigQueryParameter("new_assessors", BigQueryDbType.Array, survey.Assessors),
                new BigQueryParameter("new_date_completed", BigQueryDbType.Date, survey.DateCompleted)

            };

            foreach(SurveyRowDTO row in survey.Rows)
            {
                string newRowSql = @"
                CALL rawes.new_survey_row(
                    @new_benefit,
                    @new_importance,
                    @new_survey_id,
                    @scales,
                    @description
                );";

                var newRowParameters = new List<BigQueryParameter>
                {
                    new BigQueryParameter("new_benefit", BigQueryDbType.String, row.Benefit),
                    new BigQueryParameter("new_importance", BigQueryDbType.Float64, row.Importance),
                    new BigQueryParameter("new_survey_id", BigQueryDbType.Int64, Int64.Parse(newSurveyID)), 
                    new BigQueryParameter("scales", BigQueryDbType.Array, row.Scales),
                    new BigQueryParameter("description", BigQueryDbType.String, row.Description)
                };
                var newRowResult = client.ExecuteQuery(newRowSql, newRowParameters);
            }

            var newSurveyResult = client.ExecuteQuery(newSurveySql, newSurveyParameters);
            return Results.Ok(newSurveyResult);
        })
        .WithName("newSurvey")
        .WithOpenApi();

        app.MapDelete("survey/delete/{surveyID}", (string surveyID) => {
            BigQueryClient client = BigQueryClient.Create(Environment.GetEnvironmentVariable("GOOGLE_PROJECT"));

            string deleteSql = @"
                CALL rawes.delete_survey(
                    @survey_id
                );";

            var deleteParameters = new List<BigQueryParameter>
                {
                    new BigQueryParameter("survey_id", BigQueryDbType.String, surveyID),
                };

            var result = client.ExecuteQuery(deleteSql, deleteParameters);
            return Results.Ok(result);
        })
        .WithName("deleteSurvey")
        .WithOpenApi();
    }
}

using Google.Apis.Bigquery.v2.Data;
using Google.Cloud.BigQuery.V2;
using MainApi.Data.Queries;
using MainApi.Data.Survey;

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

            var surveyRows = new List<string>();

            // METADATA
            BigQueryRow metadataRow = metadataResult.First();
            string wetlandName = (string)metadataRow["wetland_name"];
            double gpsX = (double)metadataRow["gps_x"];
            double gpsY = (double)metadataRow["gps_y"];
            var gpsCoordinates = new GPSCoordinates { x = gpsX, y = gpsY };
            DateTime rawDate = (DateTime)metadataRow["date_completed"];
            DateOnly dateCompleted = DateOnly.FromDateTime(rawDate);


            // ASSESSORS
            List<string> assessorList = assessorsResult
            .Select(r => (string)r["a"])
            .ToList();

            // ROWS
            var benefitRows = rowsResult.ToList();
            var scaleRows = scalesResult.ToList();

            var surveyRowDtos = new List<SurveyRowDTO>();

            foreach (var benefitRow in benefitRows)
            {
                string benefit = (string)benefitRow["benefit"];
                string benefitType = (string)benefitRow["benefit_type"];
                double importance = (double)benefitRow["importance"];
                string description = (string)benefitRow["description"];

                List<string> scales = new List<string>();
                foreach (var scaleRow in scaleRows)
                {
                    if ((string)scaleRow["benefit"] == benefit)
                    {
                        var sStruct = (Dictionary<string, object>)scaleRow["s"];
                        string scaleValue = (string)sStruct["scale"];
                        scales.Add(scaleValue);
                    }
                }

                var rowDto = new SurveyRowDTO
                {
                    Benefit = benefit,
                    BenefitType = benefitType,
                    Importance = importance,
                    Scales = scales,
                    Description = description
                };
                surveyRowDtos.Add(rowDto);
            }

            var survey = new SurveyDTO
            {
                SurveyId = surveyID,
                WetlandName = wetlandName,
                GPSCoordinates = gpsCoordinates,
                Assessors = assessorList,
                DateCompleted = dateCompleted,
                Rows = surveyRowDtos
            };

            return Results.Ok(survey);
        })
    .WithName("getTable")
    .WithOpenApi();
    }
}

using Google.Cloud.BigQuery.V2;
using MainApi.Data.Survey;
using MainApi.Data.Queries;

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
            BigQueryResults metadata = client.ExecuteQuery(SurveyQueries.GetMetadataSql(allSurveysTable, surveyID), nullParameters);
            BigQueryResults assessors = client.ExecuteQuery(SurveyQueries.GetAssessorsSql(allSurveysTable, surveyID), nullParameters);
            BigQueryResults rows = client.ExecuteQuery(SurveyQueries.GetRowsSql(allRowsTable, surveyID), nullParameters);
            BigQueryResults scales = client.ExecuteQuery(SurveyQueries.GetScalesSql(allRowsTable, surveyID), nullParameters);

            var surveyRows = new List<string>();

            // METADATA
            BigQueryRow metadataRow = metadata.First();
            string wetlandName = (string)metadataRow["wetland_name"];
            double gpsX = (double)metadataRow["gps_x"];
            double gpsY = (double)metadataRow["gps_y"];
            GPSCoordinates gpsCoordinates = new GPSCoordinates(gpsX, gpsY);
            DateTime rawDate = (DateTime)metadataRow["date_completed"];
            DateOnly dateCompleted = DateOnly.FromDateTime(rawDate);


            // ASSESSORS
            List<string> assessorList = assessors
            .Select(r => (string)r["a"])
            .ToList();

            // ROWS
            var benefitRows = rows.ToList();
            var scaleRows = scales.ToList();

            var surveyRowDtos = new List<SurveyRowDTO>();

            foreach (var benefitRow in benefitRows)
            {
                string benefit = (string)benefitRow["benefit"];
                string benefitType = (string)benefitRow["benefit_type"];
                double importance = (double)benefitRow["importance"];
                string description = (string)benefitRow["description"];

                

                List<string> matchedScales = new List<string>();
                foreach (var scaleRow in scaleRows)
                {
                    if ((string)scaleRow["benefit"] == benefit)
                    {
                        var sStruct = (Dictionary<string, object>)scaleRow["s"];
                        string scaleValue = (string)sStruct["scale"];
                        matchedScales.Add(scaleValue);
                    }
                }

                var dto = new SurveyRowDTO(benefit, benefitType, importance, matchedScales, description);
                surveyRowDtos.Add(dto);
            }

            var survey = new SurveyDTO(surveyID, wetlandName, gpsCoordinates, assessorList, dateCompleted, surveyRowDtos);
            return Results.Ok(survey);
        })
    .WithName("getTable")
    .WithOpenApi();
    }
}

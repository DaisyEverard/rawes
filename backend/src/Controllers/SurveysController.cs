using Google.Cloud.BigQuery.V2;
using MainApi.Data.Survey;

namespace MainApi.Controllers
{
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
                //TableSchema schema = table.Schema;
                //return Results.Ok(schema);

                string metadataSql = $"""
                    SELECT
                    wetland_name,
                    gps_coordinates.x,
                    gps_coordinates.y,
                    date_completed
                    FROM {allSurveysTable}
                    WHERE survey_id = {surveyID.ToString()}
                    LIMIT 1
                    """;

                string assessorsSql = $"""
                    SELECT 
                    a
                    FROM {allSurveysTable} s, UNNEST(s.assessors) AS a
                    WHERE survey_id = {surveyID.ToString()}
                    LIMIT 1
                    """;

                string rowsSql = $"""
                    SELECT
                    benefit,
                    benefit_type,
                    description,
                    importance
                    FROM {allRowsTable}
                    WHERE survey_id = {surveyID.ToString()}
                    """;

                string scalesSql = $"""
                    SELECT
                    benefit,
                    s
                    FROM {allRowsTable} r, UNNEST(r.scale) AS s
                    WHERE survey_id = {surveyID.ToString()}
                    """;

                BigQueryParameter[] nullParameters = null;
                BigQueryResults metadata = client.ExecuteQuery(metadataSql, nullParameters);
                BigQueryResults assessors = client.ExecuteQuery(assessorsSql, nullParameters);
                BigQueryResults rows = client.ExecuteQuery(rowsSql, nullParameters);
                BigQueryResults scales = client.ExecuteQuery(scalesSql, nullParameters);

                //var surveyDto = SurveyDTO.ConvertResultToSurveyDTO(results);
                //return Results.Ok(surveyDto);

                //var rows = new List<SurveyRowDTO>();
                var surveyRows = new List<string>();

                BigQueryRow assessorsRow = assessors[0];
                BigQueryRow metadataRow = metadata[0];

                string assessor = (string)row["a"];
                surveyRows.Add(assessor);


                return Results.Ok(surveyRows);
            })
        .WithName("getTable")
        .WithOpenApi();
        }
}
}

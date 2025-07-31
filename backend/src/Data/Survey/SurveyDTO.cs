using Google.Cloud.BigQuery.V2;
using MainApi.Data.Queries;
using Microsoft.OpenApi.Any;
namespace MainApi.Data.Survey;

public class SurveyDTO
{
    public int SurveyId { get; set; }
    public required string WetlandName { get; set; }
    public required GPSCoordinates GPSCoordinates { get; set; }
    public required List<string> Assessors { get; set; }
    public required DateTime DateCompleted { get; set; }
    public required List<SurveyRowDTO> Rows { get; set; }

    public static SurveyDTO ConvertResultsToSurvey(int surveyID, BigQueryResults metadataResult, BigQueryResults assessorsResult,
    BigQueryResults rowsResult,
    BigQueryResults scalesResult) {
        var surveyRows = new List<string>();

        // METADATA
        BigQueryRow metadataRow = metadataResult.First();
        string wetlandName = (string)metadataRow["wetland_name"];
        double gpsX = (double)metadataRow["gps_x"];
        double gpsY = (double)metadataRow["gps_y"];
        var gpsCoordinates = new GPSCoordinates { x = gpsX, y = gpsY };
        DateTime dateCompleted = (DateTime)metadataRow["date_completed"];


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
            foreach (var field in benefitRow.Schema.Fields)
            {
                var name = field.Name;
                var value = benefitRow[name];
                Console.WriteLine($"{name}: {value}");
            }
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

        return survey;
    }

}

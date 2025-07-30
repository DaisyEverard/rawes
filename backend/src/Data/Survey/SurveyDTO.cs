using Google.Cloud.BigQuery.V2;
using Microsoft.OpenApi.Any;
namespace MainApi.Data.Survey;

public class SurveyDTO
{
    public required int SurveyId { get; set; }
    public required string WetlandName { get; set; }
    public required GPSCoordinates GPSCoordinates { get; set; }
    public required List<string> Assessors { get; set; }
    public required DateOnly DateCompleted { get; set; }
    public required List<SurveyRowDTO> Rows { get; set; }

    //public static Task<SurveyDTO> ConvertResultsToSurvey() { }

}

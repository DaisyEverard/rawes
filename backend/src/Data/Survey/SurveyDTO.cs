using Google.Cloud.BigQuery.V2;
using Microsoft.OpenApi.Any;
namespace MainApi.Data.Survey;

public class SurveyDTO
{
    public int SurveyId { get; set; }
    public string WetlandName { get; set; }
    public GPSCoordinates GPSCoordinates { get; set; }
    public List<string> Assessors { get; set; }
    public DateOnly DateCompleted { get; set; }
    public List<SurveyRowDTO> Rows { get; set; }

    public SurveyDTO(int survey_id, string wetland_name, GPSCoordinates gps_coordinates, List<string> assessors, DateOnly date_completed, List<SurveyRowDTO> rows)
    {
        SurveyId = survey_id;
        WetlandName = wetland_name;
        GPSCoordinates = gps_coordinates;
        Assessors = assessors;
        DateCompleted = date_completed;
        Rows = rows;
    }

    //public static Task<SurveyDTO> ConvertResultsToSurvey() { }

}

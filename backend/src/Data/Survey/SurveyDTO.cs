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
    //public Task<SurveyDTO> ConvertResultToSurveyDTO(BigQueryResults results)
    //{
    //    var rows = new List<SurveyRowDTO>();

    //    var rowValues = results[0]["rawRow"]["f"];

    //    int surveyId = rowValues[0]["v"];
    //    string wetlandName = rowValues[1]["v"];
    //    AnyType gpsCoordinates = rowValues[2]["v"];
    //    AnyType assessors = rowValues[3]["v"];
    //    DateOnly dateCompleted = rowValues[4]["v"];


    //    foreach (var row in results)
    //    {


    //        string benefit =
    //        string benefitType =
    //        string importanceType =
    //        float importanceValue =
    //        string[] scales =
    //        string description =
    //    }
    //    //       create new rowDTO and add to rows
    //    // return new surveyDTO

    //    //rawRow: { "f": ["v": value]}

    //    SurveyDTO dto = new SurveyDTO();
    //};

}

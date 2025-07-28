namespace MainApi.Data.Tables;

public class Surveys
{
    public int SurveyId { get; set; }
    public string WetlandName { get; set; }
    public float[] GPSCoordinates { get; set; }

    public string[] Assessors { get; set; }

    public DateOnly DateCompleted { get; set; }    
}

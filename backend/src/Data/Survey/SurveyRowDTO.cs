namespace MainApi.Data.Survey;

public class SurveyRowDTO
{
    public required string Benefit { get; set; }
    public required string BenefitType { get; set; }
    public required double Importance { get; set; }
    public required List<string> Scales { get; set; }
    public required string Description { get; set; }
    
}

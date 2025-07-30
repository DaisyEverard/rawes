namespace MainApi.Data.Survey;

public class SurveyRowDTO
{
    public string Benefit { get; set; }
    public string BenefitType { get; set; }
    public double Importance { get; set; }
    public List<string> Scales { get; set; }
    public string Description { get; set; }

    public SurveyRowDTO(string benefit,
        string benefitType,
        double importance,
        List<string> scales,
        string description)
    {
        Benefit = benefit;
        BenefitType = benefitType;
        Importance = importance;
        Scales = scales;
        Description = description;
    }
}

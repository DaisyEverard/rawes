namespace MainApi.Data.Tables;

    public class SurveyRows
    {
        public int RowId {  get; set; }
        public int SurveyId { get; set; }

        public int BenefitId { get; set; }

        public float Importance { get; set; }

        public int[] ScaleIds { get; set; }

        public string Description { get; set; }
    }


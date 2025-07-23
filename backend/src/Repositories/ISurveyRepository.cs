using MainApi.Data.Survey;

namespace MainApi.Repositories;


public interface ISurveyRepository
{
    //Task CreateSurvey();

    Task<SurveyDTO> GetSurveyByIdAsync(string Id);

    //Task GetAllAsync();

    //Task UpdateSurvey(int survey_id, string field, string newData);
}

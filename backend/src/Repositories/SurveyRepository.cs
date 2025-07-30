//using Google.Cloud.BigQuery.V2;
//using MainApi.Clients;
//using MainApi.Data.Survey;

//namespace MainApi.Repositories;

//public class SurveyRepository : ISurveyRepository
//{
//    private readonly BigQueryClient _client;
//    private readonly string _datasetId;
//    private readonly string _tableId;

//    public SurveyRepository(
//        BigQueryClientFactory clientFactory,
//        string datasetId,
//        string tableId)
//    {
//        _client = clientFactory.CreateClient();
//        _datasetId = datasetId;
//        _tableId = tableId;
//    }

//    public async Task<SurveyDTO> GetSurveyByIdAsync(string id)
//    {
//        var query = $"SELECT * FROM `{_datasetId}.{_tableId}` WHERE survey_id = @id LIMIT 1";
//        var results = await _client.ExecuteQueryAsync(query, new[] {
//        new BigQueryParameter("id", BigQueryDbType.String, id)
//    });

//        return results;
//    }
//}
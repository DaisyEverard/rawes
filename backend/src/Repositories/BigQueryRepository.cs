using Google.Cloud.BigQuery.V2;
using MainApi.Clients;

namespace backend.src.Repositories
{
    public class BigQueryRepository<T>: IBigQueryRepository<T>
    {
        private readonly BigQueryClient _client;
        private readonly string _datasetId;
        private readonly string _tableId;
        private readonly Func<BigQueryRow, T> _rowMapper;
        private readonly Func<T, BigQueryInsertRow> _rowBuilder;
    }

    public BigQueryRepository(
        BigQueryClientFactory clientFactory,
        string datasetId,
        string tableId,
        Func<BigQueryRow, T> rowMapper,
        Func<T, BigQueryInsertRow> rowBuilder)
        {
            _client = clientFactory.CreateClient();
            _datasetId = datasetId;
            _tableId = tableId;
            _rowMapper = rowMapper;
            _rowBuilder = rowBuilder;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var query = $"SELECT * FROM `{_datasetId}.{_tableId}`";
            var results = await _client.ExecuteQueryAsync(query, null);

            var list = new List<T>();
            await foreach (var row in results)
            {
                list.Add(_rowMapper(row));
            }

            return list;
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            var query = $"SELECT * FROM `{_datasetId}.{_tableId}` WHERE id = @id LIMIT 1";
            var results = await _client.ExecuteQueryAsync(query, new[] {
            new BigQueryParameter("id", BigQueryDbType.String, id)
        });

            await foreach (var row in results)
            {
                return _rowMapper(row);
            }

            return null;
        }

        public async Task InsertAsync(T entity)
        {
            var row = _rowBuilder(entity);
            await _client.InsertRowAsync(_datasetId, _tableId, row);
        }
    }

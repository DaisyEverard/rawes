// how to connect to database
// need sdk?
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Google.Cloud.BigQuery.V2;

namespace MainApi.Clients;

public class BigQueryClientFactory
{
    public IConfiguration _config { get; set; }

    public BigQueryClientFactory(IConfiguration config)
    {
        _config = config;
    }

    public BigQueryClient CreateClient()
    {
        var credentialPath = _config["GoogleCredentialPath"];

        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

        string projectId = _config["GoogleProjectId"];
        return BigQueryClient.Create(projectId);
    }
}


using CosmosConnectionIssue;
using Microsoft.Azure.Cosmos;
using System.Net.Http;

namespace RestCOSMOSDB;


public class Program
{
    private static string cosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    private static string baseUrl = "https://host.docker.internal:8081";
    public static async Task Main(string[] args)
    {
        await TestRestAPI();
        await TestDotNetClient();
    }

    static async Task TestDotNetClient()
    {
        var cosmos = new CosmosClient("AccountEndpoint=https://host.docker.internal:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            new()
            {
                ServerCertificateCustomValidationCallback = (_, _, _) => true
            });

        //vv will stall here unless on version 2.14.9 vv
        var db = await cosmos.CreateDatabaseIfNotExistsAsync("test-db-dotnet", 1000);
        var container = await db.Database.CreateContainerIfNotExistsAsync(new() { Id = "Plzwork", PartitionKeyPath = "/PartitionId" });
    }

    static async Task TestRestAPI()
    {
        var client = new RestAPIService(cosmosKey, baseUrl);
        await client.CreateDatabase("test-db-rest", DatabaseThoughputMode.none);
        await client.ListDatabases();
    }


}

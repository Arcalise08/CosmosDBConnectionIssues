namespace CosmosConnectionIssue;
internal class RestAPIService
{
    private readonly HttpClient httpClient;
    private readonly string CosmosKey;
    private readonly string BaseUrl;

    internal RestAPIService(string cosmosKey, string baseUrl)
    {
        var handler = new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl)
        };
        CosmosKey = cosmosKey;
        BaseUrl = baseUrl;
    }
    public async Task ListDatabases()
    {

        var method = HttpMethod.Get;

        var resourceType = ResourceType.dbs;
        var resourceLink = $"";
        var requestDateString = DateTime.UtcNow.ToString("r");
        var auth = Utility.GenerateMasterKeyAuthorizationSignature(method, resourceType, resourceLink, requestDateString, CosmosKey);

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("authorization", auth);
        httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
        httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");

        var requestUri = new Uri($"{BaseUrl}/dbs");
        var httpRequest = new HttpRequestMessage { Method = method, RequestUri = requestUri };

        var httpResponse = await httpClient.SendAsync(httpRequest);
        await ReportOutput($"List Databases:", httpResponse);
    }

    public async Task CreateDatabase(string databaseId, DatabaseThoughputMode mode)
    {
        var method = HttpMethod.Post;

        var resourceType = ResourceType.dbs;
        var resourceLink = $"";
        var requestDateString = DateTime.UtcNow.ToString("r");
        var auth = Utility.GenerateMasterKeyAuthorizationSignature(method, resourceType, resourceLink, requestDateString, CosmosKey);

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        httpClient.DefaultRequestHeaders.Add("authorization", auth);
        httpClient.DefaultRequestHeaders.Add("x-ms-date", requestDateString);
        httpClient.DefaultRequestHeaders.Add("x-ms-version", "2018-12-31");

        if (mode == DatabaseThoughputMode.@fixed)
            httpClient.DefaultRequestHeaders.Add("x-ms-offer-throughput", "400");
        if (mode == DatabaseThoughputMode.autopilot)
            httpClient.DefaultRequestHeaders.Add("x-ms-cosmos-offer-autopilot-settings", "{\"maxThroughput\": 4000}");

        var requestUri = new Uri($"{BaseUrl}/dbs");
        var requestBody = $"{{\"id\":\"{databaseId}\"}}";
        var requestContent = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

        var httpRequest = new HttpRequestMessage { Method = method, Content = requestContent, RequestUri = requestUri };

        var httpResponse = await httpClient.SendAsync(httpRequest);
        await ReportOutput($"Create Database with thoughput mode {mode}:", httpResponse);
    }

    private async Task ReportOutput(string methodName, HttpResponseMessage httpResponse)
    {
        var responseContent = await httpResponse.Content.ReadAsStringAsync();
        if (httpResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"{methodName}: SUCCESS\n    {responseContent}\n\n");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"{methodName}: FAILED -> {(int)httpResponse.StatusCode}: {httpResponse.ReasonPhrase}.\n    {responseContent}\n\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

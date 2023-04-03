using System.Net;

namespace CosmosConnectionIssue;
public enum ResourceType
{
    dbs,
    colls,
    docs,
    sprocs,
    pkranges,
}

public enum DatabaseThoughputMode
{
    none,
    @fixed,
    autopilot,
};

public class Utility
{
    public static string GenerateMasterKeyAuthorizationSignature(HttpMethod verb, ResourceType resourceType, string resourceLink, string date, string key)
    {
        var keyType = "master";
        var tokenVersion = "1.0";
        var payload = $"{verb.ToString().ToLowerInvariant()}\n{resourceType.ToString().ToLowerInvariant()}\n{resourceLink}\n{date.ToLowerInvariant()}\n\n";

        var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };
        var hashPayload = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
        var signature = Convert.ToBase64String(hashPayload);
        var authSet = WebUtility.UrlEncode($"type={keyType}&ver={tokenVersion}&sig={signature}");

        return authSet;
    }
}



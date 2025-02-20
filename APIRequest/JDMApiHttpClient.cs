using BotJDM.APIRequest.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace BotJDM.APIRequest;

public class JDMApiHttpClient
{
    private static HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://jdm-api.demo.lirmm.fr/v0/")
    };

    /// <summary>
    /// Return -1 if relation could not be found
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static async Task<int> GetRelationIdFromName(string name)
    {
        var rel = await GetRelationTypes();
        if (rel is null) return -1;
        foreach (var relation in rel)
        {
            if (relation.name == name) return relation.id;
        }
        return -1;
    }

    public static async Task<Node?> GetNodeById(int id)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"node_by_id/{id}");
        if (response is { StatusCode: System.Net.HttpStatusCode.OK })
        {
            Console.WriteLine("Successful request : ");
            response.EnsureSuccessStatusCode().WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Node? node = JsonSerializer.Deserialize<Node>(jsonResponse);

            return node;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            response.WriteRequestToConsole();
            return null;
        }
    }

    public static async Task<Node?> GetNodeByName(string _name)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"node_by_name/{_name}");
        if (response is { StatusCode: System.Net.HttpStatusCode.OK })
        {
            Console.WriteLine("Successful request : ");
            response.EnsureSuccessStatusCode().WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Node? node = JsonSerializer.Deserialize<Node>(jsonResponse);

            return node;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            response.WriteRequestToConsole();
            return null;
        }
    }

    public static async Task<Node?> GetNodeRefinement(string nodeName)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync($"refinements/{nodeName}");
        if (response is { StatusCode: System.Net.HttpStatusCode.OK })
        {
            Console.WriteLine("Successful request : ");
            response.EnsureSuccessStatusCode().WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            Node? node = JsonSerializer.Deserialize<Node>(jsonResponse);

            return node;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            response.WriteRequestToConsole();
            return null;
        }
    }

    public static async Task<List<NodeType>?> GetNodeTypes()
    {
        var response = await _httpClient.GetFromJsonAsync<List<NodeType>>("nodes_types");
        if (response!= null)
        {
            Console.WriteLine("Successful request : ");
            return response;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            return null;
        }
    }
    public static async Task<RelationRet?> GetRelationsFrom(string nodeName,
        int[]? typesIds = null,
        int[]? notTypesIds = null,
        int? minWeight = null,
        int? maxWeight = null,
        string[]? relationFields = null,
        string[]? nodeFields = null,
        int? limit = null,
        bool? withoutNodes = null)
    {
        #region Query
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (typesIds is not null && typesIds.Length > 0)
            query["types_ids"] = string.Join(",", typesIds);

        if (notTypesIds is not null && notTypesIds.Length > 0)
            query["not_types_ids"] = string.Join(",", notTypesIds);

        if (minWeight.HasValue)
            query["min_weight"] = minWeight.Value.ToString();

        if (maxWeight.HasValue)
            query["max_weight"] = maxWeight.Value.ToString();

        if (relationFields is not null && relationFields.Length > 0)
            query["relation_fields"] = string.Join(",", relationFields);

        if (nodeFields is not null && nodeFields.Length > 0)
            query["node_fields"] = string.Join(",", nodeFields);

        if (limit.HasValue)
            query["limit"] = limit.Value.ToString();

        if (withoutNodes.HasValue)
            query["without_nodes"] = withoutNodes.Value.ToString().ToLower(); // Convertit en "true"/"false"
        #endregion
        string requestUrl = $"relations/from/{nodeName}?{query}";

        using HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
        if (response is { StatusCode: System.Net.HttpStatusCode.OK })
        {
            Console.WriteLine("Successful request : ");
            response.EnsureSuccessStatusCode().WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            RelationRet? relationRet = JsonSerializer.Deserialize<RelationRet>(jsonResponse);

            return relationRet;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            response.WriteRequestToConsole();
            return null;
        }
    }
    public static async Task<RelationRet?> GetRelationsFromTo(string node1Name, string node2Name, int[]? typesIds = null,
        int[]? notTypesIds = null,
        int? minWeight = null,
        int? maxWeight = null,
        string[]? relationFields = null,
        string[]? nodeFields = null,
        int? limit = null,
        bool? withoutNodes = null)
    {
        #region Query
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (typesIds is not null && typesIds.Length > 0)
            query["types_ids"] = string.Join(",", typesIds);

        if (notTypesIds is not null && notTypesIds.Length > 0)
            query["not_types_ids"] = string.Join(",", notTypesIds);

        if (minWeight.HasValue)
            query["min_weight"] = minWeight.Value.ToString();

        if (maxWeight.HasValue)
            query["max_weight"] = maxWeight.Value.ToString();

        if (relationFields is not null && relationFields.Length > 0)
            query["relation_fields"] = string.Join(",", relationFields);

        if (nodeFields is not null && nodeFields.Length > 0)
            query["node_fields"] = string.Join(",", nodeFields);

        if (limit.HasValue)
            query["limit"] = limit.Value.ToString();

        if (withoutNodes.HasValue)
            query["without_nodes"] = withoutNodes.Value.ToString().ToLower(); // Convertit en "true"/"false"
        #endregion
        string requestUrl = $"relations/from/{node1Name}/to/{node2Name}?{query}";

        using HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
        if (response is { StatusCode: System.Net.HttpStatusCode.OK })
        {
            Console.WriteLine("Successful request : ");
            response.EnsureSuccessStatusCode().WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            RelationRet? relationRet = JsonSerializer.Deserialize<RelationRet>(jsonResponse);

            return relationRet;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            response.WriteRequestToConsole();
            return null;
        }
    }
    public static async Task<RelationRet?> GetRelationsTo(string node2Name, int[]? typesIds = null,
        int[]? notTypesIds = null,
        int? minWeight = null,
        int? maxWeight = null,
        string[]? relationFields = null,
        string[]? nodeFields = null,
        int? limit = null,
        bool? withoutNodes = null)
    {
        #region Query
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (typesIds is not null && typesIds.Length > 0)
            query["types_ids"] = string.Join(",", typesIds);

        if (notTypesIds is not null && notTypesIds.Length > 0)
            query["not_types_ids"] = string.Join(",", notTypesIds);

        if (minWeight.HasValue)
            query["min_weight"] = minWeight.Value.ToString();

        if (maxWeight.HasValue)
            query["max_weight"] = maxWeight.Value.ToString();

        if (relationFields is not null && relationFields.Length > 0)
            query["relation_fields"] = string.Join(",", relationFields);

        if (nodeFields is not null && nodeFields.Length > 0)
            query["node_fields"] = string.Join(",", nodeFields);

        if (limit.HasValue)
            query["limit"] = limit.Value.ToString();

        if (withoutNodes.HasValue)
            query["without_nodes"] = withoutNodes.Value.ToString().ToLower(); // Convertit en "true"/"false"
        #endregion
        string requestUrl = $"relations/to/{node2Name}?{query}";

        using HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
        if (response is { StatusCode: System.Net.HttpStatusCode.OK })
        {
            Console.WriteLine("Successful request : ");
            response.EnsureSuccessStatusCode().WriteRequestToConsole();
            var jsonResponse = await response.Content.ReadAsStringAsync();
            RelationRet? relationRet = JsonSerializer.Deserialize<RelationRet>(jsonResponse);

            return relationRet;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            response.WriteRequestToConsole();
            return null;
        }
    }
    public static async Task<List<RelationType>?> GetRelationTypes()
    {
        var response = await _httpClient.GetFromJsonAsync<List<RelationType>>("relations_types");
        if (response != null)
        {
            Console.WriteLine("Successful request : ");
            return response;
        }
        else
        {
            Console.WriteLine("Incorrect request : ");
            return null;
        }
    }

}
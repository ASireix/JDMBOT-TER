namespace BotJDM.APIRequest;

public class JDMApiHttpClient
{
    private static HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://jdm-api.demo.lirmm.fr/v0/")
    };
    
    static async Task GetAsync(HttpClient httpClient)
    {
        using HttpResponseMessage response = await httpClient.GetAsync("node_by_id/15");
    
        response.EnsureSuccessStatusCode()
            .WriteRequestToConsole();
    
        var jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"{jsonResponse}\n");
    }

    public async Task TestRequest()
    {
        await GetAsync(_httpClient);
    }
}
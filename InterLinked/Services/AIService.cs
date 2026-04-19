using System.Text;
using Newtonsoft.Json;

public class AIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AIService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string> GetResponse(string userMessage)
    {
        var apiKey = _config["AI:ApiKey"];
        var endpoint = _config["AI:Endpoint"];

        if (string.IsNullOrWhiteSpace(apiKey))
            return "API key is missing.";

        if (string.IsNullOrWhiteSpace(endpoint))
            return "API endpoint is missing.";

        var url = $"{endpoint}?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = $"You are an assistant for Linker, a platform for job, internship, and event applications.\n\nUser: {userMessage}"
                        }
                    }
                }
            }
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine(error);

                return "AI ERROR: " + error;
            }

            dynamic json = JsonConvert.DeserializeObject(result);

            var reply =
                json?.candidates?[0]?.content?.parts?[0]?.text;

            if (reply == null)
            {
                return "Invalid response from AI.";
            }

            return reply.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine("AI SERVICE ERROR: " + ex.Message);
            return "Something went wrong with the AI service.";
        }
    }
}

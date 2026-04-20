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
        var apiKey = _config["AI:ApiKey"]; // reuse your config key

        if (string.IsNullOrWhiteSpace(apiKey))
            return "API key is missing.";

        var url = "https://api.groq.com/openai/v1/chat/completions";

        var requestBody = new
        {
            model = "llama-3.1-8b-instant",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = "You are an assistant for InterLinked, a platform for job, internship, and event applications."
                },
                new
                {
                    role = "user",
                    content = userMessage
                }
            },
            temperature = 0.7
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(requestBody),
            Encoding.UTF8,
            "application/json"
        );

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(result);
                return "AI ERROR: " + result;
            }

            dynamic json = JsonConvert.DeserializeObject(result);

            var reply =
                json?.choices?[0]?.message?.content;

            if (reply == null)
                return "Invalid response from AI.";

            return reply.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine("AI SERVICE ERROR: " + ex.Message);
            return "Something went wrong with the AI service.";
        }
    }
}
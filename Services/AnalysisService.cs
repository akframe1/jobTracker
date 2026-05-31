public class AnalysisService : IAnalysisService
{
    private readonly IConfiguration _config;

    public AnalysisService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<string> AnalyseAsync(string jobDescription)
    {
        var apiKey = _config["Groq:ApiKey"];

        var payload = new
        {
            model = "llama-3.1-8b-instant",
            messages = new[]
            {
                new {
                    role = "user",
                    content = $"Analyse this job description and return three things: 1. A 2 sentence summary of the role 2. Top 5 key skills required 3. Three suggested talking points for an interview. Job description: {jobDescription}"
                }
            }
        };

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        var url = "https://api.groq.com/openai/v1/chat/completions";
        var response = await http.PostAsJsonAsync(url, payload);
        var rawJson = await response.Content.ReadAsStringAsync();

        var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = System.Text.Json.JsonSerializer.Deserialize<GroqResponse>(rawJson, options);

        return result?.Choices?[0]?.Message?.Content ?? "No response received.";
    }
}
public class GroqResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("choices")]
    public GroqChoice[]? Choices { get; set; }
}

public class GroqChoice
{
    [System.Text.Json.Serialization.JsonPropertyName("message")]
    public GroqMessage? Message { get; set; }
}

public class GroqMessage
{
    [System.Text.Json.Serialization.JsonPropertyName("content")]
    public string? Content { get; set; }
}
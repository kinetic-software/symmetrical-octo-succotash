using System.Text.Json.Serialization;

namespace Kx.Core.HttpMocks.HttpMocks;

public class MockRequest
{
  [JsonPropertyName("path")]
  public string Path { get; set; } = string.Empty;
  [JsonPropertyName("type")]
  public string Type { get; set; } = string.Empty;
  [JsonPropertyName("requestBody")]
  public string? Body { get; set; }
  [JsonPropertyName("responseCode")]
  public int ResponseCode { get; set; }
  [JsonPropertyName("response")]
  public string? Response { get; set; } 
}

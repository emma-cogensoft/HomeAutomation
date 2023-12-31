using System.Text.Json.Nodes;

namespace HomeAutomation.LocalInverter.ApiAccessor;

public interface ILocalInverterApiAccessor
{
    Task<JsonObject> GetJsonAsync(Uri uri, string body, CancellationToken cancellationToken);
    Task<string> GetStringAsync(Uri uri, string body, CancellationToken cancellationToken);
}
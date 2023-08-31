using System.Text.Json.Nodes;

namespace HomeAutomation.CloudInverter.ApiAccessor;

public interface ICloudInverterApiAccessor
{
    Task<JsonObject> GetJsonAsync(Uri uri, CancellationToken cancellationToken);
}
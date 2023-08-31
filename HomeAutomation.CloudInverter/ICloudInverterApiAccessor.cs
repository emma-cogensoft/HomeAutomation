using System.Text.Json.Nodes;

namespace HomeAutomation.CloudInverter;

public interface ICloudInverterApiAccessor
{
    Task<JsonObject> ReadAsync(Uri uri, CancellationToken cancellationToken);
}
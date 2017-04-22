using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Card;
using Newtonsoft.Json;
using System.IO;

namespace Net.Chdk.Detectors
{
    public abstract class MetadataDetector<TDetector, TValue>
        where TDetector : MetadataDetector<TDetector, TValue>
        where TValue : class
    {
        protected ILogger Logger { get; }

        protected MetadataDetector(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<TDetector>();
        }

        protected abstract string FileName { get; }

        protected TValue GetValue(CardInfo cardInfo)
        {
            string metadataPath = cardInfo.GetMetadataPath();
            var filePath = Path.Combine(metadataPath, FileName);
            if (!File.Exists(filePath))
            {
                Logger.LogTrace("{0} not found", filePath);
                return null;
            }

            Logger.LogInformation("Reading {0}", filePath);

            using (var stream = File.OpenRead(filePath))
            {
                try
                {
                    return JsonObject.Deserialize<TValue>(stream);
                }
                catch (JsonException ex)
                {
                    Logger.LogError(0, ex, "Error deserializing");
                    throw;
                }
            }
        }
    }
}

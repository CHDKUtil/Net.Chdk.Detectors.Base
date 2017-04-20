using Microsoft.Extensions.Logging;
using Net.Chdk.Model.Card;
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
            var metadataPath = Path.Combine(cardInfo.DriveLetter, "METADATA");
            var filePath = Path.Combine(metadataPath, FileName);
            if (!File.Exists(filePath))
                return null;

            Logger.LogInformation("Reading {0}", filePath);

            using (var stream = File.OpenRead(filePath))
            {
                return JsonObject.Deserialize<TValue>(stream);
            }
        }
    }
}

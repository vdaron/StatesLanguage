using Newtonsoft.Json.Linq;
using StatesLanguage.States;

namespace StatesLanguage.Interfaces
{
    public interface IInputOutputProcessor
    {
        JToken GetEffectiveInput(JToken input, OptionalString inputPath, JObject payload, JObject context);
        JToken GetEffectiveResult(JToken output, JObject payload, JObject context);
        JToken GetEffectiveOutput(JToken input, JToken result, OptionalString outputPath, OptionalString resultPath);
        JToken GetFailPathValue(JToken input, OptionalString failPath, JObject payload, JObject context);
    }
}
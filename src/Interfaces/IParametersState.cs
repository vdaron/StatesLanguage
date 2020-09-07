using Newtonsoft.Json.Linq;

namespace StatesLanguage.Interfaces
{
    public interface IParametersState : IInputOutputState
    {
        string ResultPath { get; }
        JObject Parameters { get; }
    }
}
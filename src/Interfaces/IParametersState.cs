using Newtonsoft.Json.Linq;
using StatesLanguage.States;

namespace StatesLanguage.Interfaces
{
    public interface IParametersState : IInputOutputState
    {
        OptionalString ResultPath { get; }
        JObject Parameters { get; }
    }
}
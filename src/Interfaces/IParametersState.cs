using Newtonsoft.Json.Linq;

namespace StatesLanguage.Interfaces
{
    public interface IParametersState : IInputOutputState
    {
        JToken Parameters { get; }
    }
}
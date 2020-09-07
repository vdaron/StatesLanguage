using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Internal;

namespace StatesLanguage.States
{
    public abstract class ParameterState : TransitionState, IParametersState
    {
        /// <summary>
        ///     MUST be a Reference Path, which specifies the raw input’s combination with or replacement by the state’s result.
        /// </summary>
        [JsonProperty(PropertyNames.RESULT_PATH)]
        public OptionalString ResultPath { get; protected set; }

        /// <summary>
        ///     MUST be a Payload Template which is a JSON object, whose input is the result of applying the InputPath to the raw
        ///     input.
        ///     If the "Parameters" field is provided, its payload, after the extraction and embedding, becomes the effective
        ///     input.
        /// </summary>
        [JsonProperty(PropertyNames.PARAMETERS)]
        public JObject Parameters { get; protected set; }
    }
}
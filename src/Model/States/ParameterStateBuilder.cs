using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    public abstract class ParameterStateBuilder<T, B> : TransitionStateBuilder<T,B>
        where T : State
        where B : ParameterStateBuilder<T, B>
    {
        [JsonProperty(PropertyNames.RESULT_PATH)]
        protected OptionalString _resultPath;

        [JsonProperty(PropertyNames.PARAMETERS)]
        protected JObject _parameters;
        
        /// <summary>
        /// The value of “ResultPath” MUST be a Reference Path, which specifies the combination with or replacement of
        /// the state’s result with its raw input. If not provided then the output completely replaces the input.
        /// </summary>
        /// <param name="resultPath">New path value.</param>
        /// <returns>This object for method chaining.</returns>
        public B ResultPath(ReferencePath resultPath)
        {
            _resultPath = resultPath.Path;
            return (B) this;
        }

        /// <summary>
        /// Payload used to compute th input
        /// </summary>
        /// <param name="parameters">Payload</param>
        /// <returns>This object for method chaining.</returns>
        public B Parameters(JObject parameters)
        {
            _parameters = parameters;
            return (B) this;
        }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    public abstract class ParameterState : TransitionState, IParametersState
    {
        [JsonProperty(PropertyNames.RESULT_PATH)]
        public string ResultPath { get; protected set; }
        
        [JsonProperty(PropertyNames.PARAMETERS)]
        public JObject Parameters { get; protected set; }
    }
}

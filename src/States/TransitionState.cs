using Newtonsoft.Json;

namespace StatesLanguage.States
{
    public abstract class TransitionState : InputOutputState
    {
        [JsonIgnore]
        public ITransition Transition { get; protected set; }

        [JsonIgnore]
        public override bool IsTerminalState => Transition != null && Transition.IsTerminal;
    }
}
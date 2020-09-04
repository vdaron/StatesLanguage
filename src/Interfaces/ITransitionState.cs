using StatesLanguage.States;

namespace StatesLanguage.Interfaces
{
    public interface ITransitionState : IInputOutputState
    {
        ITransition Transition { get; }
    }
}
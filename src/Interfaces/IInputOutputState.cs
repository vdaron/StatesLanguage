using StatesLanguage.Model.States;

namespace StatesLanguage.Interfaces
{
    public interface IInputOutputState : IState
    {
        OptionalString InputPath { get; }
        OptionalString OutputPath { get; }
    }
}
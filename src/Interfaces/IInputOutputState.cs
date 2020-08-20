namespace StatesLanguage.Interfaces
{
    public interface IInputOutputState : IState
    {
        string InputPath { get; }
        string OutputPath { get; }
    }
}
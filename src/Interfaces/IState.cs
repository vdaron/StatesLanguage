using StatesLanguage.Model.States;

namespace StatesLanguage.Interfaces
{
    public interface IState
    {
        StateType Type { get; }
        string Comment { get; }
    }
}
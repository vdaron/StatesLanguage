using System.Collections.Generic;
using StatesLanguage.Model.States;

namespace StatesLanguage.Interfaces
{
    public interface IRetryCatchState : ITransitionState
    {
        IList<Retrier> Retriers { get; }

        IList<Catcher> Catchers { get; }
    }
}
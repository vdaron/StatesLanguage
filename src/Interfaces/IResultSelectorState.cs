using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StatesLanguage.Model.States;

namespace StatesLanguage.Interfaces
{
    public interface IResultState : IParametersState
    {
        JObject ResultSelector { get; }
        
        IList<Retrier> Retriers { get; }

        IList<Catcher> Catchers { get; }
    }
}
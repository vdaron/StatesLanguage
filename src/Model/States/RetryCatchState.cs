using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StatesLanguage.Interfaces;
using StatesLanguage.Model.Internal;

namespace StatesLanguage.Model.States
{
    public abstract class RetryCatchState : ParameterState, IResultState
    {
        [JsonProperty(PropertyNames.RESULT_SELECTOR)]
        public JObject ResultSelector { get; protected set; }

        [JsonProperty(PropertyNames.RETRY)]
        public IList<Retrier> Retriers { get; protected set; }

        [JsonProperty(PropertyNames.CATCH)]
        public IList<Catcher> Catchers { get; protected set; }
    }

    public abstract class RetryCatchStateBuilder<T, B> : ParameterStateBuilder<T, B>
        where T : State
        where B : RetryCatchStateBuilder<T, B>
    {
        [JsonProperty(PropertyNames.CATCH)] protected List<Catcher.Builder> _catchers = new List<Catcher.Builder>();

        [JsonProperty(PropertyNames.RESULT_SELECTOR)]
        protected JObject _resultSelector;

        [JsonProperty(PropertyNames.RETRY)] protected List<Retrier.Builder> _retriers = new List<Retrier.Builder>();

        internal RetryCatchStateBuilder()
        {
        }

        /// <summary>
        ///     OPTIONAL. Adds the <see cref="Retrier" /> to this states retries. If a single branch fails then the entire parallel
        ///     state is
        ///     considered failed and eligible for retry.
        /// </summary>
        /// <param name="retrierBuilders">
        ///     Instances of <see cref="Retrier.Builder" />. Note that the <see cref="Retrier" /> object is not built
        ///     until the <see cref="ParallelState" /> is built so any modifications on the state model will be reflected in this
        ///     object.
        /// </param>
        /// <returns>This object for method chaining.</returns>
        public B Retriers(params Retrier.Builder[] retrierBuilders)
        {
            _retriers.AddRange(retrierBuilders);
            return (B) this;
        }

        /// <summary>
        ///     OPTIONAL. Adds the {@link Retrier} to this states retries. If a single branch fails then the entire parallel state
        ///     is
        ///     considered failed and eligible for retry.
        /// </summary>
        /// <param name="retrierBuilder">
        ///     Instance of <see cref="Retrier.Builder" />. Note that the <see cref="Retrier" /> object is not built
        ///     until the <see cref="ParallelState" /> is built so any modifications on the state model will be reflected in this
        ///     object.
        /// </param>
        /// <returns>This object for method chaining.</returns>
        public B Retrier(Retrier.Builder retrierBuilder)
        {
            _retriers.Add(retrierBuilder);
            return (B) this;
        }

        /// <summary>
        ///     OPTIONAL. Adds the <see cref="Catcher" />s to this states catchers.  If a single branch fails then the entire
        ///     parallel state
        ///     is considered failed and eligible to be caught.
        /// </summary>
        /// <param name="catcherBuilders">
        ///     Instances of <see cref="Catcher.Builder" />. Note that the <see cref="Catcher" /> object is not built until the
        ///     <see cref="ParallelState" /> is built so any modifications on the state model will be reflected in this object.
        /// </param>
        /// <returns>This object for method chaining.</returns>
        public B Catchers(params Catcher.Builder[] catcherBuilders)
        {
            _catchers.AddRange(catcherBuilders);
            return (B) this;
        }

        /// <summary>
        ///     OPTIONAL. Adds the <see cref="Catcher" /> to this states catchers.  If a single branch fails then the entire
        ///     parallel state
        ///     is considered failed and eligible to be caught.
        /// </summary>
        /// <param name="catcherBuilder">
        ///     Instance of <see cref="Catcher.Builder" />. Note that the <see cref="Catcher" /> object is not built until the
        ///     <see cref="ParallelState" /> is built so any modifications on the state model will be reflected in this object.
        /// </param>
        /// <returns>This object for method chaining.</returns>
        public B Catcher(Catcher.Builder catcherBuilder)
        {
            _catchers.Add(catcherBuilder);
            return (B) this;
        }

        public B ResultSelector(JObject resultSelector)
        {
            _resultSelector = resultSelector;
            return (B) this;
        }
    }
}
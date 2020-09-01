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

    public abstract class RetryCatchStateBuilder<T, B> : TransitionStateBuilder<T, B>
        where T : State
        where B : RetryCatchStateBuilder<T, B>
    {
        [JsonProperty(PropertyNames.CATCH)] protected List<Catcher.Builder> _catchers = new List<Catcher.Builder>();

        [JsonProperty(PropertyNames.RETRY)] protected List<Retrier.Builder> _retriers = new List<Retrier.Builder>();

        internal RetryCatchStateBuilder()
        {
        }

        /**
             * OPTIONAL. Adds the {@link Retrier}s to this states retries. If a single branch fails then the entire parallel state is
             * considered failed and eligible for retry.
             *
             * @param retrierBuilders Instances of {@link Retrier.Builder}. Note that the {@link
             *                        Retrier} object is not built until the {@link ParallelState} is built so any modifications on
             *                        the state model will be reflected in this object.
             * @return This object for method chaining.
             */
        public B Retriers(params Retrier.Builder[] retrierBuilders)
        {
            _retriers.AddRange(retrierBuilders);
            return (B) this;
        }

        /**
             * OPTIONAL. Adds the {@link Retrier} to this states retries. If a single branch fails then the entire parallel state is
             * considered failed and eligible for retry.
             *
             * @param retrierBuilder Instance of {@link Retrier.Builder}. Note that the {@link
             *                       Retrier} object is not built until the {@link ParallelState} is built so any modifications on
             *                       the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
        public B Retrier(Retrier.Builder retrierBuilder)
        {
            _retriers.Add(retrierBuilder);
            return (B) this;
        }

        /**
             * OPTIONAL. Adds the {@link Catcher}s to this states catchers.  If a single branch fails then the entire parallel state
             * is considered failed and eligible to be caught.
             *
             * @param catcherBuilders Instances of {@link Catcher.Builder}. Note that the {@link
             *                        Catcher} object is not built until the {@link ParallelState} is built so any modifications on
             *                        the state model will be reflected in this object.
             * @return This object for method chaining.
             */
        public B Catchers(params Catcher.Builder[] catcherBuilders)
        {
            _catchers.AddRange(catcherBuilders);
            return (B) this;
        }

        /**
             * OPTIONAL. Adds the {@link Catcher} to this states catchers.  If a single branch fails then the entire parallel state
             * is
             * considered failed and eligible to be caught.
             *
             * @param catcherBuilder Instance of {@link Catcher.Builder}. Note that the {@link
             *                       Catcher} object is not built until the {@link ParallelState} is built so any modifications on
             *                       the
             *                       state model will be reflected in this object.
             * @return This object for method chaining.
             */
        public B Catcher(Catcher.Builder catcherBuilder)
        {
            _catchers.Add(catcherBuilder);
            return (B) this;
        }
    }
}

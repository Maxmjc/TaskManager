using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.File
{
    public static class ChangeToken
    {
        public static ChangeTokenRegistration<Action> OnChange(Func<IChangeToken> changeTokenProducer, Action changeTokenConsumer)
        {
            return new ChangeTokenRegistration<Action>(changeTokenProducer, callback => callback(), changeTokenConsumer);
        }
    }
    public class ChangeTokenRegistration<TAction>
    {
        private readonly Func<IChangeToken> _changeTokenProducer;
        private readonly Action<TAction> _changeTokenConsumer;
        private readonly TAction _state;

        public ChangeTokenRegistration(Func<IChangeToken> changeTokenProducer, Action<TAction> changeTokenConsumer, TAction state)
        {
            _changeTokenProducer = changeTokenProducer;
            _changeTokenConsumer = changeTokenConsumer;
            _state = state;

            var token = changeTokenProducer();

            RegisterChangeTokenCallback(token);
        }

        private void RegisterChangeTokenCallback(IChangeToken token)
        {
            token.RegisterChangeCallback(_ => OnChangeTokenFired(), this);
        }

        private void OnChangeTokenFired()
        {
            var token = _changeTokenProducer();

            try
            {
                _changeTokenConsumer(_state);
            }
            finally
            {
                // We always want to ensure the callback is registered
                RegisterChangeTokenCallback(token);
            }
        }
    }

    //public static class ChangeToken
    //{
    //    public static IDisposable OnChange(Func<IChangeToken> changeTokenProducer, Action changeTokenConsumer)
    //    {
    //        Action<object> callback = null;
    //        callback = delegate (object s)
    //        {
    //            changeTokenConsumer();
    //            changeTokenProducer().RegisterChangeCallback(callback, null);
    //        };
    //        return changeTokenProducer().RegisterChangeCallback(callback, null);
    //    }
    //}
}

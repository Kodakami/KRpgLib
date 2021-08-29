using System;

namespace KRpgLib.Utility
{
    public sealed class Timer
    {
        private readonly Action _onExpireAction;

        private int _current;
        public int Current => _current;
        public bool Expired { get; private set; }

        public Timer(int initialValue, Action onExpireAction)
        {
            _onExpireAction = onExpireAction ?? throw new ArgumentNullException(nameof(onExpireAction));

            SetValue(initialValue);
        }
        public void SetValue(int newValue)
        {
            _current = newValue;
            Expired = false;
        }

        public void Tick(int subtractedValue = 1)
        {
            if (Expired)
            {
                return;
            }

            _current -= subtractedValue;

            if (_current <= 0)
            {
                // Reminder: Expiration comes before invocation. Action involved resetting the timer, but expiration was AFTER the reset. Subtle bug was hard to trace.
                Expired = true;
                _onExpireAction.Invoke();
            }
        }
    }
}

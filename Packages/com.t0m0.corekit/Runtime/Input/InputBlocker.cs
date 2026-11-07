using System;
using UnityEngine;

namespace CoreKit.Inputs
{
    public static class InputBlocker
    {
        private static int _count;
        public static bool IsBlocked => _count > 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => _count = 0;

        /// <summary>using ‚ÅˆÍ‚ß‚Î©“®‰ğœB•¡”‰ÓŠ‚©‚çŒÄ‚ñ‚Å‚à‹£‡‚µ‚È‚¢</summary>
        public static IDisposable Block()
        {
            _count++;
            return new Token(() => _count = Mathf.Max(0, _count - 1));
        }

        internal sealed class Token : IDisposable
        {
            private Action _onDispose;
            public Token(Action onDispose) => _onDispose = onDispose;
            public void Dispose() { _onDispose?.Invoke(); _onDispose = null; }
        }
    }
}
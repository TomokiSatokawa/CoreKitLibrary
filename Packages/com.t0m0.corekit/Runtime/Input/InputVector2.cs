using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public sealed class InputVector2 : InputBase
    {
        public InputVector2(string path) : base(path) { }

        public Vector2 Value => InputBlocker.IsBlocked ? default : Ready().ReadValue<Vector2>();

        public IDisposable OnChanged(Action<Vector2> callback)
        {
            var a = Ready();
            void Handler(InputAction.CallbackContext c)
            {
                if (!InputBlocker.IsBlocked) callback(c.ReadValue<Vector2>());
            }
            a.performed += Handler;
            a.canceled += Handler;
            return new InputBlocker.Token(() =>
            {
                a.performed -= Handler;
                a.canceled -= Handler;
            });
        }
    }
}
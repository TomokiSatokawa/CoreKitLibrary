using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public sealed class InputVector2 : InputBase
    {
        public InputVector2(string path, InputGroup group = null) : base(path, group) { }

        public Vector2 Value => IsBlocked ? default : Ready().ReadValue<Vector2>();

        public IDisposable OnChanged(Action<Vector2> callback)
        {
            var a = Ready();
            void Handler(InputAction.CallbackContext c)
            {
                if (!IsBlocked) callback(c.ReadValue<Vector2>());
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
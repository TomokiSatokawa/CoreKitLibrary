using System;
using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public sealed class InputButton : InputBase
    {
        public InputButton(string path) : base(path) { }

        public bool Value => !InputBlocker.IsBlocked && Ready().IsPressed();
        public bool Down => !InputBlocker.IsBlocked && Ready().WasPressedThisFrame();
        public bool Up => !InputBlocker.IsBlocked && Ready().WasReleasedThisFrame();

        public IDisposable OnPressed(Action callback)
        {
            var a = Ready();
            void Handler(InputAction.CallbackContext _) { if (!InputBlocker.IsBlocked) callback(); }
            a.performed += Handler;
            return new InputBlocker.Token(() => a.performed -= Handler);
        }

        public IDisposable OnReleased(Action callback)
        {
            var a = Ready();
            void Handler(InputAction.CallbackContext _) { if (!InputBlocker.IsBlocked) callback(); }
            a.canceled += Handler;
            return new InputBlocker.Token(() => a.canceled -= Handler);
        }
    }
}
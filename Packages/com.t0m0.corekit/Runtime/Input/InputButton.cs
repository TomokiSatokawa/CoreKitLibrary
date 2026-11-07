using System;
using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public sealed class InputButton : InputBase
    {
        public InputButton(string path, InputGroup group = null) : base(path, group) { }

        public bool Key => !IsBlocked && Ready().IsPressed();
        public bool Down => !IsBlocked && Ready().WasPressedThisFrame();
        public bool Up => !IsBlocked && Ready().WasReleasedThisFrame();

        public IDisposable OnPressed(Action callback)
        {
            var a = Ready();
            void Handler(InputAction.CallbackContext _) { if (!IsBlocked) callback(); }
            a.performed += Handler;
            return new InputBlocker.Token(() => a.performed -= Handler);
        }

        public IDisposable OnReleased(Action callback)
        {
            var a = Ready();
            void Handler(InputAction.CallbackContext _) { if (!IsBlocked) callback(); }
            a.canceled += Handler;
            return new InputBlocker.Token(() => a.canceled -= Handler);
        }
    }
}
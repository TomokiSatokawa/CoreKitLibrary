using System;
using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public sealed class InputButton : InputBase
    {
        public InputButton(string path, InputGroup group = null) : base(path, group) { }

        public bool GetKey() => !IsBlocked && Ready().IsPressed();
        public bool GetKeyDown() => !IsBlocked && Ready().WasPressedThisFrame();
        public bool GetKeyUp() => !IsBlocked && Ready().WasReleasedThisFrame();

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
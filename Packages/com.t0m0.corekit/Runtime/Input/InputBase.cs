using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public abstract class InputBase
    {
        private static readonly List<InputBase> All = new();
        private readonly string _path;
        private readonly InputGroup _group;
        private InputAction _action;

        protected InputBase(string path, InputGroup group = null)
        {
            _path = path;
            _group = group;
            All.Add(this);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAll()
        {
            foreach (var i in All) i._action = null;
            InputSource.Reset();
        }

        // 全体ブロック or グループのロック
        internal bool IsBlocked => InputBlocker.IsBlocked || (_group != null && _group.IsBlocked);

        internal InputAction Ready()
        {
            _action ??= InputSource.Resolve().FindAction(_path, throwIfNotFound: true);
            if (!_action.enabled) _action.Enable();
            return _action;
        }
    }
}
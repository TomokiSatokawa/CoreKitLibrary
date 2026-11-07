using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreKit.Inputs
{
    public abstract class InputBase
    {
        private static readonly List<InputBase> All = new();
        private readonly string _path;
        private InputAction _action;

        // 生成コードから呼ばれる（"Player/Dash" のようなパス）
        protected InputBase(string path)
        {
            _path = path;
            All.Add(this);
        }

        // ドメインリロード無効時に、前回Playのキャッシュを捨てる
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAll()
        {
            foreach (var i in All) i._action = null;
            InputSource.Reset();
        }

        // 初回アクセス時にアセットから解決する
        internal InputAction Ready()
        {
            _action ??= InputSource.Resolve().FindAction(_path, throwIfNotFound: true);
            if (!_action.enabled) _action.Enable();
            return _action;
        }
    }
}
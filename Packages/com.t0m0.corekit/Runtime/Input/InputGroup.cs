using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreKit.Inputs
{
    /// <summary>アクションマップ単位でロックするための単位。生成コードが継承する</summary>
    public abstract class InputGroup
    {
        private static readonly List<InputGroup> All = new();

        private bool _enabled = true;
        private int _blockCount;

        public string Name { get; }
        public bool IsEnabled => _enabled;
        public bool IsBlocked => !_enabled || _blockCount > 0;

        protected InputGroup(string name)
        {
            Name = name;
            All.Add(this);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetAll()
        {
            foreach (var g in All)
            {
                g._enabled = true;
                g._blockCount = 0;
            }
        }

        /// <summary>単純なON/OFF。複数箇所から操作する場合は Block() を使う</summary>
        public void SetEnabled(bool enabled) => _enabled = enabled;

        /// <summary>using で囲めば自動解除。複数箇所から呼んでも競合しない</summary>
        public IDisposable Block()
        {
            _blockCount++;
            return new InputBlocker.Token(() => _blockCount = Mathf.Max(0, _blockCount - 1));
        }
    }
}
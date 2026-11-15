using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreKit.UI.Selector
{
    /// <summary>
    /// 複数選択可能なSelectableの選択状態を管理する。
    /// </summary>
    public class MultiSelectionController<T> : IDisposable where T : ISelectable<T>
    {
        private readonly T[] _selectableArray;
        private readonly bool[] _selectStates;
        private readonly Dictionary<T, int> _indexMap;

        public MultiSelectionController(T[] selectables,params int[] startSelectIndex)
        {
            _selectableArray = selectables;
            _selectStates = new bool[selectables.Length];
            _indexMap = new Dictionary<T, int>(selectables.Length);

            for (var i = 0; i < _selectableArray.Length; i++)
            {
                var selectable = _selectableArray[i];
                _indexMap.Add(selectable, i);
                selectable.OnClick += OnSelect;
                selectable.OnDeselect();
            }

            foreach (var index in startSelectIndex)
            {
                if (!IsValidIndex(index))
                {
                    Debug.LogError($"[MultiSelectionController] 開始選択のindexが不正です: {index}");
                    continue;
                }

                _selectStates[index] = true;
                _selectableArray[index].OnSelect();
            }
        }

        public void SetSelect(int index, bool setValue)
        {
            if (!IsValidIndex(index))
            {
                Debug.LogError($"[MultiSelectionController] 選択indexが不正です: {index}");
                return;
            }

            ApplySelect(index, setValue);
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _selectableArray.Length;
        }

        private void ApplySelect(int index, bool value)
        {
            if (_selectStates[index] == value)
                return;

            _selectStates[index] = value;

            var target = _selectableArray[index];
            if (value)
                target.OnSelect();
            else
                target.OnDeselect();
        }

        private void OnSelect(T selectable)
        {
            if (!_indexMap.TryGetValue(selectable, out var index))
            {
                Debug.LogError($"[MultiSelectionController] 未登録のSelectableです: {selectable}");
                return;
            }

            ApplySelect(index, !_selectStates[index]);
        }

        public void Dispose()
        {
            foreach (var selectable in _selectableArray)
                selectable.OnClick -= OnSelect;
        }
    }
}
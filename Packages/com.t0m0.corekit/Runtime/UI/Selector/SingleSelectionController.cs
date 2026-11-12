using System;
using System.Collections.Generic;

namespace CoreKit.UI.Selector
{
    /// <summary>
    /// 選択可能な要素の選択状態を管理する
    /// </summary>
    public class SingleSelectionController<T> : IDisposable where T : ISelectable<T>
    {
        private readonly T[] _selectableArray;

        public T CurrentSelect { get; private set; }

        public SingleSelectionController(T[] selectables, int startSelectIndex)
        {
            _selectableArray = selectables;

            foreach (var selectable in _selectableArray)
                selectable.OnClick += OnSelect;

            SetSelect(startSelectIndex);
        }

        public void SetSelect(int index)
        {
            if (index < 0 || index >= _selectableArray.Length)
            {
                UnityEngine.Debug.LogError($"Invalid selection index: {index}");
                return;
            }

            OnSelect(_selectableArray[index]);
        }

        private void OnSelect(T selectable)
        {
            if (EqualityComparer<T>.Default.Equals(CurrentSelect, selectable))
                return;

            CurrentSelect?.OnDeselect();

            CurrentSelect = selectable;
            CurrentSelect.OnSelect();
        }

        public void Dispose()
        {
            foreach (var selectable in _selectableArray)
                selectable.OnClick -= OnSelect;
        }
    }
}
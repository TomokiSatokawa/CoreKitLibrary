using System;

namespace CoreKit.UI.Selector
{
    public interface ISelectable<T>
    {
        public event Action<T> OnClick;
        public void OnSelect();
        public void OnDeselect();
    }
}

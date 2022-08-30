using System;

public interface IView<T> where T : ViewModelBase
{
    T BindingContext { get; set; }
    void Show(bool immediate = false, Action action = null);
    void Hide(bool immediate = false, Action action = null);
}
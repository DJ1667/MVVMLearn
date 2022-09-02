using System;
using UnityEngine;

public interface IView
{
    ViewModelBase BindingContext { get; set; }
    GameObject SelfGameObject { get; }
    void Initialize();
    void Show(bool immediate = false, Action startAction = null, Action finishAction = null);
    void Hide(bool immediate = false, Action startAction = null, Action finishAction = null);
}
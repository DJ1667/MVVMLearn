using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class ViewBase<T> : MonoBehaviour, IView<T> where T : ViewModelBase
{
    private CanvasGroup _canvasGroup;

    public CanvasGroup CanvasGroup
    {
        get
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }
    }

    private bool _isInitialized;

    protected readonly PropertyBinder<T> Binder = new PropertyBinder<T>();
    private readonly BindableProperty<T> ViewModelProperty = new BindableProperty<T>();

    /// <summary>
    /// 显示之后的回调
    /// </summary>
    private Action ShowedAction { get; set; }

    /// <summary>
    /// 隐藏之后的回调
    /// </summary>
    private Action HiddenAction { get; set; }

    public T BindingContext
    {
        get { return ViewModelProperty.Value; }
        set
        {
            if (!_isInitialized)
            {
                OnInitialized();
                _isInitialized = true;
            }

            //触发OnValueChanged事件
            ViewModelProperty.Value = value;
        }
    }

    public void Show(bool immediate = false, Action action = null)
    {
        if (action != null)
        {
            ShowedAction += action;
        }

        OnShowStart(immediate);
    }

    public void Hide(bool immediate = false, Action action = null)
    {
        if (action != null)
        {
            HiddenAction += action;
        }

        OnHideStart(immediate);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 初始化View，当BindingContext改变时执行
    /// </summary>
    protected virtual void OnInitialized()
    {
        ViewModelProperty.OnValueChanged += OnBindingContextChanged;
    }

    protected virtual void OnShowStart(bool immediate)
    {
        gameObject.SetActive(true);

        BindingContext.OnShowStart();

        if (immediate)
        {
            //立即显示
            transform.localScale = Vector3.one;
            CanvasGroup.alpha = 1;
        }
        else
        {
            StartShowAnimation();
        }
    }

    protected virtual void OnShowFinish()
    {
        BindingContext.OnShowFinish();
        ShowedAction?.Invoke();
    }

    protected virtual void OnHideStart(bool immediate)
    {
        BindingContext.OnHideStart();

        if (immediate)
        {
            //立即隐藏
            transform.localScale = Vector3.zero;
            CanvasGroup.alpha = 0;
        }
        else
        {
            StartHideAnimation();
        }
    }

    protected virtual void OnHideFinish()
    {
        BindingContext.OnHideFinish();
        HiddenAction?.Invoke();
    }

    protected virtual void OnDestroy()
    {
        if (BindingContext.IsShowed)
        {
            Hide(true);
        }

        BindingContext.OnDestroy();
        BindingContext = null;
        ViewModelProperty.OnValueChanged = null;
    }

    protected virtual void StartShowAnimation()
    {
        CanvasGroup.interactable = false;
        transform.localScale = Vector3.one;
        CanvasGroup.DOFade(1, 0.2f).SetDelay(0.2f).OnComplete(() =>
        {
            CanvasGroup.interactable = true;
            OnShowFinish();
        });
    }

    protected virtual void StartHideAnimation()
    {
        CanvasGroup.interactable = false;
        CanvasGroup.DOFade(0, 0.2f).SetDelay(0.2f).OnComplete(() =>
        {
            transform.localScale = Vector3.zero;
            CanvasGroup.interactable = true;
            OnHideFinish();

            gameObject.SetActive(false);
        });
    }

    protected virtual void OnBindingContextChanged(T oldValue, T newValue)
    {
        Binder.UnBind(oldValue);
        Binder.Bind(newValue);
    }
}
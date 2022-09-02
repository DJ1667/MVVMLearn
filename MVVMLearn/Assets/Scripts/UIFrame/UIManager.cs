using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance = null;

    #region 组件

    private Canvas _mainCanvas;
    private RectTransform _rectTransform;

    #endregion

    #region 数据

    private const string DefaultViewPrefabPath = "Prefabs/UI/Panel/";

    private Dictionary<string, ViewInfoAttribute> _viewInfoDict = new Dictionary<string, ViewInfoAttribute>(); //View的信息
    private Dictionary<string, GameObject> _viewPrefabDict = new Dictionary<string, GameObject>(); //View的预制
    private Dictionary<string, Object> _viewDict = new Dictionary<string, Object>(); //View的实例
    private Dictionary<string, ViewModelBase> _viewModelDict = new Dictionary<string, ViewModelBase>(); //View对应的ViewModel

    private string _curView = null;
    private Stack<string> _viewStack = new Stack<string>();
    private Stack<string> _tempViewStack = new Stack<string>();

    public string CurView
    {
        get => _curView;
    }

    #endregion

    #region 初始化

    private void Awake()
    {
        Instance = this;
        _mainCanvas = transform.Find("Canvas").GetComponent<Canvas>();
        _rectTransform = _mainCanvas.GetComponent<RectTransform>();

        AnalysisAllViewInfo();

        Initialize();
    }

    private void AnalysisAllViewInfo()
    {
        var assembly = Assembly.GetAssembly(typeof(ViewInfoAttribute));
        Type[] types = assembly.GetTypes();

        foreach (var t in types)
        {
            var attr = t.GetCustomAttribute(typeof(ViewInfoAttribute));
            {
                var info = attr as ViewInfoAttribute;

                if (info != null && t.BaseType.Name == typeof(ViewBase<ViewModelBase>).Name)
                {
                    var tName = t.ToString();
                    info.SetName(tName, DefaultViewPrefabPath + tName);
                    _viewInfoDict.Add(tName, info);

                    LogEx.Log($"读取到{tName}: {info.Layer} {info.Path}");
                }
            }
        }
    }

    private void Initialize()
    {
        foreach (var layer in Enum.GetNames(typeof(UILayer)))
        {
            var layerRoot = new GameObject(layer);
            var rectTransform = layerRoot.AddComponent<RectTransform>();
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            layerRoot.transform.SetParent(_mainCanvas.transform, false);
        }
    }

    #endregion

    #region 界面操作

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="startAction">打开前回调</param>
    /// <param name="finishAction">打开后回调</param>
    /// <typeparam name="T"></typeparam>
    public void OpenView<T>(Action startAction, Action finishAction = null) where T : IView
    {
        OpenView<T>(false, startAction, finishAction);
    }

    /// <summary>
    /// 打开界面
    /// </summary>
    /// <param name="immediate">是否跳过动画立即打开</param>
    /// <param name="startAction">打开前回调</param>
    /// <param name="finishAction">打开后回调</param>
    /// <typeparam name="T"></typeparam>
    public void OpenView<T>(bool immediate = false, Action startAction = null, Action finishAction = null) where T : IView
    {
        var tName = typeof(T).ToString();

        var test = typeof(T);

        if (_viewInfoDict.ContainsKey(tName))
        {
            var viewInfo = _viewInfoDict[tName];

            if (_viewDict.ContainsKey(tName))
            {
                if (_viewDict[tName] is IView view)
                {
                    view.Show(immediate, startAction, finishAction);
                    view.SelfGameObject.transform.SetAsLastSibling();

                    _curView = tName;
                    _viewStack.Push(tName);
                }
            }
            else
            {
                CreateView<T>(viewInfo, immediate, startAction, finishAction);
            }
        }
        else
        {
            LogEx.LogError($"未注册{tName} 检查你的View是否继承ViewBase，是否添加ViewInfo属性");
        }
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="startAction">关闭前回调</param>
    /// <param name="finishAction">关闭后回调</param>
    /// <typeparam name="T"></typeparam>
    public void CloseView<T>(Action startAction, Action finishAction = null) where T : IView
    {
        CloseView<T>(false, startAction, finishAction);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="immediate">是否跳过动画立即关闭</param>
    /// <param name="startAction">关闭前回调</param>
    /// <param name="finishAction">关闭后回调</param>
    /// <typeparam name="T"></typeparam>
    public void CloseView<T>(bool immediate = false, Action startAction = null, Action finishAction = null) where T : IView
    {
        var tName = typeof(T).ToString();

        if (_viewDict.ContainsKey(tName))
        {
            if (_viewDict[tName] is IView view)
            {
                view.Hide(false, startAction, finishAction);
                view.BindingContext.Timer = 0;

                StackPopEx(tName);
                if (_viewStack.Count > 0)
                    _curView = _viewStack.Peek();
                else
                    _curView = "";
            }
        }
        else
        {
            LogEx.Log($"关闭失败 {tName} 尚未打开");
        }
    }

    /// <summary>
    /// 获取ViewModel
    /// </summary>
    /// <param name="type">View的type</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetViewModel<T>() where T : ViewModelBase
    {
        var typeName = typeof(T).ToString();
        if (_viewModelDict.ContainsKey(typeName))
        {
            return _viewModelDict[typeName] as T;
        }
        else
        {
            LogEx.LogError($"不存在此ViewModel 检查 {typeName} 是否声明 {typeof(T).ToString()}");
        }

        return null;
    }

    private void CreateView<T>(ViewInfoAttribute viewInfo, bool immediate = false, Action startAction = null, Action finishAction = null) where T : IView
    {
        GameObject prefab = null;
        if (_viewPrefabDict.ContainsKey(viewInfo.Name))
        {
            prefab = _viewPrefabDict[viewInfo.Name];
        }
        else
        {
            prefab = Resources.Load<GameObject>(viewInfo.Path);
            if (prefab == null)
            {
                LogEx.LogError($"不存在此预制{viewInfo.Name} {viewInfo.Path}");
                return;
            }

            _viewPrefabDict.Add(viewInfo.Name, prefab);
        }

        var viewObj = Instantiate(prefab);
        SetViewParent(viewObj, viewInfo.Layer);
        var view = viewObj.GetComponent<T>() as IView;
        if (view == null)
            LogEx.LogError($"无法获取 {typeof(T).ToString()} 脚本，检查是否挂载");
        view.Initialize();
        _viewDict.Add(viewInfo.Name, view);

        _viewModelDict.Add(view.BindingContext.GetType().ToString(), view.BindingContext);

        view.Show(immediate, startAction, finishAction);
        view.SelfGameObject.transform.SetAsLastSibling();

        _curView = viewInfo.Name;
        _viewStack.Push(viewInfo.Name);
    }

    private void SetViewParent(GameObject go, UILayer layer)
    {
        var parent = _mainCanvas.transform.Find(layer.ToString());
        go.transform.SetParent(parent, false);
    }

    private void DestroyView(string viewName)
    {
        var view = _viewDict[viewName] as IView;
        Destroy(view.SelfGameObject);

        _viewDict.Remove(viewName);
        _viewModelDict.Remove(view.BindingContext.GetType().ToString());
    }

    private void StackPopEx(string viewName)
    {
        _tempViewStack.Clear();
        while (_viewStack.Count > 0)
        {
            var tempName = _viewStack.Pop();
            if (tempName != viewName)
            {
                _tempViewStack.Push(tempName);
            }
            else
            {
                break;
            }
        }

        while (_tempViewStack.Count > 0)
        {
            _viewStack.Push(_tempViewStack.Pop());
        }
    }

    #endregion

    #region 生命周期

    private int _frame = 0;
    private int _frameMax = 2; //几帧更新一次

    private void Update()
    {
        _frame++;
        if (_frame >= _frameMax)
        {
            _frame = 0;

            var viewNameList = _viewDict.Keys.ToList();

            for (int i = 0; i < viewNameList.Count; i++)
            {
                var viewName = viewNameList[i];

                var viewModel = (_viewDict[viewName] as IView)?.BindingContext;
                if (viewModel.IsShowed || viewModel.IsShowInProgress) continue;

                viewModel.Timer += Time.deltaTime * _frameMax;

                if (viewModel.Timer > GetLifeTime(_viewInfoDict[viewName].Life))
                {
                    DestroyView(viewName);
                }
            }
        }
    }

    private int GetLifeTime(UILife life)
    {
        int time = -1;

        switch (life)
        {
            case UILife.S30:
                time = 3;
                break;
            case UILife.M1:
                time = 60;
                break;
            case UILife.M3:
                time = 180;
                break;
        }

        return time;
    }

    #endregion
}
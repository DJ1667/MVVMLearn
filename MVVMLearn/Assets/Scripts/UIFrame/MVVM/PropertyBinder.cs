using System;
using System.Collections.Generic;
using System.Reflection;

public class PropertyBinder<T> where T : ViewModelBase
{
    #region 优点：只需在初始化时绑定信息，不用关心什么时候移除监听，内部会自动处理。 缺点：只能绑定ViewModel中定义的响应式属性，无法对class类型的响应式属性内部的属性进行监听。

    private delegate void BindHandler(T viewModel);

    private delegate void UnBindHandler(T viewModel);

    private readonly List<BindHandler> _binders = new List<BindHandler>();
    private readonly List<UnBindHandler> _unBinders = new List<UnBindHandler>();

    public void Add<TProperty>(string name, BindableProperty<TProperty>.ValueChangedHandler valueChangedHandler)
    {
        var fieldInfo = typeof(T).GetField(name, BindingFlags.Instance | BindingFlags.Public);

        if (fieldInfo == null)
        {
            throw new Exception(string.Format("找不到需要绑定的属性 '{0}.{1}'", typeof(T).Name, name));
        }

        _binders.Add((viewModel) => { GetPropertyValue<TProperty>(name, viewModel, fieldInfo).OnValueChanged += valueChangedHandler; });

        _unBinders.Add((viewModel) => { GetPropertyValue<TProperty>(name, viewModel, fieldInfo).OnValueChanged -= valueChangedHandler; });
    }

    private BindableProperty<TProperty> GetPropertyValue<TProperty>(string name, T viewModel, FieldInfo fieldInfo)
    {
        var value = fieldInfo.GetValue(viewModel);

        BindableProperty<TProperty> bindableProperty = value as BindableProperty<TProperty>;
        if (bindableProperty == null)
        {
            throw new Exception(string.Format("非法的属性值 '{0}.{1}' ", typeof(T).Name, name));
        }

        return bindableProperty;
    }

    public void Bind(T viewModel)
    {
        if (viewModel != null)
        {
            foreach (var binder in _binders)
            {
                binder(viewModel);
            }
        }
    }

    public void UnBind(T viewModel)
    {
        if (viewModel != null)
        {
            foreach (var unBind in _unBinders)
            {
                unBind(viewModel);
            }
        }
    }

    #endregion


    #region 优点：可以精确到任何一个响应式属性。 缺点：需要自己手动管理添加和删除，进入界面添加，退出界面删除。

    public void Add<TProperty>(BindableProperty<TProperty> bp, BindableProperty<TProperty>.ValueChangedHandler valueChangedHandler)
    {
        bp.OnValueChanged += valueChangedHandler;
    }

    public void Remove<TProperty>(BindableProperty<TProperty> bp, BindableProperty<TProperty>.ValueChangedHandler valueChangedHandler)
    {
        bp.OnValueChanged -= valueChangedHandler;
    }

    #endregion
}
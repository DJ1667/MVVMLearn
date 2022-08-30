using System;
using System.Collections.Generic;
using System.Reflection;

public class PropertyBinder<T> where T : ViewModelBase
{
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
}
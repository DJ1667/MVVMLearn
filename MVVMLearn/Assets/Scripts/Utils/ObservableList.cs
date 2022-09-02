using System.Collections;
using System.Collections.Generic;

public class ObservableList<T> : IList<T>
{
    //直接改动Value时触发
    public delegate void ValueChangedHandler(List<T> oldValue, List<T> newValue);

    public ValueChangedHandler OnValueChanged;

    public delegate void AddHandler(T instance);

    public AddHandler OnAdd;

    public delegate void InsertHandler(int index, T instance);

    public InsertHandler OnInsert;

    public delegate void RemoveHandler(T instance);

    public RemoveHandler OnRemove;

    private List<T> _value = new List<T>();

    public List<T> Value
    {
        get { return _value; }
        set
        {
            if (!Equals(_value, value))
            {
                var old = _value;
                _value = value;

                ValueChanged(old, _value);
            }
        }
    }

    private void ValueChanged(List<T> oldValue, List<T> newValue)
    {
        if (OnValueChanged != null)
        {
            OnValueChanged(oldValue, newValue);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(T item)
    {
        _value.Add(item);
        OnAdd?.Invoke(item);
    }

    public void Clear()
    {
        _value.Clear();
    }

    public bool Contains(T item)
    {
        return _value.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _value.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        if (_value.Remove(item))
        {
            OnRemove?.Invoke(item);

            return true;
        }

        return false;
    }

    public int Count
    {
        get { return _value.Count; }
    }

    public bool IsReadOnly { get; private set; }

    public int IndexOf(T item)
    {
        return _value.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _value.Insert(index, item);
        OnInsert?.Invoke(index, item);
    }

    public void RemoveAt(int index)
    {
        var val = _value[index];
        _value.RemoveAt(index);
        OnRemove?.Invoke(val);
    }

    public T this[int index]
    {
        get => _value[index];
        set => _value[index] = value;
    }
}
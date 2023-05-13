using System.Collections.Generic;

public class LimitedList<T>
{
    private int _capacity;
    private List<T> _list;

    public LimitedList(int capacity)
    {
        _capacity = capacity;
        _list = new List<T>(capacity);
    }

    public void Add(T item)
    {
        if (_list.Count == _capacity)
        {
            _list.RemoveAt(0);
        }
        _list.Add(item);
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public T this[int index]
    {
        get { return _list[index]; }
        set { _list[index] = value; }
    }

    public int Count
    {
        get { return _list.Count; }
    }
}


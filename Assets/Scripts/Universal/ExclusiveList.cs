using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExclusiveList<T> : ICollection<T>, IEnumerable<T>, IList<T> where T : class
{
    [SerializeReference]
    public List<T> innerList = new List<T>();

    public T this[int index]
    {
        get => innerList[index];
        set
        {
            if (!innerList.Contains(value)) innerList[index] = value;
        }
    }

    public int Count => innerList.Count;
    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (!innerList.Contains(item)) innerList.Add(item);
        else throw new System.Exception("Item already exist.");
    }
    public void AddWithoutException(T item)
    {
        if (!innerList.Contains(item)) innerList.Add(item);
    }
    public void Clear()
    {
        innerList.Clear();
    }
    public bool Contains(T item)
    {
        return innerList.Contains(item);
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        innerList.CopyTo(array, arrayIndex);
    }
    public void CopyToList(ref List<T> list)
    {
        foreach (T t in this)
        {
            list.Add(t);
        }
    }
    public void CopyToXList(ref ExclusiveList<T> xList)
    {
        foreach (T t in this)
        {
            xList.AddWithoutException(t);
        }
    }
    public IEnumerator<T> GetEnumerator()
    {
        return innerList.GetEnumerator();
    }
    public int IndexOf(T item)
    {
        for (int i = 0; i < innerList.Count; i++)
        {
            if (innerList[i] == item)
            {
                return i;
            }
        }
        throw new System.Exception("No item found.");
    }
    public void Insert(int index, T item)
    {
        if (index >= innerList.Count) throw new System.Exception("Index out of range.");
        else if (index < 0) throw new System.Exception("Index can't be negative.");
        else
        {
            if (!innerList.Contains(item)) innerList[index] = item;
            else throw new System.Exception("Item already exist.");
        }
    }
    public void InsertWithoutException(int index, T item)
    {
        if (index >= 0 && index < Count && !innerList.Contains(item)) innerList[index] = item;
    }
    public bool Remove(T item)
    {
        if (innerList.Contains(item))
        {
            innerList.Remove(item);
            return true;
        }
        else return false;
    }
    public void RemoveAt(int index)
    {
        if (index >= innerList.Count) throw new System.Exception("Index out of range.");
        else if (index < 0) throw new System.Exception("Index can't be negative.");
        else innerList.RemoveAt(index);
    }
    public void RemoveAtWithoutException(int index)
    {
        if (index < Count && index >= 0) innerList.RemoveAt(index);
    }
    public List<T> ReturnList()
    {
        List<T> list = new List<T>();
        foreach (T t in this)
        {
            list.Add(t);
        }
        return list;
    }
    public System.Type GetListType()
    {
        return typeof(T);
    }
    public int GetTypeAmount(System.Type type)
    {
        int amount = 0;
        foreach (T t in innerList)
        {
            if (type == t.GetType()) amount++;
        }
        return amount;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return innerList.GetEnumerator();
    }
}

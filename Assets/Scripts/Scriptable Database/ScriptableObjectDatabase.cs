using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New ScriptableObjectDatabase", menuName = "Create ScriptableObject Database", order = 1)]
public class ScriptableObjectDatabase : SerializedScriptableObject
{
    [OnValueChanged("AddItemAndID")]

    public ScriptableObjectBase[] items;

    public Dictionary<int, ScriptableObjectBase> itemDatabase = new Dictionary<int, ScriptableObjectBase>();

    public void AddItemAndID()
    {
        itemDatabase.Clear();
        
        for (int i = 0; i < items.Length; i++)
        {
            if (!itemDatabase.TryGetValue(items[i].GetID(), out ScriptableObjectBase item))
                itemDatabase.Add(items[i].GetID(), items[i]);
        }
    }

    public T GetItemSOByID<T>(int id) where T : ScriptableObjectBase
    {
        if (itemDatabase.TryGetValue(id, out ScriptableObjectBase item)) return item as T;
        return null;
    }
}

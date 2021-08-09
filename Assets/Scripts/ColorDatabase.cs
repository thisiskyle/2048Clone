using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KE.Serialization;

[CreateAssetMenu(fileName = "NewColorDB", menuName = "ScriptableObjects/Color Database", order = 1)]
public class ColorDatabase : ScriptableObject
{
    [System.Serializable]
    private class StringAndColorDict : SerializableDictionary<int, Color>{}
    [SerializeField]
    private StringAndColorDict list = new StringAndColorDict();

    public Color Query(int q)
    {
        return list[q];
    }
}

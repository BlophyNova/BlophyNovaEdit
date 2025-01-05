using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyValueList<Key, Value>/*:IEnumerable,IEnumerator<KeyValueList<Key,Value>>*/
{
    public List<Key> Keys = new();
    public List<Value> Values = new();
    public int Count => Keys.Count;

    //public KeyValueList<Key, Value> Current => throw new NotImplementedException();

    //object IEnumerator.Current => throw new NotImplementedException();

    public Key this[int index]
    {
        get { return Keys[index]; }
    }
    public void Add(Key key,Value value)
    {
        Keys.Add(key); 
        Values.Add(value);  
    }
    public void RemoveAt(int index)
    {
        Keys.RemoveAt(index); 
        Values.RemoveAt(index);
    }
    public void Insert(int index,Key key,Value value)
    {
        Keys.Insert(index,key);
        Values.Insert(index,value);
    }
    public Key GetKey(int index)
    {
        return Keys[index];
    }
    public Value GetValue(int index) 
    {
        return Values[index];
    }
    public KeyValuePair<Key, Value> GetPair(int index) 
    {
        return new(Keys[index], Values[index]);
    }

    //IEnumerator IEnumerable.GetEnumerator()
    //{

    //}

    //public bool MoveNext()
    //{
    //    throw new NotImplementedException();
    //}

    //public void Reset()
    //{
    //    throw new NotImplementedException();
    //}

    //public void Dispose()
    //{
    //    throw new NotImplementedException();
    //}
}

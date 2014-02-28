using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using Pathfinding.Serialization.JsonFx;

[Serializable]
public abstract class StoreModel : INotifyPropertyChanged
{
    public event EventHandler<EventArgs> AfterReset;

    public event PropertyChangedEventHandler PropertyChanged;

    private readonly Store _nativeStore;

    protected string ToJSON<T>(T obj)
    {
        try
        {
            var json = JsonWriter.Serialize(obj);
            return json;
        }
        catch (JsonSerializationException exception)
        {
            Debug.LogError(string.Format("Exceptions was raised during serialization:\r\n{0}", exception.Message));
            throw exception;
        }                
    }

    protected T FromJSON<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default(T);

        try
        {            
            var instanceT = JsonReader.Deserialize<T>(json);
            return instanceT;
        }
        catch (JsonDeserializationException exception)
        {
            Debug.LogError(string.Format("Exceptions was raised during deserialization:\r\n{0}", exception.Message));
            return default(T);
        }      
    }

    protected StoreModel(Store store)
    {
        _nativeStore = store;
    }

    public void Flush(Store store=null)
    {
        this.OnFlush(store ?? _nativeStore);        
    }

    public void Default()
    {
        this.OnDefault();        
        this.OnAfterReset();
        this.OnPropertyChanged(string.Empty);
    }

    protected void OnAfterReset()
    {
        if (AfterReset != null)
        {
            AfterReset(this, EventArgs.Empty);
        }
    }

    protected void OnPropertyChanged(string name)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }

    protected abstract void OnFlush(Store store);

    protected abstract void OnDefault();
}

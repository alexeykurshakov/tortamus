using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class WorldSettings : StoreModel
{
    private const string kStoreKey = "WorldSettings";

    private float _gravity;
    public float Gravity
    {
        get { return _gravity; }
        set
        {
            _gravity = value;
            this.OnPropertyChanged("Gravity");
        }
    }

    private float _angularDrag;
    public float AngularDrag
    {
        get { return _angularDrag; }
        set
        {
            _angularDrag = value;
            this.OnPropertyChanged("AngularDrag");
        }
    }

    public WorldSettings(Store store) : base(store)
    {
        var worldInStore = FromJSON<WorldSettings>(store.GetValue(kStoreKey));
        if (worldInStore == null)
        {
            this.OnDefault();
            return;
        }
        _gravity = worldInStore._gravity;
        _angularDrag = worldInStore._angularDrag;
    }

    protected override void OnDefault()
    {
        _gravity = -1f;
        _angularDrag = 0.15f;
    }

    protected override void OnFlush(Store store)
    {
        store.SetValue(kStoreKey, ToJSON(this));            
    }
}

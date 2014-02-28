using System;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;

[Serializable]
public class Player : StoreModel
{
    private const string kStoreKey = "Player";

    private int _currentLevel;
    public int CurrentLevel
    {
        get { return _currentLevel; }
        set
        {
            _currentLevel = value;
            this.OnPropertyChanged("CurrentLevel");
        }
    }

    public Player(Store store) : base(store)
    {
        var playerInStore = FromJSON<Player>(store.GetValue(kStoreKey));
        if (playerInStore == null)
        {
            this.OnDefault();
            return;
        }
        _currentLevel = playerInStore._currentLevel;
    }

    protected override void OnDefault()
    {
        _currentLevel = 0;        
    }

    protected override void OnFlush(Store store)
    {
        store.SetValue(kStoreKey, ToJSON(this));        
    }
}

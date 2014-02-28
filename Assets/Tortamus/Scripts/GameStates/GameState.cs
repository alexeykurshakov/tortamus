using System;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;
using System.Collections;

public abstract class GameState : MonoBehaviour
{
    public abstract GameStates State { get; }

    public void Start(Store store)
    {        
    }



    //public static event EventHandler<EventArgs> Started;

    //public static event EventHandler<EventArgs> Paused;

    //public static event EventHandler<EventArgs> Completed;

    //private bool _isPaused;
    //public bool IsPaused
    //{
    //    get { return _isPaused; }
    //    set
    //    {
    //        if (value == _isPaused)
    //            return;

    //        _isPaused = value;            
    //        OnPause(value);            
    //    }
    //}

    //private bool _isCompleted;
    //public bool IsCompleted
    //{
    //    get { return _isCompleted; }
    //    protected set
    //    {
    //        _isCompleted = value;
    //        if (_isCompleted)
    //            OnComplete();
    //    }
    //}

    //public void Reset()
    //{        
    //    this.OnReset();
    //}

    //protected virtual void OnStart()
    //{
    //    IsPaused = false;
    //    IsCompleted = false;
    //    if (Started != null)
    //    {
    //        Started(this, EventArgs.Empty);
    //    }
    //}

    //protected virtual void OnComplete()
    //{
    //    if (Completed != null)
    //    {
    //        Completed(this, EventArgs.Empty);
    //    }
    //}

    //protected virtual void OnPause(bool pause)
    //{
    //    if (Paused != null)
    //    {
    //        Paused(this, EventArgs.Empty);
    //    }
    //}

    //protected virtual void OnReset()
    //{        
    //    OnStart();
    //}
}
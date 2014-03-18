using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;
using System.Collections;

public class Level1 : GameState
{
    public override GameStates State { get { return GameStates.Level1;} }

	[SerializeField] private ParticleSystem _leftPS;

	[SerializeField] private ParticleSystem _rihgPS;

    [SerializeField] private Gear _mainGear;	   

	[SerializeField] private InstrumentScale _instrumentScale;

    [SerializeField] private Inventory _inventory;

    [SerializeField] private RingDisk _ringDisk;

	[SerializeField] private PushButton _pushButton;

    [SerializeField] private HandModel _handModel;    

	private enum States
	{
		WaitForInput,
		DiskRotate,
		UseInventory,
		Cinematic,
		OnStart = WaitForInput,
	}

	private States _currentState = States.OnStart;
	private States CurrentState
	{
		get { return _currentState; }
		set
		{
			_currentState = value;
		}
	}
	   
    private void Update()
    {			
		var delta = Time.deltaTime;
        _instrumentScale.Speed0_100 = _ringDisk.Speed0_100;
        
		if (_instrumentScale.Speed0_100 > 10 || _currentState == States.DiskRotate)
		{
			var speedScaleCoeff = (_instrumentScale.Speed0_100/100f);
			var emitRate = 20 + speedScaleCoeff * 1200;
			_leftPS.emissionRate = emitRate;
			_rihgPS.emissionRate = emitRate;

			_leftPS.startSpeed = speedScaleCoeff * 1f;
			_rihgPS.startSpeed = speedScaleCoeff * 1f;

			if (!_leftPS.enableEmission)
			{
				_leftPS.enableEmission = true;
				_rihgPS.enableEmission = true;
				if (_leftPS.isStopped)
				{
					_leftPS.Play();
					_rihgPS.Play();
				}					
			}		
		}
		else
		{
			_leftPS.enableEmission = false;
			_rihgPS.enableEmission = false;
		}

        switch (_currentState)
        {
            case States.WaitForInput:
                WaitForInputProcess();
                break;

        	case States.DiskRotate:		
            	break;

            case States.UseInventory:
                InventoryUseProcess(delta);
                break;

            case States.Cinematic:
                CinematicProcess(delta);
                return;
        }
		
        _mainGear.AngularSpeed = _ringDisk.AngularSpeed / 4f;	
		if (_ringDisk.IsReadyForBoost)
		{
            _pushButton.IsActive = true;		
		}			
    }

    private float _cinematicTimer = 0f;
    private void CinematicProcess(float delta)
    {
        _cinematicTimer += delta;
        if (_cinematicTimer > 2f)
        {
            var stateManager = StateManager.Instance;
            if (!stateManager.IsBusy)
			{
				_mainGear.HideLinks();
                stateManager.SwitchState(GameStates.Level2);
			}
        }
    }

	private void WaitForInputProcess()
	{	    
        if (!_handModel.IsPressed)
	        return;
        RaycastHit hit;

        if (!Physics.Raycast(_handModel.GetRay(), out hit, 
		                      Mathf.Infinity, 1 << LayerMask.NameToLayer("UserInteractive")))
            return;					        

		var hitObject = hit.collider.gameObject;
		if (hitObject == this._ringDisk.gameObject)
		{
            this._ringDisk.SetHand(_handModel);			
			CurrentState = States.DiskRotate;
			return;
		}

        if (_handModel.IsRetained)
	        return;    

		if (hitObject == this._pushButton.gameObject)
		{
			if (_pushButton.IsActive)
			{			
				CurrentState = States.Cinematic;
                _ringDisk.Boost();
                _mainGear.TransferSpin();			    
				_pushButton.IsPressed = true;
			}
			return;
		}

        if (_inventory.Touch(hitObject, _handModel))
		    CurrentState = States.UseInventory;	
	}

	private void InventoryUseProcess(float delta)
	{
	    if (!_inventory.Process(delta))
	    {
            CurrentState = States.WaitForInput;
	    }		
	}

    private void Start()
    {
        _ringDisk.HandReleased += (sender, args) => this.CurrentState = States.WaitForInput;	

		_inventory.AllBalls.ForEach(b => b.IsEnabled = true);	

		_leftPS.enableEmission = false;
		_rihgPS.enableEmission = false;
   	}
}
 
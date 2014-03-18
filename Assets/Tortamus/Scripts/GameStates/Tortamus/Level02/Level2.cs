using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using UnityEngine;
using System.Collections;

public class Level2 : GameState
{
    public override GameStates State { get { return GameStates.Level2;} }

	[SerializeField] private GameObject _ballSelectEffect;

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
		if (StateManager.Instance.IsBusy)
			return;

		var delta = Time.deltaTime;
        _instrumentScale.Speed0_100 = _ringDisk.Speed0_100;

		if (_instrumentScale.Speed0_100 > 10 || _currentState == States.DiskRotate)
		{
			var speedScaleCoeff = (_instrumentScale.Speed0_100/100f);
			var emitRate = 20 + speedScaleCoeff * 400;
			_leftPS.emissionRate = emitRate;
			_rihgPS.emissionRate = emitRate;

			_leftPS.startSpeed = speedScaleCoeff * 2f;
			_rihgPS.startSpeed = speedScaleCoeff * 2f;

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
				DiskRotateProcess(delta);
            	break;

            case States.UseInventory:
                InventoryUseProcess(delta);
                break;

            case States.Cinematic:
                return;
        }
		
        _mainGear.AngularSpeed = _ringDisk.AngularSpeed / 4f;	
		if (_ringDisk.IsReadyForBoost)
		{
            _pushButton.IsActive = true;		
		}			
    }

	private float _rotateAngles = 0f;
	private void DiskRotateProcess(float delta)
	{
		_rotateAngles += Mathf.Abs(_ringDisk.AngularSpeed * delta);
		if (_rotateAngles > 3.14 * 3)
		{
			var allBalls = _inventory.AllBalls;
			if (allBalls.Any(b => b.IsEnabled))
				return;
			
			var restDisabledBalls = _inventory.AllBalls.FindAll(b => !b.IsEnabled);
			var index = UnityEngine.Random.Range(0, restDisabledBalls.Count);
			restDisabledBalls[index].IsEnabled = true;
			restDisabledBalls[index].DoCastling(_ballSelectEffect);
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
		_ringDisk.HandReleased += (sender, args) => 
		{
			_rotateAngles = 0f;
			this.CurrentState = States.WaitForInput;
		};
				
		_ballSelectEffect.SetActive(false);
		_inventory.BallPlugged += (object sender, EventArgs e) => 
		{
			var ball = (Ball)sender;
		    ball.IsEnabled = false;
			ball.UndoCastling();
		};

		_leftPS.enableEmission = false;
		_rihgPS.enableEmission = false;
   	}
}
 
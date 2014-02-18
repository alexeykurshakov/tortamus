using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using UnityEngine;
using System.Collections;

public class GameTortamus : MonoBehaviour
{  
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

	private States _state = States.OnStart;
	private States State
	{
		get { return _state; }
		set
		{
			_state = value;
		}
	}
	   
    private void Update()
    {		
		var delta = Time.deltaTime;
        _instrumentScale.Speed0_100 = _ringDisk.Speed0_100;
		_instrumentScale.Temp0_100 = _ringDisk.Temp0_100;

        switch (_state)
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
                return;
        }
		
        _mainGear.AngularSpeed = _ringDisk.AngularSpeed / 4f;
		if (_ringDisk.IsReadyForBoost)
		{
            _pushButton.IsActive = true;		
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
			State = States.DiskRotate;
			return;
		}

        if (_handModel.IsRetained)
	        return;    

		if (hitObject == this._pushButton.gameObject)
		{
			if (_pushButton.IsActive)
			{			
				State = States.Cinematic;
                _ringDisk.Boost();
                _mainGear.TransferSpin();			    
			}
			return;
		}

        if (_inventory.Touch(hitObject, _handModel))
		    State = States.UseInventory;	
	}

	private void InventoryUseProcess(float delta)
	{
	    if (!_inventory.Process(delta))
	    {
            State = States.WaitForInput;
	    }		
	}

    private void Start()
    {
        _ringDisk.HandReleased += (sender, args) => this.State = States.WaitForInput;
    }
}
 
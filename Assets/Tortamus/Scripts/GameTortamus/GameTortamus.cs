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

	private enum States
	{
		WaitForInput,
		OrbitRotate,
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
		_instrumentScale.Value0_100 = (int)(100f * (Math.Abs(_ringDisk.AngularSpeed) / RingDisk.kMaxSpeed));

		switch (_state)
		{
		case States.WaitForInput:
			WaitForInputProcess();
			break;

		case States.OrbitRotate:
			OrbitRotateProcess();
			break;

		case States.UseInventory:
			InventoryUseProcess(delta);
			break;

		case States.Cinematic:
		    return;
		}
		
        _mainGear.AngularSpeed = _ringDisk.AngularSpeed / 4f;
		_pushButton.IsActive = _ringDisk.IsReadyForBoost;        
    }

	private void WaitForInputProcess()
	{
		System.Object rayRef = InputHelper.GetTouchBeganRay();
		if (rayRef == null)
			return;	

		RaycastHit hit;
		if (!Physics.Raycast ((Ray)rayRef, out hit, 
		                      Mathf.Infinity, 1 << LayerMask.NameToLayer("UserInteractive")))
			return;
			

		var hitObject = hit.collider.gameObject;
		if (hitObject == this._ringDisk.gameObject)
		{
			this._ringDisk.SetHand(new Vector3(
				hit.point.x,
				hit.point.y,
				hit.point.z
			));            
			State = States.OrbitRotate;
			return;
		}	

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

        if (_inventory.Touch(hitObject))
		    State = States.UseInventory;	
	}

	private void OrbitRotateProcess()
	{
		if (Input.GetMouseButtonUp(0))
		{
			State = States.WaitForInput;
			_ringDisk.RemoveHand();
			return;
		}

		RaycastHit hit;
		if (!Physics.Raycast (InputHelper.GetTouchRay(), out hit, 
		                      Mathf.Infinity, 1 << LayerMask.NameToLayer("UserInteractive")))
		{            
			_ringDisk.RemoveHand(false);
			return;
		}

		var hitObject = hit.collider.gameObject;
		if (hitObject != this._ringDisk.gameObject)
		{         
            _ringDisk.RemoveHand(false);
			return;
		}

        this._ringDisk.SetHand(new Vector3(
			hit.point.x,
			hit.point.y,
			hit.point.z
		));
	}

	private void InventoryUseProcess(float delta)
	{
	    if (!_inventory.Process(delta))
	    {
            State = States.WaitForInput;
	    }		
	}
}
 
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameTortamus : MonoBehaviour
{  
    [SerializeField]
    private Gear _mainGear;	   

	[SerializeField]
	private Gear _gear;

    [SerializeField]
    private RingDisk _ringDisk;

	[SerializeField]
	private TurboButton _turboButton;

    [SerializeField]
    private InternalDisk _internalDisk;

	private Ball _currentDragBall;

	private const float kGearTurboMinLimitSpeed = 1.5f;

	private const float kTimeForTurboActivate = 2f;

	private float _turboTimer;

	private float _forceForTorque = 1f;

	private enum States
	{
		WaitForInput,
		RingCall,
		BallDrag,
		TurboOn,
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

		if (_state != States.TurboOn)
		{
			_mainGear.AngularSpeed = _ringDisk.AngularSpeed/4f;
			var absGearSpeed = Mathf.Abs(_mainGear.AngularSpeed);
			if (!_turboButton.IsActive)
			{
				if (absGearSpeed > kGearTurboMinLimitSpeed)
				{
					if ( (_turboTimer += delta) > kTimeForTurboActivate)			
						_turboButton.IsActive = true;
				}
				else if (_turboTimer > 0)
				{
					_turboTimer -= delta;
				}
			}
			else if (absGearSpeed < kGearTurboMinLimitSpeed * 0.75f)
			{
				_turboButton.IsActive = false;
				_turboTimer = 0f;
			}
		}

		switch (_state)
		{
		case States.WaitForInput:
			WaitForInputProcess(delta);
			break;

		case States.RingCall:
			RingCallProcess(delta);
			break;

		case States.BallDrag:
			BallDragProcess(delta);
			break;

		case States.TurboOn:
			_gear.AngularSpeed = -_mainGear.AngularSpeed;
			break;
		}
    }

	private System.Object GetTouchBeganRay()
	{
		System.Object rayRef = null;
		if (Input.touchCount > 0)
		{
			foreach (Touch touch in Input.touches)
			{
				if (touch.phase == TouchPhase.Began)
				{
					rayRef = Camera.main.ScreenPointToRay(touch.position);
					break;
				}
			}
		}
		else if (Input.GetMouseButtonDown(0))
		{
			rayRef = Camera.main.ScreenPointToRay(Input.mousePosition);
		}
		return rayRef;
	}

	private Ray GetTouchRay()
	{
		if (Input.touchCount < 0)
			return Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

		return Camera.main.ScreenPointToRay(Input.mousePosition);
	}

	private void WaitForInputProcess(float delta)
	{
		System.Object rayRef = GetTouchBeganRay();
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
			), _forceForTorque);		
			State = States.RingCall;
			return;
		}	

		if (hitObject == this._turboButton.gameObject)
		{
			if (_turboButton.IsActive)
			{
				_gear.transform.Rotate(0,0,-_mainGear.transform.eulerAngles.z);
				_gear.gameObject.SetActive(true);
				State = States.TurboOn;
			}
			return;
		}	

		var ball = hitObject.GetComponent<Ball>();
		if (ball.IsPlugged)
			return;

		_currentDragBall = ball;
		State = States.BallDrag;	
	}

	private void RingCallProcess(float delta)
	{
		if (Input.GetMouseButtonUp(0))
		{
			State = States.WaitForInput;
			_ringDisk.RemoveHand();
			return;
		}

		RaycastHit hit;
		if (!Physics.Raycast (GetTouchRay(), out hit, 
		                      Mathf.Infinity, 1 << LayerMask.NameToLayer("UserInteractive")))
		{
			_ringDisk.RemoveHand();
			return;
		}

		var hitObject = hit.collider.gameObject;
		if (hitObject != this._ringDisk.gameObject)
		{
			_ringDisk.RemoveHand();
			return;
		}

		this._ringDisk.MoveHand(new Vector3(
			hit.point.x,
			hit.point.y,
			hit.point.z
		));
	}

	private void BallDragProcess(float delta)
	{
		var ray = GetTouchRay();
		if (Input.GetMouseButtonUp(0))
		{
			RaycastHit hit;
			if (!Physics.Raycast (ray, out hit, 
			                     Mathf.Infinity, 1 << LayerMask.NameToLayer("Outlets")))
			{
				_currentDragBall.Throw();
			}
			else
			{
				var outlet = hit.collider.gameObject.GetComponent<BallOutlet>();
				if (outlet.IsFree)
				{
					_currentDragBall.PlugIn(outlet);
					_forceForTorque = (_forceForTorque > 1 ? _forceForTorque : 0) + _currentDragBall.Weight * 5;
				}
				else
				{
					_currentDragBall.Throw();
				}
			}				
			State = States.WaitForInput;
			_currentDragBall = null;
			return;
		}
		_currentDragBall.Drag(ray.origin);
	}
}
 
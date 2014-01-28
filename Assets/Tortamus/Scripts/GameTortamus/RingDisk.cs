using UnityEngine;
using System.Collections;

public class RingDisk : MonoBehaviour 
{
	private MeshCollider _meshCollider;

	private BoxCollider _boxCollider;

	private ConstantForce _force;

	private Rigidbody _rigidBody;

	private Vector3 _handPosPrev;

	private float _prevHandVelocity;

	private Vector3 _handPos;

	private float _timer;

	private bool _isHandSet;		

	private float _forceCoeff;

	public float AngularSpeed
	{
		get { return this._rigidBody.angularVelocity.z; }
	}

	private float GetHandForce()
	{
		if (Vector3.Distance(_handPos, _handPosPrev) < 0.009f)
		{
			if (Mathf.Abs(_rigidBody.angularVelocity.z) < 0.001f)
				return 0;


			return _rigidBody.angularVelocity.z > 0 ? -5 : 5;						
		}

		var v1 = this.transform.position - _handPos;
		var v2 = this.transform.position - _handPosPrev;

		var angle = Vector3.Angle(v1, v2);
		var radius = (v1.magnitude + v2.magnitude)/2f;

		var isCounterClockwise = Vector3.Cross(v1, v2).z > 0;
		if (isCounterClockwise)
			angle *= -1;

		if (Mathf.Abs(_prevHandVelocity) < 0.0001f)
		{
			_prevHandVelocity = angle/_timer;
			return 0;
		}
		else
		{
			var handVelocity = angle/_timer;
			var force = handVelocity * 0.005f;
			force += ((handVelocity - _prevHandVelocity)/_timer) * 0.00005f;
			_prevHandVelocity = handVelocity;
			return force * _forceCoeff;
		}
	}

	public void SetHand(Vector3 handPos, float force)
	{
		_handPos = new Vector3(handPos.x, handPos.y, this.transform.position.z);
		_handPosPrev = _handPos;
		_isHandSet = true;

		_timer = 0f;
		_prevHandVelocity = 0f;
		_boxCollider.enabled = true;
		_meshCollider.enabled = false;

		_forceCoeff = force;
	}

	public void MoveHand(Vector3 handPos)
	{
		_handPos = new Vector3(handPos.x, handPos.y, this.transform.position.z);
	}

	public void RemoveHand()
	{
		_boxCollider.enabled = false;
		_meshCollider.enabled = true;
		_isHandSet = false;
	}

	private void Update () 
	{
		_timer += Time.deltaTime;
		if (!_isHandSet)
		{
			_force.torque = new Vector3();
			return;
		}
			
		_force.torque = new Vector3(0, 0, GetHandForce() * (this.transform.position - _handPos).magnitude);
		_handPosPrev = _handPos;
		_timer = 0f;
	}

	private void Start () 
	{
		_force = this.GetComponent<ConstantForce>();
		_rigidBody = this.GetComponent<Rigidbody>();
		_meshCollider = this.GetComponent<MeshCollider>();
		_boxCollider = this.GetComponent<BoxCollider>();
		_boxCollider.enabled = false;
	}
}

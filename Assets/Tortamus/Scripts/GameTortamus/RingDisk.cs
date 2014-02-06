using UnityEngine;
using System.Collections;

public class RingDisk : MonoBehaviour 
{
    [SerializeField] private bool _isBoostEnable;

    public bool IsReadyForBoost
    {
        get
        {
            if (this._isBoostEnable)
            {
				return Mathf.Abs(this.AngularSpeed) >= kMaxSpeed;
            }
            return false;
        }
    }

	public const float kMaxSpeed = 6f;

	private MeshCollider _meshCollider;

	private BoxCollider _boxCollider;

	private ConstantForce _force;

	private Rigidbody _rigidBody;

	private Vector3 _handPosPrev;

	private float _prevHandVelocity;

	private float _prevDiskVelocity;

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

			return _rigidBody.angularVelocity.z > 0 ? -3.5f : 3.5f;
		}

		var v1 = this.transform.position - _handPos;
		var v2 = this.transform.position - _handPosPrev;

		var angle = Vector3.Angle(v1, v2);
		var radius = (v1.magnitude + v2.magnitude)/2f;

		var isCounterClockwise = Vector3.Cross(v1, v2).z > 0;
		if (isCounterClockwise)
			angle *= -1;

		angle /= 180f;
		{
			var handVelocity = angle/_timer;
			var diskVelocity = this.AngularSpeed;
			var absDiff = Mathf.Abs(diskVelocity - handVelocity);
			var forceCoeffVel = 0f;
			var sameDirection = false;
			if ((handVelocity < 0 && diskVelocity > 0) || (handVelocity > 0 && diskVelocity < 0))
			{
				forceCoeffVel = 2 + absDiff/3;
			}
			else
			{
				sameDirection = true;
				forceCoeffVel = (3.5f/(Mathf.Abs (diskVelocity)+1)) / (absDiff+0.01f);
			}				

			if (Mathf.Abs (_prevDiskVelocity) > 0.000001f)
			{
				var absDiskDiff = Mathf.Abs(_prevDiskVelocity - diskVelocity);			
				if (sameDirection && absDiskDiff < 0.07f && Mathf.Abs(handVelocity) > Mathf.Abs(diskVelocity))
				{
					_forceCoeff += 0.4f;
				}
				else if (_forceCoeff > 0)
				{
					_forceCoeff -= 0.4f;
				}			
			}

			_prevHandVelocity = handVelocity;
			_prevDiskVelocity = diskVelocity;
			return (handVelocity * (forceCoeffVel + _forceCoeff)) * (1 + (sameDirection ? diskVelocity * Inventory.BallUsesCount/6f: 0));
		}
	}

	public void SetHand(Vector3 handPos)
	{
		_handPos = new Vector3(handPos.x, handPos.y, this.transform.position.z);
		_handPosPrev = _handPos;
		_isHandSet = true;

		_timer = 0f;
		_prevHandVelocity = 0f;
		_prevDiskVelocity = 0f;
		_forceCoeff = 0f;
		_boxCollider.enabled = true;
		_meshCollider.enabled = false;
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

    public void Boost()
    {        
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

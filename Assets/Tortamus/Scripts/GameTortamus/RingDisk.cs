using Helpers;
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
	
    private float _timer;

	private float _prevHandVelocity;

	private float _prevDiskVelocity;

    private Vector3 _handPosPrev;

	private Vector3 _handPos;

	private bool _isHandSet;			

	private bool _isAccelartionCouldApply;

	private float _torqueAcc;

	public float AngularSpeed
	{
		get { return this._rigidBody.angularVelocity.z; }
	}

	private float PassiveDeceleration()
	{
		// 1. If speed of the disk is low, return 0
		if (_rigidBody.angularVelocity.z.IsEqual(0, 0.001f))
			return 0;

		// 2. Otherwise return opposite sign to angular speed force
		var res = 4.5f + Inventory.Instance.PluggedBalls.Count * 0.5f;
		return res * (_rigidBody.angularVelocity.z > 0 ? -1 : 1);
	}
	
	private float CalcForceForTorque()
	{
		// If we just keep our hand on the disk, but don't move
		if (Vector3.Distance(_handPos, _handPosPrev) < 0.0025f)
		{
			// Debug.Log ("PassiveDeceleration");
			return PassiveDeceleration();
		}

		var v1 = this.transform.position - _handPos;
		var v2 = this.transform.position - _handPosPrev;

		var angle = Vector3.Angle(v1, v2)/ 180f;

		var isCounterClockwise = Vector3.Cross(v1, v2).z > 0;
		if (isCounterClockwise)
			angle *= -1;	

		// Count of balls that plugged into disk
		var pluggedCount = Inventory.Instance.PluggedBalls.Count;

		// The angular speed of our hand
		var handVelocity = angle/_timer;

		// The angular speed of the disk
		var diskVelocity = this.AngularSpeed;

		// Is hand rotate for the same the direction as the disk
		var isSameDirection = diskVelocity.IsEqual(0) || handVelocity.IsSameSign(diskVelocity);	

		// Difference in angular speed between hand and disk
		var speedDiff = Mathf.Abs(diskVelocity - handVelocity);	

		// Current accelartion
		var accelaration = (handVelocity - _prevHandVelocity)/_timer;
		_prevHandVelocity = handVelocity;
				
		if (isSameDirection) 
		{
			// Increase speed of the disk
			var step = 0.3f * Mathf.Sign(handVelocity);	
			var diskAccelartion = Mathf.Abs (diskVelocity - _prevDiskVelocity)/_timer;
			var etalonAccelartion = 0.8f * kMaxSpeed/(20-pluggedCount);

			if (diskAccelartion < etalonAccelartion)
		    {
		        _torqueAcc += step;
		    }
		    else
		    {			
		        _torqueAcc -= step/4;
		        if (!_torqueAcc.IsSameSign(handVelocity))
		        {
		            _torqueAcc = 0;
		        }
		    }

			if (_isAccelartionCouldApply)
			{
				if (accelaration.IsSameSign(handVelocity))
				{
					_torqueAcc += (0.25f * Mathf.Sign(handVelocity)) * (Mathf.Abs (diskVelocity)/2f+5+pluggedCount/2f);
				}
				else
				{
					_isAccelartionCouldApply = false;
				}
			}
		}
		else
		{
			_torqueAcc = 0;
			return handVelocity * (1 + pluggedCount * 0.15f)*(2 + Mathf.Abs(diskVelocity - handVelocity)/3);
		}
			
		return _torqueAcc;
	}

	public void SetHand(Vector3 handPos)
	{
        _handPos = new Vector3(handPos.x, handPos.y, this.transform.position.z);
	    if (_isHandSet)
            return;

		var pluggedBalls = Inventory.Instance.PluggedBalls;
		pluggedBalls.ForEach(b => b.Weight = 0f);
		_rigidBody.angularDrag = 0.15f - pluggedBalls.Count * 0.01f;
		
        _isHandSet = true;
	    if (_boxCollider.enabled)
	        return;    

		_isAccelartionCouldApply = true;
		_handPosPrev = _handPos;							
		_boxCollider.enabled = true;
		_meshCollider.enabled = false;
	}

	public void RemoveHand(bool release = true)
	{
	    _force.torque = new Vector3();
		_torqueAcc = 0f;
        _isHandSet = false;

		Inventory.Instance.PluggedBalls.ForEach(b => b.Weight = 0.3f);
	    if (!release)
	        return;

        _timer = 0f;
        _prevHandVelocity = 0f;

        _boxCollider.enabled = false;
        _meshCollider.enabled = true;			            
	}
	  
	private void Update () 
	{		
		_timer = Time.deltaTime;
		if (!_isHandSet)
		{			
            _prevDiskVelocity = this.AngularSpeed;
			return;
		}
		
		_force.torque = new Vector3(0, 0, CalcForceForTorque() * (this.transform.position - _handPos).magnitude);
		_handPosPrev = _handPos;

        _prevDiskVelocity = this.AngularSpeed;
	    _timer = 0f;
	}

	public void Boost()
	{        
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

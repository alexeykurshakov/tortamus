using System;
using Helpers;
using UnityEngine;
using System.Collections;

public class RingDisk : MonoBehaviour
{
    public event EventHandler<EventArgs> HandReleased;

    private float _maxTemperature;

    private float _temperature;

	private bool _isBoost;

    public bool IsReadyForBoost
    {
        get { return _temperature >= _maxTemperature; }
    }
 
    public float MaxSpeed { get; private set; }    

    public int Speed0_100
    {
        get { return (int)(100f * (Math.Abs(this.AngularSpeed) / this.MaxSpeed)); }
    }

    public int Temp0_100
    {
        get { return (int)(100f * (this._temperature) / this._maxTemperature); }
    }

	private MeshCollider _meshCollider;

	private BoxCollider _boxCollider;

	private ConstantForce _force;

	private Rigidbody _rigidBody;

    private HandModel _handModel;

    private float _delta;

    private float _torqueEnergy;

    private float _torqueEnergyExtra; 

    private Vector3 _handPosPrev;

    private Vector3 _handPos;	          	

	public float AngularSpeed
	{
		get { return this._rigidBody.angularVelocity.z; }
	}

	private void PassiveDeceleration()
	{
		// 1. If speed of the disk is low, return 0
		if (_rigidBody.angularVelocity.z.IsEqual(0, 0.001f))
		{
			_torqueEnergy = 0f;
			return;
		}

		// 2. Otherwise return opposite sign to angular speed force		
		_torqueEnergy = GameConfig.Instance.Коэф_Трения_Покоя * (_rigidBody.angularVelocity.z > 0 ? -1 : 1);
	}

	private const float kVelocityConvertCoeff = 360f/Mathf.PI;
	
	private void CalcForceForTorque()
	{
	    var gameConfig = GameConfig.Instance;	    

		// If we just keep our hand on the disk, but don't move
	    if (Vector3.Distance(_handPos, _handPosPrev) < 0.0025f)
	    {
	        PassiveDeceleration();
	        return;
	    }

	    var v1 = this.transform.position - _handPos;
		var v2 = this.transform.position - _handPosPrev;

		var angle = Vector3.Angle(v1, v2);	    

		var isCounterClockwise = Vector3.Cross(v1, v2).z > 0;
	    if (isCounterClockwise)
            angle *= -1;	    

	    // The angular speed of our hand
		var handVelocity = angle/(_delta * kVelocityConvertCoeff);

		// The angular speed of the disk
		var diskVelocity = this.AngularSpeed;

		// Velocity diff
		var diffVelocity = handVelocity - diskVelocity;
		if (handVelocity.IsSameSign(diskVelocity) && !handVelocity.IsSameSign(diffVelocity))
		{
			_torqueEnergy = 0;
		}
		else
		{
			var count  = Inventory.Instance.PluggedBalls.Count;
			_torqueEnergy = Mathf.Sign(diffVelocity) * _handModel.PressureCoeff * diffVelocity * diffVelocity / 2f;
			if (count > 0)
			{
				_torqueEnergy *= (1+count*gameConfig.Усил_Коэф_Трения);
			}
		}				
	}

    private void OnHandRelease()
    {
        if (HandReleased != null)
        {
            HandReleased(this, EventArgs.Empty);
        }
        _handModel = null;
        _force.torque = new Vector3();
		_boxCollider.enabled = false; 
		_meshCollider.enabled = true;
    }

    private bool Raycast(out Vector3 vec3)
    {
        RaycastHit hit;
        if (Physics.Raycast(_handModel.GetRay(), out hit,
            Mathf.Infinity, 1 << LayerMask.NameToLayer("UserInteractive")))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                vec3 = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                return true;
            }
        }
        vec3 = new Vector3();
        return false;
    }

    public void SetHand(HandModel handModel)
    {        
        _handModel = handModel;
        if (!Raycast(out _handPos))
        {
            OnHandRelease();
            return;
        }	     

        _handPosPrev = _handPos;
        _boxCollider.enabled = true;
        _meshCollider.enabled = false;
        _torqueEnergy = 0f;
        _torqueEnergyExtra = _handModel.Momentum;
    }

    private void FixedUpdate()
    {
        var fixedDelta = Time.fixedDeltaTime;
		OnTemperatureProcess(fixedDelta);

        if (_handModel == null)
			return;

        if (_delta.IsEqual(0))
            return;     

		var torqueZ = _torqueEnergy * (fixedDelta / _delta);
		_force.torque = new Vector3(0, 0, torqueZ);                
    }

	private void OnTemperatureProcess(float delta)
	{
		if (_isBoost)
			return;

		var timeF = GameConfig.Instance.Время_Нагрева;
		if (timeF < 9)
			timeF = 9;

		var absSpeed = Mathf.Abs(this.AngularSpeed);
		if (_handModel != null)
		{
			var time = timeF - Inventory.Instance.PluggedBalls.Count;
			var coef = 1/(MaxSpeed*time);
			_temperature += absSpeed * coef * delta;
		}

		var coef2 = 1/(timeF * 2.5f);
		_temperature -= coef2 * delta;
		if (_temperature < 0)
			_temperature = 0;	
	}
	  
	private void Update ()
	{		
		//this.gameObject.renderer.material.color;

	   	if (_handModel == null)
	        return;
       
		if (_handModel.IsPressed && Raycast(out _handPos))
        {
			_delta = Time.deltaTime;
			CalcForceForTorque();    
			_handPosPrev = _handPos;
        }
		else
		{
			OnHandRelease();		
		}	   	
	}

	public void Boost()
	{        
		_isBoost = true;
	}

    private void OnConfigChanged(object sender, EventArgs args)
    {
        var config = (GameConfig) sender;

        this.MaxSpeed = 0.8f * config.Макс_Угл_Скорость;        
        this._rigidBody.mass = config.Масса_Диска.MassCorrect();
		this._maxTemperature = 1f;
		ChangeAngularDrag(config.Коэф_Трения_Качения);			
    }	   

	private void ChangeAngularDrag(float angularDrag)
	{
		var pluggedCount = Inventory.Instance.PluggedBalls.Count;
		this._rigidBody.angularDrag = angularDrag * (1 - 0.067f * pluggedCount);
	}

	private void Start () 
	{
		_force = this.GetComponent<ConstantForce>();
		_rigidBody = this.GetComponent<Rigidbody>();
		_meshCollider = this.GetComponent<MeshCollider>();
		_boxCollider = this.GetComponent<BoxCollider>();
		_boxCollider.enabled = false;      

		GameConfig.Changed += OnConfigChanged;
		OnConfigChanged(GameConfig.Instance, EventArgs.Empty);

		Inventory.Instance.BallPlugged += (obj, args) =>
		{
			ChangeAngularDrag(GameConfig.Instance.Коэф_Трения_Качения);
		};
	}
}

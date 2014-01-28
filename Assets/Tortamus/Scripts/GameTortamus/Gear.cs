using UnityEngine;
using System.Collections;

public class Gear : MonoBehaviour 
{
	private Rigidbody _rigidBody;

	public float AngularSpeed
	{
		get
		{
			return _rigidBody.angularVelocity.z;
		}
		set
		{
			_rigidBody.angularVelocity = new Vector3(0,0,value);
		}
	}

	private void Start()
	{
		_rigidBody = this.GetComponent<Rigidbody>();	
	}
}

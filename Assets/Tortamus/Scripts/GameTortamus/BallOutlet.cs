using UnityEngine;
using System.Collections;

public class BallOutlet : MonoBehaviour
{
	private Rigidbody _rigidBody;

	[SerializeField]
	private Transform _originalPlace;

	public bool IsFree
	{
		get
		{
			return this.Weight < 0.05f;
		}
	}

	public Vector3 Scale
	{
		get { return _originalPlace.localScale; }
	}

	public Vector3 Position
	{
		get { return this._originalPlace.position; }
	}

	public float Weight
	{
		get { return this._rigidBody.mass; }
		set 
		{
			this._rigidBody.mass = value;
		}
	}

	private void Start()
	{
		_rigidBody = this.GetComponent<Rigidbody>();
	}
}

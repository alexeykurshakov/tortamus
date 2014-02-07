using UnityEngine;
using Helpers;
using System.Collections;

public class BallOutlet : MonoBehaviour
{
	private Rigidbody _rigidBody;

	[SerializeField]
	private Transform _originalPlace;

	public bool IsFree
	{
		get { return this.Weight.IsEqual(0); }
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
			if (value.IsEqual(0))
			{
				this._rigidBody.mass = 0.000001f;
				return;
			}
			this._rigidBody.mass = value;
		}
	}

	private void Start()
	{
		_rigidBody = this.GetComponent<Rigidbody>();
	}
}

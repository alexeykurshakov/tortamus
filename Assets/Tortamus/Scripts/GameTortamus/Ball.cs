using System;
using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
	[SerializeField] private float _weight;   
	public float Weight { get { return _weight;} }

	private Vector3 _originalPlace;

	private Vector3 _prevDragPos;

	private BallOutlet _outlet;

	private bool _isThrown;

	private void Start()
	{
		_originalPlace = this.transform.localPosition;
	}

    public bool IsPlugged { private set; get; }

    public void PlugIn(BallOutlet outlet)
    {                
		if (this._outlet)
			throw new InvalidOperationException("Ball is yet plugIn");

		this.GetComponent<SphereCollider>().enabled = false;
		_outlet = outlet;
		_outlet.Weight = _weight;        
		this.transform.localScale = _outlet.Scale;
		this.transform.position = _outlet.Position;		
    }

	public void PlugOff()
	{
		this._outlet.Weight = 0f;
		this._outlet = null;
		Throw();
	}

	public void Throw()
	{
		this._isThrown = true;
		this.GetComponent<SphereCollider>().enabled = true;
	}

	private void Update()
	{
		if (this._outlet)
		{
			this.transform.position = _outlet.Position;						
			return;
		}

		if (this._isThrown)
		{
			this._isThrown = false;
			this.transform.localPosition = _originalPlace;
			_prevDragPos = new Vector3();
		}
	}

	public void Drag(Vector3 vec)
	{
		if (_prevDragPos.magnitude != 0)
			this.transform.position += (vec - _prevDragPos) * 32f;

		_prevDragPos = vec;
	}
}

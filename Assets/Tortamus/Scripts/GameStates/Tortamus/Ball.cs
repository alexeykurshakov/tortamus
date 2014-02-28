using System;
using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    private bool _isDragged;
    private float _dragTime;

	private float _weight;   
	public float Weight 
	{ 
		get { return _weight;} 
		set 
		{ 
			_weight = value; 
			if (this._outlet != null)
				this._outlet.Weight = _weight;
		}
	}

	private Vector3 _originalPlace;

	private Vector3 _originalScale;

	private Vector3 _prevDragPos;

	private BallOutlet _outlet;

	private bool _isThrown;

    private void Awake()
    {      
		OnConfigChanged(GameConfig.Instance, EventArgs.Empty);
        GameConfig.Changed += OnConfigChanged;        
    }

    private void OnConfigChanged(object sender, EventArgs args)
    {
        var config = (GameConfig)sender;
        this.Weight = config.Масса_Шара;
    }

	private void Start()
	{        
		if (this._originalPlace.magnitude == 0)
		{
			_originalPlace = this.transform.localPosition;
			_originalScale = this.transform.localScale;
		}
	}

    public bool IsPlugged 
	{ 
		get { return _outlet != null; }
	}

    public void PlugIn(BallOutlet outlet)
    {                
		if (this._outlet != null)
			throw new InvalidOperationException("Ball is yet plugIn");

		this.GetComponent<SphereCollider>().enabled = false;
		_outlet = outlet;
		_outlet.Weight = _weight;        
		this._isDragged = false;
		this.transform.localScale = _outlet.Scale;
		this.transform.position = _outlet.Position;		
    }

	public void PlugOff()
	{
		this._outlet.Weight = 0f;
		this._outlet = null;
		this.GetComponent<SphereCollider>().enabled = true;
		Throw();
	}

	public void Throw(System.Object newOriginalPlace = null)
	{
        if (newOriginalPlace != null)
			_originalPlace = (Vector3)newOriginalPlace;

		this._isThrown = true;
	    this._isDragged = false;
	    this._dragTime = 0f;
	}

	private void Update()
	{
	    if (_isDragged)
	    {
            _dragTime += Time.deltaTime;
	        var scale = 1 + 0.05f * Mathf.Sin(6 * _dragTime);
			this.transform.localScale = scale * _originalScale;
	    }	

		if (this._outlet)
		{
			this.transform.position = _outlet.Position;						
			return;
		}

		if (this._isThrown)
		{
			this._isThrown = false;
			this.transform.localPosition = _originalPlace;
			this.transform.localScale = _originalScale;
			_prevDragPos = new Vector3();
		}
	}

	public void Drag(Vector3 vec)
	{
	    _isDragged = true;

		if (_prevDragPos.magnitude != 0)
			this.transform.position += (vec - _prevDragPos) * 32f;

		_prevDragPos = vec;
	}
}

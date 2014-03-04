using System;
using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
	public bool IsEnabled { get; set; }

	private float _dragTime;

    private bool _isDragged;
	public bool IsDragged 
	{
		get { return _isDragged; }
		set
		{
			if (_isDragged == value)
				return;

			_isDragged = value;
			if (_isDragged)
			{
				SoundManager.Instance.GetSound(SoundManager.Sounds.BallTap).Play();
			}
			else
			{
				this._dragTime = 0f;
			}
		}
	}

	private GameObject _castlingAlternate;

	public bool IsCastling 
	{ 
		get { return _castlingAlternate != null; }
	}

	private BallOutlet _outlet;

	public bool IsPlugged 
	{ 
		get { return _outlet != null; }
	}

	private int _color;
	public int TemperatureColor
	{
		get {return _color; }
		set
		{
			_color = value;
			var colorF = _color/100f;
			var red = 189 + 66f * colorF;
			var green = 189 - 61f * colorF;
			var blue = 189 - 61f * colorF;
			renderer.material.SetColor("_Color", new Color(red/255f, green/255f, blue/255f));
		}
	}

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

	private bool _isThrown;

    private void Awake()
    {      
		OnConfigChanged(GameConfig.Instance, EventArgs.Empty);
        GameConfig.Changed += OnConfigChanged;        
    }

	private void SetPosition(Vector3 position)
	{
		this.transform.position = position;
		if (IsCastling)
		{
			_castlingAlternate.transform.position = position;
		}
	}

	private void SetScale(Vector3 scale)
	{
		this.transform.localScale = scale;
		if (IsCastling)
		{
			_castlingAlternate.transform.localScale = scale;
		}
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
			_originalPlace = this.transform.position;
			_originalScale = this.transform.localScale;
		}
	}

    public void PlugIn(BallOutlet outlet)
    {                
		if (this._outlet != null)
			throw new InvalidOperationException("Ball is yet plugIn");

		SoundManager.Instance.GetSound(SoundManager.Sounds.BallPlugin).Play();

		this.GetComponent<SphereCollider>().enabled = false;
		_outlet = outlet;
		_outlet.Weight = _weight;        
		this.IsDragged = false;

		SetPosition(_outlet.Position);
		SetScale(_outlet.Scale);	
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
	    this.IsDragged = false;	    
	}

	public void DoCastling(GameObject castlingAlternate)
	{
		_castlingAlternate = castlingAlternate;

		_castlingAlternate.transform.position = this.transform.position;
		_castlingAlternate.transform.localScale = this.transform.localScale;

		_castlingAlternate.SetActive(true);
		this.renderer.enabled = false;
	}

	public void UndoCastling()
	{
		if (_castlingAlternate == null)
			return;

		this.transform.position = _castlingAlternate.transform.position;
		this.transform.localScale = _castlingAlternate.transform.localScale;

		_castlingAlternate.SetActive(false);
		this.renderer.enabled = true;

		_castlingAlternate = null;
	}

	private void Update()
	{			
	    if (IsDragged && !IsCastling)
	    {
            _dragTime += Time.deltaTime;
	        var scale = 1 + 0.05f * Mathf.Sin(6 * _dragTime);
			this.transform.localScale = scale * _originalScale;
	    }	

		if (this._outlet)
		{				
			SetPosition(_outlet.Position);
			return;
		}

		if (this._isThrown)
		{
			this._isThrown = false;

			SetPosition(_originalPlace);
			SetScale(_originalScale);
			_prevDragPos = new Vector3();
		}
	}

	public void Drag(Vector3 vec)
	{
	    IsDragged = true;

		var currentTransform = IsCastling ? _castlingAlternate.transform : this.transform;

		if (_prevDragPos.magnitude != 0)
			currentTransform.position += (vec - _prevDragPos) * 32f;

		_prevDragPos = vec;
	}
}

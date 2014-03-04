using System;
using System.Configuration;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gear : MonoBehaviour 
{			
	[SerializeField] private Gear[] _connectedGears = new Gear[5];

	private Rigidbody _rigidBody;	  

	private readonly List<Gear> _links = new List<Gear>();

    public void TransferSpin()
    {        
        _links.ForEach(g =>
        {
            g.TransferSpin();
			g.transform.Rotate(0,0,-this.transform.eulerAngles.z);
            g.gameObject.SetActive(true);
        });
    }

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

    private void Update()
    {
        foreach (var link in _links)
        {		
            link.AngularSpeed = -this.AngularSpeed;
        }
    }

	public void Init()
	{
		if (this._rigidBody != null)
			return;

		_rigidBody = this.GetComponent<Rigidbody>();
		foreach (var connectedGear in _connectedGears)
		{
			if (connectedGear != null)
			{
				connectedGear.Init();
				_links.Add(connectedGear);
			}
		}
	}

	private void Start()
	{
		Init();	
	}
}

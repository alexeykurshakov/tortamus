using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System;

public class Inventory : MonoBehaviour 
{
    public static Inventory Instance { private set; get; }

	public event EventHandler<EventArgs> BallPlugged;

    private readonly List<Ball> _balls = new List<Ball>(); 

    private Ball _currentDragBall;

    private HandModel _handModel;

    public List<Ball> PluggedBalls
    {
        get { return _balls.FindAll (b => b.IsPlugged); }
    }

	private void Awake()
	{
		Instance = this;
	}

    private void Start()
    {     
        foreach (Transform child in transform)
        {
			var ball = child.gameObject.GetComponent<Ball>();
			if (ball == null)			
				continue;

            _balls.Add(ball);         
        }	
   	}

    public bool Touch(GameObject hitObject, HandModel handModel)
    {
        _handModel = handModel;
        var ball = hitObject.GetComponent<Ball>();
        if (ball.IsPlugged)
            return false;

        _currentDragBall = ball;
        return true;
    }

	private void OnBallPlugged()
	{
		if (BallPlugged != null)
		{
			BallPlugged(this, EventArgs.Empty);
		}
	}

    public bool Process(float delta)
    {
        if (!_handModel.IsPressed)
        {
            RaycastHit hit;
            if (!Physics.Raycast(_handModel.GetRay(), out hit,
                                 Mathf.Infinity, 1 << LayerMask.NameToLayer("Outlets")))
            {
                _currentDragBall.Throw();
            }
            else
            {
                var outlet = hit.collider.gameObject.GetComponent<BallOutlet>();
                if (outlet.IsFree)
                {
                    _currentDragBall.PlugIn(outlet);
					OnBallPlugged();
                }
                else
                {
                    _currentDragBall.Throw();
                }
            }
            _currentDragBall = null;
            return false;
        }
        _currentDragBall.Drag(_handModel.GetRay().origin);
        return true;            
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class Inventory : MonoBehaviour 
{
    private readonly List<Ball> _balls = new List<Ball>(); 

    private Ball _currentDragBall;

	private Vector3 _place1 = new Vector3(-0.5653334f, 1.039437f, -75.01415f);

	private Vector3 _place2 = new Vector3(-0.7272438f, 1.039437f, -75.01415f);

	public static int BallUsesCount;

    private void Start()
    {
        foreach (Transform child in transform)
        {
            _balls.Add(child.gameObject.GetComponent<Ball>());
            if (child.localPosition.x < -0.8f)
			{
				child.gameObject.SetActive(false);
			}
        }	
   	}

    public bool Touch(GameObject hitObject)
    {
        var ball = hitObject.GetComponent<Ball>();
        if (ball.IsPlugged)
            return false;

        _currentDragBall = ball;
        return true;
    }

    public bool Process(float delta)
    {
        var ray = InputHelper.GetTouchRay();
        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit,
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
					BallUsesCount++;
                    ShiftBalls();
                }
                else
                {
                    _currentDragBall.Throw();
                }
            }            
            _currentDragBall = null;
            return false;
        }
        _currentDragBall.Drag(ray.origin);
        return true;
    }

    private void ShiftBalls()
    {
        var notPluggedBalls = _balls.FindAll(b => !b.IsPlugged);
        if (notPluggedBalls.Any())
        {
            var firstNoActiveBall = notPluggedBalls.Find(b => !b.gameObject.activeSelf);		
            if (firstNoActiveBall != null)
            {			
				firstNoActiveBall.gameObject.SetActive(true);
				firstNoActiveBall.Throw(_place2);             

				var yetActiveBall = notPluggedBalls.Find(b => b.gameObject.activeSelf);
				if (yetActiveBall !=null && yetActiveBall.transform.localPosition.x < -0.6f)
				{
					yetActiveBall.Throw(_place1);
				}
            }           
        }
    }
}

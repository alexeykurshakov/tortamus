using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class InputHelper
{
    public static System.Object GetTouchBeganRay()
    {
        System.Object rayRef = null;
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    rayRef = Camera.main.ScreenPointToRay(touch.position);
                    break;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            rayRef = Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        return rayRef;
    }

    public static Ray GetTouchRay()
    {
        if (Input.touchCount < 0)
            return Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}


﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HandModel : MonoBehaviour
{   
    public Vector3 Speed { get; private set; }

    public Vector3 Accelaration { get; private set; }

    public Vector3 CurrentPos { get; private set; }

    public Vector3 Direction { get; private set; }

    public float Momentum { get; private set; }

    public float PressureCoeff = 1f;

    public bool IsPressed { get; private set; }

    public bool IsRetained { get; private set; }

    public Ray GetRay()
    {
        return _rayRef != null ? (Ray) _rayRef : new Ray();
    }

    private System.Object _rayRef;
   
    private void Update()
    {
        var delta = Time.deltaTime;
        if (IsPressed)
        {            
            _rayRef = Camera.main.ScreenPointToRay(Input.mousePosition);
            var newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Direction = newPos - CurrentPos;

            var newSpeed = Direction/delta;
            Accelaration = (newSpeed - Speed)/delta;

            var angleBetween = Vector3.Angle(Speed, newSpeed);
            if (angleBetween > 5)
                this.Momentum = 0;
            
            CurrentPos = newPos;
            Speed = newSpeed;
            
            this.Momentum = this.PressureCoeff*Speed.sqrMagnitude/2f;

            if (Input.GetMouseButtonUp(0))
            {
                IsPressed = false;
                IsRetained = false;
            }
            else
            {
                IsRetained = true;
            }
        }
        else
        {         
            if (Input.GetMouseButtonDown(0))
            {                                
                _rayRef = Camera.main.ScreenPointToRay(Input.mousePosition);
                CurrentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);                
                ResetInputState();
                IsPressed = true;
            }            
        }
    }

    private void ResetInputState()
    {
        IsRetained = false;
        IsPressed = false;

        Momentum = 0f;
        Speed = new Vector3();
        Accelaration = new Vector3();       
        Direction = new Vector3();
    }   
}

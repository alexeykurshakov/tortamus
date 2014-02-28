using UnityEngine;
using System.Collections;

public class InstrumentScale : MonoBehaviour 
{
	[SerializeField] private Transform _speedArrow;
	[SerializeField] private Transform _tempArrow;

	private int _speed;
	public int Speed0_100
	{
		get { return _speed; }
		set
		{
			_speed = value > 100 ? 100 : value < 0 ? 0 : value;
			_speedArrow.eulerAngles = new Vector3(0,0,-3.6f*_speed);
		}
	}


	private int _temp;
	public int Temp0_100
	{
		get { return _temp; }
		set
		{
			_temp = value > 100 ? 100 : value < 0 ? 0 : value;
			_tempArrow.eulerAngles = new Vector3(0,0,-3.6f*_temp);
		}
	}
}

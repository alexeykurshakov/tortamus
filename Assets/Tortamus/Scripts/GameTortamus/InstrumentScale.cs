using UnityEngine;
using System.Collections;

public class InstrumentScale : MonoBehaviour 
{
	[SerializeField]
	private Transform _rulerArrow;

	private int _value;
	public int Value0_100
	{
		get { return _value; }
		set
		{
			_value = value > 100 ? 100 : value < 0 ? 0 : value;
			_rulerArrow.eulerAngles = new Vector3(0,0,-3.6f*_value);
		}
	}
}

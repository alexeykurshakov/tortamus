using UnityEngine;
using System.Collections;

public class PushButton : MonoBehaviour 
{
	[SerializeField]
	private Transform _outletsContainer;

	private Color _inactiveColor = new Color(0.8f, 0.8f, 0.8f);

	private bool _isActive;
	public bool IsActive
	{
		get { return _isActive; }
		set
		{
			this.gameObject.renderer.material.color = value ? Color.red : _inactiveColor;
			_isActive = value;
		}
	}

	private void Start()
	{
		this.gameObject.renderer.material.color = _inactiveColor;
	}
}

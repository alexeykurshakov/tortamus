using UnityEngine;
using System.Collections;

public class PushButton : MonoBehaviour 
{
	[SerializeField] private Transform _outletsContainer;

	private const float kZmax = -0.003072725f;
	private const float kZmin = -0.0022f;

	private readonly Color _inactiveMainColor = new Color(214f/255f, 214f/255f, 214f/255f);
	private readonly Color _inactiveSpecColor = new Color(0f, 0f, 0f);

	private readonly Color _activeMainColor = new Color(204f/255f, 15f/255f, 15f/255f);
	private readonly Color _activeSpecColor = new Color(159f/255f, 88f/255f, 107f/255f);

	private bool _isActive;
	public bool IsActive
	{
		get { return _isActive; }
		set
		{
			if (value == _isActive)
				return;
			_isActive = value;

			if (_isActive)
			{
				renderer.material.SetColor("_SpecColor", _activeSpecColor);
				renderer.material.SetColor("_Color", _activeMainColor);
				this.gameObject.SetActive(true);
				this.transform.localScale = new Vector3(0, 1, 0);
			}

		}
	}

	private float _pressedTimer;
	
	public bool IsPressed;

	private void Update()
	{
		if (IsActive && _pressedTimer >= 0)
		{
			if (this.transform.localScale.x < 1)
			{
				var scale = this.transform.localScale.x;
				scale += Time.deltaTime * 2.5f;
				if (scale > 1)
				{
					scale = 1;
				}
				this.transform.localScale = new Vector3(scale, 1, scale);
			}
			else
			{
				var position = this.transform.localPosition;
				_pressedTimer += Time.deltaTime;
				if (_pressedTimer >= 1.5f)
				{
					this.transform.localPosition = new Vector3(position.x, position.y, kZmax);
					_pressedTimer = -1f;
				}
				else
				{
					this.transform.localPosition = new Vector3(position.x, position.y, kZmin + kZmax * (1 + Mathf.Sin (24 * _pressedTimer)));
				}
			}
		}
	}

	private void Start()
	{
		IsActive = false;
	}
}

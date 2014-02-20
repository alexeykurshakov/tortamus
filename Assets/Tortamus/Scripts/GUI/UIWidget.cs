using UnityEngine;
using System;
using System.Collections;

public class UIWidget : MonoBehaviour 
{
	[SerializeField] protected GUISkin Skin;

	protected float ScaleFactor;
	
	protected bool IsShown {get; set;}

	private const float kBaseDPI = 160f;

	private void OnGUI()
	{
		GUI.skin = Skin;

		var scaledMatrix = Matrix4x4.identity * Matrix4x4.Scale(new Vector3(ScaleFactor, ScaleFactor, ScaleFactor));        
		GUI.matrix = scaledMatrix;

		GUIDraw ();
	}

	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			var dpi = Screen.dpi;
			dpi = dpi == 0 ? kBaseDPI : dpi;
			var extraScreenScaleFactor = Screen.width/960f;
			ScaleFactor = (extraScreenScaleFactor < 1 ? 1 : extraScreenScaleFactor) * dpi/kBaseDPI; 
		}
		else
		{
			ScaleFactor = 1f;
		}
	}
	
	protected virtual void GUIDraw()
	{
	}
}

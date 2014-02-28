using System;
using System.Reflection;
using UnityEngine;

public class UIProfile : UIWidget
{
	[SerializeField] private GUISkin _skin2;
	
	private bool IsShown;

	protected override void GUIDraw ()
	{
		if (!IsShown)
			return;

		// Lets just quickly set up some GUI layout variables
		const float panelWidth = 200f;
		const float panelHeight = 300f;	
		
		// Draw the box
		GUILayout.BeginArea(new Rect(10, 60, panelWidth, panelHeight));
		GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical();
		GUILayout.BeginArea(new Rect(20, 25, panelWidth - 40, panelHeight - 60), GUI.skin.customStyles[0]);
		
		var cFile = GameConfig.Instance;
		
		for (var i=0; i<5;++i)
		{
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button(string.Format("Профиль {0}", i+1), cFile.GetActiveProfile() == i ? _skin2.button : Skin.button))
			{
				cFile.SetActiveProfile(i);
				IsShown = false;
			}
			
			GUILayout.EndHorizontal();
		}

		GUILayout.EndArea();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	public void Show()
	{
		IsShown = true;
	}

	public void Hide()
	{
		IsShown = false;
	}
}

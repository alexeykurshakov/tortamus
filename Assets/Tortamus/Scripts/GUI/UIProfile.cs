using System;
using System.Reflection;
using UnityEngine;

public class UIProfile : MonoBehaviour
{
	[SerializeField] private GUISkin _skin;
	[SerializeField] private GUISkin _skin2;
	
	private bool _isShown;
	
	private void OnGUI()
	{
		GUI.skin = _skin;		
		if (_isShown)
		{
			DrawGUI();		
		}
	}

	public void Show()
	{
		_isShown = true;
	}
	
	private void DrawGUI()
	{
		// Lets just quickly set up some GUI layout variables
		const float panelWidth = 200f;
		const float panelHeight = 300f;
		var panelPosX = Screen.width / 2f - panelWidth / 2f;
		var panelPosY = Screen.height / 2f - panelHeight / 2f;
		
		// Draw the box
		GUILayout.BeginArea(new Rect(10, 60, panelWidth, panelHeight));
		GUILayout.Box(string.Empty, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical();
		GUILayout.BeginArea(new Rect(20, 25, panelWidth - 40, panelHeight - 60), GUI.skin.customStyles[0]);

		var cFile = GameConfig.Instance;

		for (var i=0; i<5;++i)
		{
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button(string.Format("Профиль {0}", i+1), cFile.GetActiveProfile() == i ? _skin2.button : _skin.button))
			{
				cFile.SetActiveProfile(i);
				_isShown = false;
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndArea();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}

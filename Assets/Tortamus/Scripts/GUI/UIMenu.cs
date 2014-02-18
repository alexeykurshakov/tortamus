using System;
using System.Reflection;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
	[SerializeField] private GUISkin _skin;

	[SerializeField] private UIProfile _uiProfile;

	private bool _isShown;
	
	private void OnGUI()
	{
		GUI.skin = _skin;

		if (GUI.Button(new Rect(10, 35, 100, 20), "Меню"))
		{
			_isShown = true;
		}

		if (_isShown)
		{
			DrawGUI();		
		}
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
		
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Рестарт"))
		{
			_isShown = false;
			Application.LoadLevel(Application.loadedLevel);
		}

		GUILayout.EndHorizontal();
				
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Профиль"))
		{
			_isShown = false;
			_uiProfile.Show ();
		}
		
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("По умолчанию"))
		{
			_isShown = false;
			GameConfig.Default();
		}
		
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Отмена"))
		{
			_isShown = false;
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Выход"))
		{
			_isShown = false;
			Application.Quit();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}

using System;
using System.Reflection;
using UnityEngine;

public class UIMenu : UIWidget
{
	[SerializeField] private UIProfile _uiProfile;

	protected override void GUIDraw ()
	{
		if (GUI.Button(new Rect(10, 35, 100, 20), "Меню"))
		{
			IsShown = !IsShown;
			_uiProfile.Hide ();
		}
		
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
		
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Рестарт"))
		{
			IsShown = false;
			Application.LoadLevel(Application.loadedLevel);
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Профиль"))
		{
			IsShown = false;
			_uiProfile.Show();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("По умолчанию"))
		{
			IsShown = false;
			GameConfig.Default();
		}
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("Отмена"))
		{
			IsShown = false;
		}
		
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.BeginVertical();
		GUILayout.Space(20);
		
		if (GUILayout.Button("Выход"))
		{
			IsShown = false;
			Application.Quit();
		}

		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
		GUILayout.EndVertical();
		GUILayout.EndArea();		
	}
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Globalization;

public class UISettings : MonoBehaviour
{
    [SerializeField]
    private GUISkin _skin;

    private FieldInfo _needKeyCode;

    private bool _isShown;

    private readonly Dictionary<string, string> _overrideValues = new Dictionary<string, string>(); 

    private void OnGUI()
    {
        GUI.skin = _skin;

		if (GUI.Button(new Rect(10, 10, 100, 20), "Настройки"))
		{
			_isShown = true;
		}

		if (_isShown)
		{
			DrawGUI();        
        }		    
   	}    

	private void Awake()
	{
		GameConfig.Changed += (sender, e) => 
		{
			_overrideValues.Clear ();
		};
	}

    private void DrawGUI()
    {
        // Lets just quickly set up some GUI layout variables
        const float panelWidth = 400f;
        const float panelHeight = 300f;
        var panelPosX = Screen.width / 2f - panelWidth / 2f;
        var panelPosY = Screen.height / 2f - panelHeight / 2f;

        // Draw the box
        GUILayout.BeginArea(new Rect(panelPosX, panelPosY, panelWidth, panelHeight));
        GUILayout.Box("Настройки", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect(20, 25, panelWidth - 40, panelHeight - 60), GUI.skin.customStyles[0]);

        // Lets show login box!
        GUILayout.FlexibleSpace();

        var cFile = GameConfig.Instance;

        Type type = cFile.GetType();
        PropertyInfo[] properties = type.GetProperties();
        FieldInfo[] fields = type.GetFields();

        foreach (FieldInfo field in fields)
        {
            if (Type.GetTypeCode(field.FieldType) == TypeCode.Single)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(field.Name);

                float fValue;
                if (!_overrideValues.ContainsKey(field.Name))
                {
                    fValue = (float)field.GetValue(cFile);
                    _overrideValues.Add(field.Name, fValue.ToString());
                }

                var currentValue = GUILayout.TextField(_overrideValues[field.Name], 25, GUILayout.MinWidth(100));
                var textValue = new StringBuilder(currentValue);
                textValue.Replace(',', '.');
				if (textValue.Length == 0 || textValue[textValue.Length - 1] == '.')
				{
					textValue.Append('0');																            
				}                
				else if (textValue[0] == '.')
				{
					textValue.Insert(0, '0');	
				}
				if (float.TryParse(textValue.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out fValue))
                {
					field.SetValue(cFile, fValue);
                    _overrideValues[field.Name] = currentValue;
                }				
				GUILayout.EndHorizontal();
            }
            else if (field.GetValue(cFile) is KeyCode)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(field.Name);

                if (_needKeyCode == field)
                {
                    GUILayout.Button("Нажми клавишу");
                    Event ev = Event.current;
                    if (ev.isKey)
                    {
                        field.SetValue(cFile, ev.keyCode);
                        _needKeyCode = null;
                    }
                }
                else
                {
                    var kCode = (KeyCode)field.GetValue(cFile);
                    string keyName = Enum.GetName(kCode.GetType(), kCode);
                    if (GUILayout.Button(keyName))
                    {
                        _needKeyCode = field;
                    }
                }


                GUILayout.EndHorizontal();
            }
        }

        GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

        if (GUILayout.Button("Сохранить"))
        {
            GameConfig.Save();
            _isShown = false;
        }
		if (GUILayout.Button("Отмена"))
		{
			GameConfig.Load(true);
			_isShown = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.FlexibleSpace();

        GUILayout.EndArea();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
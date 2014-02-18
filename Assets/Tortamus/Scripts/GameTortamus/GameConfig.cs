using UnityEngine;
using System;
using System.Collections;
using Pathfinding.Serialization.JsonFx;

public class GameConfig
{
	public static event EventHandler<EventArgs> Changed;

    const string kPlayerPrefsName = "settings";    

	private static int _currentProfile;

    public float Масса_Диска = 5f;
    public float Масса_Шара = 1.5f;

    public float Коэф_Трения = 1f;
	public float Усил_Коэф_Трения = 0.1f;
	public float Коэф_Трения_Покоя = 2f;
    public float Коэф_Трения_Качения = 0.2f;
    public float Макс_Угл_Скорость = 8f;        
    public float Уск_Своб_Падения = 1f;

    public float Время_Нагрева = 20f;	       

    static GameConfig _instance;

    public static GameConfig Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            Load();                   
            Save();
            return _instance;
        }
    }

	public static void Load(bool fireEvent = false)
    {
        try
        {  
			_currentProfile = PlayerPrefs.GetInt(kPlayerPrefsName);
			var newP = JsonReader.Deserialize<GameConfig>(
				PlayerPrefs.GetString(string.Format("{0}.profile{1}", kPlayerPrefsName, _currentProfile)));
			_instance = newP != null ? newP : new GameConfig();         
			if (fireEvent && _instance != null)
				_instance.OnChanged();
        }
        catch 
        {
        }
    }

	private void OnChanged()
	{
		if (Changed != null)
		{
			Changed(this, EventArgs.Empty);
		}
        Physics.gravity = new Vector3(0, this.Уск_Своб_Падения, 0);
	}

	public static void Default()
	{
		PlayerPrefs.DeleteKey(string.Format("{0}.profile{1}", kPlayerPrefsName, _currentProfile));
		PlayerPrefs.Save();
		_instance = new GameConfig();
		_instance.OnChanged();
	}

	public int GetActiveProfile()
	{
		return _currentProfile;
	}

	public void SetActiveProfile(int profileNum)
	{
		_currentProfile = profileNum;
		PlayerPrefs.SetInt(kPlayerPrefsName, _currentProfile);
		Load(true);
		PlayerPrefs.Save();
	}

    public static void Save()
    {
        if (_instance == null)
            return;

		PlayerPrefs.SetString(string.Format("{0}.profile{1}", kPlayerPrefsName, _currentProfile), 
		                      JsonWriter.Serialize(_instance));
        PlayerPrefs.Save();   
		_instance.OnChanged();
   	}
}

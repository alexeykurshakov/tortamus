using System.ComponentModel;
using UnityEngine;
using System.Collections;


public class StateManager : MonoBehaviour
{
	[SerializeField] private StaffManager _staff;    

    //public const string CmdSwitchState = "switch state";	   

    //private bool CanSwichGameState(object sender, CommandArgs arguments)
    //{
    //    return true;
    //}

    //private void SwichGameState(object sender, CommandArgs args)
    //{
    //}

    public static StateManager Instance { get; private set; }

    public bool IsBusy { get; private set; }

	[SerializeField] private GameObject _level1;

	[SerializeField] private GameObject _level2;

	// private const float kCameraPos1 = -0.5505492f;
	private const float kCameraPos2 = 2.39f;

	private float _timer;

    public void SwitchState(GameStates state)
    {
		IsBusy = true;
		SoundManager.Instance.IsSoundEnabled = false;
		_level2.SetActive(true);
    }    

	private void Update()
	{
		if (!IsBusy)
			return;	

		var delta = Time.deltaTime;
		var cameraTransform = _staff.MainCamera.transform;
		var currentPos = cameraTransform.localPosition.x;
		currentPos += 2 * delta;
		if (currentPos >= kCameraPos2)
		{
			currentPos = kCameraPos2;
			SoundManager.Instance.StopAll();
			SoundManager.Instance.IsSoundEnabled = true;
			IsBusy = false;
		}
		cameraTransform.localPosition = new Vector3(currentPos, cameraTransform.localPosition.y, cameraTransform.localPosition.z); 
		SoundManager.Instance.transform.localPosition = cameraTransform.localPosition;
	}

    private void Awake()
    {
        Instance = this;
    }
}

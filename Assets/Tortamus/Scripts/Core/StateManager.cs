using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour
{
    public const string CmdSwitchState = "switch state";

    [SerializeField] private StaffManager _staff;    

    private bool CanSwichGameState(object sender, CommandArgs arguments)
    {
        return true;
    }

    private void SwichGameState(object sender, CommandArgs args)
    {
    }

    private void Awake()
    {
        CommandManager.RegisterCommandHandlers(CmdSwitchState, CanSwichGameState, SwichGameState);        
    }
}

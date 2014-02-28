using UnityEngine;
using System.Collections;

public class StaffManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    [SerializeField] private Transform _levelsRoot;

    public Store Store { get; private set; }
  
    private void Awake()
    {
        Store = new Store();
    }   
}

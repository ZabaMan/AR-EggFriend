using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARCentralManager : MonoBehaviour
{
    //public static ARDebugger Debug = new ARDebugger();

    private static ARCentralManager _Project;
    public GameManager GameManager;
    public NetworkManager NetworkManager;
    public MinigameManager MinigameManager;
 
    private void Awake()
    {
        if (_Project != null && _Project != this)
        {
            Destroy(this.gameObject);
        } else {
            _Project = this;
        }
    }
    
    public static ARCentralManager Project {
        get {
            return _Project;
        }
    }
    
    public TextMeshProUGUI text;

    
    
    public void Log(string debugText)
    {
        text.text = debugText + "\n" + text.text;
    }
}

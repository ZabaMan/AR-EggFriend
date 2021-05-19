using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ARDebugger : MonoBehaviour
{
    //public static ARDebugger Debug = new ARDebugger();

    private static ARDebugger _Debug;
 
    private void Awake()
    {
        if (_Debug != null && _Debug != this)
        {
            Destroy(this.gameObject);
        } else {
            _Debug = this;
        }
    }
    
    public static ARDebugger Debug {
        get {
            return _Debug;
        }
    }
    
    public TextMeshProUGUI text;

    
    
    public void Log(string debugText)
    {
        text.text = text.text + "\n" + debugText;
    }
}

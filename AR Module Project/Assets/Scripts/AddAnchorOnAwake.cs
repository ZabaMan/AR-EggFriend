using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AddAnchorOnAwake : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.AddComponent<ARAnchor>();
    }
}

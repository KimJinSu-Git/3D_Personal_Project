using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    public RectTransform iconUI;
    
    void Start()
    {
        FindObjectOfType<MinimapIconManager>()?.Register(this);
    }
}

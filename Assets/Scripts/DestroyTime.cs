using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTime : MonoBehaviour
{
    public float leftTime;
    // Start is called before the first frame update
    void Start()
    {
        
        Destroy(gameObject, leftTime);
    }

    
}

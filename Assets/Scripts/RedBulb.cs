using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBulb : MonoBehaviour
{
    SpriteRenderer Bulb;
    float DelayTime;
    float InitTime;
    void Start()
    {
        DelayTime = Random.Range(2,7);
        InitTime = DelayTime;
        Bulb = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (DelayTime > 0.0f)
            Bulb.enabled = true;
        if (DelayTime > -InitTime/4 && DelayTime < 0.0f)
            Bulb.enabled = false;
        
        if (DelayTime < -InitTime/4)
        {
            DelayTime = InitTime;
        }
        DelayTime -= Time.deltaTime;
    }
}

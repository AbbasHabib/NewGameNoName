using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash: MonoBehaviour
{ 
    public float FlashLife;

    void Start()
    {
        Invoke("DestroyBullet", FlashLife);
    }

    
    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}

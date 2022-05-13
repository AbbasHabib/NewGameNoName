using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBG : MonoBehaviour
{

    public bool IsStill = true;
    public float PEffect = 1;
    public GameObject Cam;
    public PlayerMovement P1;


    private float StartPosX;
    private float StartPosY;
    public float scaleFactorX;
    public float scaleFactorY;


    void Start()
    {
        StartPosX = transform.position.x;
        StartPosY = transform.position.y;         
    }



    void Update()
    {
        if(P1.transform.position.y < -60)
        {
            transform.position = new Vector3(StartPosX, transform.position.y, transform.position.z);
            return;
        }

        if (IsStill) // don't follow
            transform.position = new Vector3(StartPosX, StartPosY, transform.position.z);
        else // follow
        {
            this.transform.position = new Vector2(Cam.transform.position.x, this.transform.position.y);
            this.transform.position = Vector3.Lerp(this.transform.position, Cam.transform.position, PEffect * Time.deltaTime);
        }
        transform.localScale = new Vector3 (Camera.main.orthographicSize * scaleFactorX, Camera.main.orthographicSize * scaleFactorY, 0.0f);

    }

}

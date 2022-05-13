using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform Player1Pos;
    public float FlowUpSpeed = 0.125f;
    public Vector3 Offset;
    public bool Is2Player = false;

    [SerializeField]private Transform Player2Pos  = null;



    private Vector3 PosBeforeShake;
    [HideInInspector] public float ShakingVal;

    [HideInInspector] public static CameraController Instance;


    public float OriginalCameraSize;
    public float MaxcameraSize;


    private void Awake()
    {
        Is2Player = GameOverAllController.Instance.IS_TWO_PLAYER_GAME;
    }
    private void Start()
    {
        PosBeforeShake = transform.position;
        Instance = this;
        Camera.main.orthographicSize = OriginalCameraSize;
    }
    void FixedUpdate()
    {
        if(Is2Player)
        {
            float distance = Vector2.Distance(Player1Pos.transform.position, Player2Pos.transform.position);
            if (distance > OriginalCameraSize || Camera.main.orthographicSize > OriginalCameraSize)
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, distance < MaxcameraSize ? distance : MaxcameraSize, FlowUpSpeed * Time.deltaTime);
            }
            transform.position = Vector3.Lerp(transform.position,( Player1Pos.position  + Player2Pos.position + Offset) / 2.0f, FlowUpSpeed * Time.deltaTime);
     
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, Player1Pos.position + Offset, FlowUpSpeed * Time.deltaTime);
        }
    }

    public void ShakingCamera(float ShakeValue)
    {
        ShakingVal = ShakeValue > MaxValues.MAX_SHAKING_EFFECT ? MaxValues.MAX_SHAKING_EFFECT : ShakeValue;
        PosBeforeShake = transform.position;
        InvokeRepeating("CameraShake",0, 0.1f);
        Invoke("StopShaking", 0.3f);
    }

    void CameraShake()
    {    
        float quakeAmt = (Random.value % 3  - 1) * ShakingVal/ 2;
        Vector2 P = transform.position;
        P.y += quakeAmt;
        if(ShakingVal > 5)
            P.x += quakeAmt;
        transform.position = P;
    }

    void StopShaking()
    {
        CancelInvoke("CameraShake");
    }

}

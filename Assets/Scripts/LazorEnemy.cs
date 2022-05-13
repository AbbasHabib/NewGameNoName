using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazorEnemy : MonoBehaviour
{
    public float RotationSpeed;
    public float LookingDistance;
    public float RotatationValue;
    public string LazorHitSFX;
    private float DifferingTimer;

    public LineRenderer Laser; 

    [SerializeField]private float Damage;



    
    private float CountDown = 0.0f;
    private void Start()
    {
        Laser.enabled = true;
        DifferingTimer = Random.Range(0, 3);
        Physics2D.queriesStartInColliders = false;
    }
    private void Update()
    {
        if(DifferingTimer >= 0.0f)
        {
            DifferingTimer -= Time.deltaTime;
            return;
        }

        RaycastHit2D HitInfo = Physics2D.Raycast(transform.position, (transform.up * -1), LookingDistance);

        transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime);
        float val = transform.rotation.z;
        if (Mathf.Abs(val) >= RotatationValue / 100.0)
        {
            RotationSpeed = RotationSpeed * -1;
            transform.Rotate(Vector3.forward * RotationSpeed * Time.deltaTime * 10);
        }
        if (CountDown < 0.5f)
            CountDown += Time.deltaTime;

        if (HitInfo.collider != null)
        {
            Laser.SetPosition(1, HitInfo.point);

            if (HitInfo.collider.CompareTag("Player"))
            {
                if (CountDown >= 0.5f)
                {
                    HitInfo.collider.gameObject.GetComponent<PlayerController>().TakeDamage(Damage);
                    AudioManager.Instance.Play(LazorHitSFX);
                }
                CountDown = 0.0f;
            }
            Debug.DrawLine(transform.position, transform.position + (transform.up * -1) * LookingDistance, Color.green);
        }
        else
        {
            Debug.DrawLine(transform.position, transform.position + (transform.up * -1) * LookingDistance, Color.red);
            Laser.SetPosition(1, transform.position + (transform.up * -1) * LookingDistance);

        }
        Laser.SetPosition(0, transform.position);
    }

}

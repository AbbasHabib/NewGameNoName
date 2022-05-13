using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamagingSys : MonoBehaviour
{
    [Header("DataFromAnotherScripts")]
    public GameObject HealthBar;
    public CameraController Cam;
    public GameStylingController GStyler;
    public GameObject DestructionSpark;
    public string DestructionSFX;
    [SerializeField] private float Health;


    [Header("Effects")]
    public float BloomEff = 15;
    public float CameraShakeEffect = 15;


    private float HealthBarTmpX;
    private bool IsDead = false;


    private void Start()
    {
        HealthBarTmpX = HealthBar.transform.localScale.x;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (collision.gameObject.GetComponent<Bullet>().BulletType == 'F')
                return;
            TakeDamage(collision.gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            if (collision.gameObject.GetComponent<Bullet>().BulletType == 'F')
                return;
            TakeDamage(collision.gameObject);
        }
    }

    private void TakeDamage(GameObject collision)
    {
        Vector3 HB_Tmp = HealthBar.transform.localScale;
        float Damage = (collision.GetComponent<Bullet>().BulletPower / Health) * HealthBarTmpX;

        float tmp = HealthBar.transform.localScale.x - Damage;

        HealthBar.transform.localScale = new Vector3(tmp, HB_Tmp.y, HB_Tmp.z);
        float Eff = (collision.GetComponent<Bullet>().BulletPower) / 2.0f;

        Cam.ShakingCamera(Eff *3);
        GStyler.EffectBloom(BloomEff * Eff + 5);
    }
    private void Update()
    {
        if (IsDead)
            return;
        if (HealthBar.transform.localScale.x <= 0.0)
        {
            IsDead = true;
            Cam.ShakingCamera(CameraShakeEffect);

            DestroyEnemy();
        }
    }
    void DestroyEnemy()
    {
        Instantiate(DestructionSpark, this.transform.position, this.transform.rotation);
        GStyler.EffectBloom(BloomEff * 2);
        Cam.ShakingCamera(CameraShakeEffect +5);
        AudioManager.Instance.Play(DestructionSFX);
        Destroy(gameObject);
    }
}

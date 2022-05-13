using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RobotEnemySystem : MonoBehaviour
{
    [Header("DataFromAnotherScripts")]
    [SerializeField] private GameObject Bullet;
    public GameObject GetBullet()
    {
         return Bullet;        
    }
    [SerializeField]private Transform ShootingPoint;
    [SerializeField]private GameObject HealthBar;
    [SerializeField]private GameObject DestructionSpark;
    [SerializeField]private Transform GroundDetector;
    [SerializeField]private LayerMask GroundLayerMask;


    [Header("Effects")]
    [SerializeField]private float BloomEff;
    [SerializeField]private float CameraShakeEffect = 15;
    [SerializeField]private string BulletSFX;
    [SerializeField]private string EnemyDestructionSFX;


    [Header("EnemyPowers")]
    [SerializeField]private float Speed;
    [SerializeField]private float Health;
    [SerializeField]private bool CanShoot;
    [SerializeField]private float ShootingRange;
    [SerializeField]private float BulletHold;


    private float BulletHoldTemp;
    private float HealthBarTmpX;
    private bool IsDead = false;
    private string EnemyGroundName;
    private bool P_E_SameGround;


    private CameraController Cam;   
    private GameStylingController GStyler;
    private GameObject[] Players;

    private void Start()
    {
        Cam = CameraController.Instance;
        GStyler = GameStylingController.Instance;

        Players = GameObject.FindGameObjectsWithTag("Player");

        BulletHoldTemp = BulletHold;

        HealthBarTmpX = HealthBar.transform.localScale.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Bullet")
        {
            if (collision.gameObject.GetComponent<Bullet>().BulletType == 'F')
                return;
            TakeDamage(collision.gameObject);
        }

        if (collision.gameObject.tag == "Ground")
            EnemyGroundName = collision.gameObject.name;
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
        float Damage =  (collision.GetComponent<Bullet>().BulletPower / Health) * HealthBarTmpX;

        float tmp = HealthBar.transform.localScale.x - Damage;

        HealthBar.transform.localScale = new Vector3(tmp, HB_Tmp.y, HB_Tmp.z);

        float BullterPowEff = (collision.GetComponent<Bullet>().BulletPower) / 2.0f;

        Cam.ShakingCamera(BullterPowEff);
        GStyler.EffectBloom(BloomEff * BullterPowEff);
    }

    private void Update()
    {
        if (IsDead)
            return;
        BulletHoldTemp -= Time.deltaTime;
        GameObject _player  = null;
        bool IsGrounded = Physics2D.OverlapCircle(GroundDetector.position, 1f, GroundLayerMask);

        foreach (GameObject pl in Players)
        {
            if (EnemyGroundName == pl.GetComponent<PlayerController>().Get_PlayerGroundName())
            {
                P_E_SameGround = true;
                _player = pl;
                break;
            }
            else
            {
                P_E_SameGround = false;
            }
        }

        if (_player != null)
        {

            if (_player.transform.position.x > transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }

        if ( P_E_SameGround && CanShoot && BulletHoldTemp <= 0)
        {
            ShootAtPlayer();
            BulletHoldTemp = BulletHold;
        }

        if (IsGrounded && P_E_SameGround)
        {

            transform.Translate(new Vector3(-1, 0, 0) * Speed * Time.deltaTime);
        }
        else
            transform.Translate(new Vector3(0, 0, 0) * Speed * Time.deltaTime);

        if (HealthBar.transform.localScale.x <= 0.0)
        {        
            DestroyEnemy();
        }

        if(transform.position.y < -150f)
        {

            DestroyEnemy();
        }
    }
    public void DestroyEnemy()
    {
        //RobotDes
        IsDead = true;
        GStyler.EffectBloom(BloomEff *3);
        Cam.ShakingCamera(CameraShakeEffect);

        Instantiate(DestructionSpark, ShootingPoint.position, ShootingPoint.rotation);
        AudioManager.Instance.Play(EnemyDestructionSFX);
        Destroy(gameObject);
    }
    void ShootAtPlayer()
    {
        AudioManager.Instance.Play(BulletSFX);
        Instantiate(Bullet, ShootingPoint.position, ShootingPoint.rotation);
    }
  
}

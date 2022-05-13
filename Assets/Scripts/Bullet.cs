using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("FromAnotherScripts")]
    [SerializeField] private float BulletSpeed;
    [SerializeField] private float BulletLife;
    [SerializeField] private GameObject Explotion;
    [SerializeField] private char bulletType;
    [SerializeField] private float bulletPower;
    [SerializeField] private float BloomEff;
    public float BulletPower { get { return bulletPower; } set { bulletPower = value; } }
    public char BulletType { get { return bulletType; } set { bulletType = value; } }

    public string OwnedBy { get; set; } // specify who is the player shoot the bullet





    private float StartPos;

    private GameObject ColliderGob;
    private GameStylingController GSC;
    private CameraController CC;

    [Header("UseForEnemyBulletsOnly")]
    [SerializeField]private string BulletHitSFX = "";

    void Start()
    {
        GSC = GameStylingController.Instance;
        CC = CameraController.Instance;
        Invoke("DestroyBullet", BulletLife);
    }

    


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "GunTag" || collision.gameObject.tag == "PowerUpJump")
            return;
        if ((collision.gameObject.tag == "Player" && bulletType == 'P' && GameOverAllController.Instance.IS_SINGLE_PLAYER_GAME)
            || ((collision.gameObject.tag == "Player" && bulletType == 'P' &&
            GameOverAllController.Instance.IS_TWO_PLAYER_GAME_COOP)))
            return;

        if (bulletType == 'P' || bulletType == 'F')
            if (collision.gameObject.name == this.OwnedBy)   
                return;
        

        if (collision.gameObject.tag == "GunTag" || collision.gameObject.tag == "PowerUpJump")
            return;
        Debug.Log(collision.gameObject.name);

        
        AudioManager.Instance.Play(BulletHitSFX);
        

        Instantiate(Explotion, transform.position, transform.rotation);
        GSC.EffectBloom( bulletPower * BloomEff);
        CC.ShakingCamera(bulletPower);
        DestroyBullet();     
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "GunTag" || collision.gameObject.tag == "PowerUpJump")
            return;
        if ((collision.gameObject.tag == "Player" && bulletType == 'P' && GameOverAllController.Instance.IS_SINGLE_PLAYER_GAME)
            || ((collision.gameObject.tag == "Player" && bulletType == 'P' && 
            GameOverAllController.Instance.IS_TWO_PLAYER_GAME_COOP)))
            return;

        if (bulletType == 'P' || bulletType == 'F')
            if (collision.gameObject.name == this.OwnedBy)
                return;


        AudioManager.Instance.Play(BulletHitSFX);
        

        Instantiate(Explotion, transform.position, transform.rotation);
        GSC.EffectBloom(bulletPower * BloomEff);
        CC.ShakingCamera(bulletPower);

        DestroyBullet();

    }

    void FixedUpdate()
    {
       

        if (PlayerController.IsDead)
        {
            Destroy(gameObject);
            return;
        }
        if(bulletType == 'P' || bulletType == 'F')
        {

            transform.Translate(new Vector3(1, 0, 0) * BulletSpeed * Time.deltaTime);
        }
        else if (bulletType == 'E')
        {
            transform.Translate(new Vector3(-1, 0, 0) * BulletSpeed * Time.deltaTime);
        }      

    }

    void DestroyBullet()
    {
        Destroy(gameObject);
    }
}

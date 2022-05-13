using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("FromAnotherScripts")]

    [SerializeField]private Transform WeaponTip;
    [SerializeField]private Transform WeaponTipY;
    [SerializeField]private GameObject GunGob;
    [SerializeField]private CameraController CC;
    [SerializeField]private PlayerMovement PM;
    [SerializeField]private LineRenderer Laser;
    [SerializeField]private int VerticalLasorRange = 30;

    private float ShootingTemp;
    private Animator Anim;


    private GameObject bullet;
    private GameObject MuzzleFlash;
    private string ShootingSFX;
    private LineRenderer detectionLaser;



    /* network
    */
    public char _shooting_dir = 'H';


    public string explosionSFX { get; set; }

    [Header("ChangableValues")]

    [SerializeField] private float ShootCameraShake = 10f;

    [SerializeField]
    private float ShootingLimit = 0.3f;


    private void Start()
    {
        bullet = null;
        MuzzleFlash = null;
        ShootingTemp = ShootingLimit;
        Anim = GetComponent<Animator>();
        detectionLaser = Instantiate(Laser, transform.position, transform.rotation) as LineRenderer;
        detectionLaser.enabled = false;
    }

    void Shoot(char IsHorizontal)
    {
        _shooting_dir = IsHorizontal;
        Instantiate(MuzzleFlash, WeaponTip.position, WeaponTip.rotation);
        if (gameObject.GetComponent<PlayerController>().IsFrozen)
            return;
        if (IsHorizontal == 'H')
        {
            GameObject bullet_temp = Instantiate(bullet, WeaponTip.position, WeaponTip.rotation) as GameObject;
            bullet_temp.GetComponent<Bullet>().OwnedBy = this.gameObject.name;
        }
        else
        {
            GameObject bullet_temp = Instantiate(bullet, WeaponTipY.position, WeaponTipY.rotation) as GameObject;
            bullet_temp.GetComponent<Bullet>().OwnedBy = this.gameObject.name;
        }
        AudioManager.Instance.Play(ShootingSFX);

        CC.ShakingCamera(bullet.GetComponent<Bullet>().BulletPower >= 5 ?
            5 : bullet.GetComponent<Bullet>().BulletPower);
    }



    public void GotA_Weapon(Weapons Wp)
    {
        bullet = Wp.Bullet;
        MuzzleFlash = Wp.MuzzleFlash;
        ShootingTemp = ShootingLimit = Wp.ShootingTime;
        ShootingLimit = 0.0f;
        explosionSFX = Wp.ExplosionSFX;
        ShootingSFX = Wp.ShootingSound;
    }

    
    void Update()
    {
        if (ShootingLimit > 0.0f)
        {
            ShootingLimit -= Time.deltaTime; // trigger when it's less than 0
            GunGob.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, ShootingLimit);
        }
        else if (ShootingLimit <= 0.0f)
        {
            GunGob.GetComponent<SpriteRenderer>().color = Color.white;
        }
      

        if (GunGob.activeSelf && !this.GetComponent<PlayerController>().IsFrozen)
        {
            if ((Input.GetAxisRaw(PM.playerMov.YMovement) > 0.0f
                || Input.GetAxisRaw(PM.playerMov.YMovement + 'J') > 0.0f))
            {
                if (Input.GetButton(PM.playerMov.Shoot))
                {
                    if (ShootingLimit <= 0.0f)
                    {
                        GunGob.GetComponent<SpriteRenderer>().color = Color.white;
                        if (PM.IsGrounded)
                            Anim.SetTrigger("IsShootingInY");
                        else
                            Anim.SetTrigger("IsShootingYJumping");
                        Shoot('V');
                        ShootingLimit = ShootingTemp;
                        return;
                    }
                }
                Vector3 currPos = this.gameObject.transform.position;
                Vector3 upperPos = new Vector3(currPos.x, currPos.y + 10, currPos.z);
                //Debug.DrawLine(transform.position, transform.position + (transform.up * -1) * 500, Color.red);
                detectionLaser.enabled = true;
                GameObject lazer_point = transform.Find("WeaponTip").gameObject;
                detectionLaser.SetColors(new Color(255, 0, 0, 0.0f), new Color(255, 0, 0, 0.6f));
                detectionLaser.SetPosition(0, lazer_point.transform.position);
                detectionLaser.SetPosition(1, lazer_point.transform.position + (transform.up) * VerticalLasorRange);
            }
            else if (Input.GetButton(PM.playerMov.Shoot))
            {
                if (ShootingLimit <= 0.0f)
                {
                    GunGob.GetComponent<SpriteRenderer>().color = Color.white;
                    Anim.SetTrigger("IsShootingX");
                    ShootingLimit = ShootingTemp;
                    Shoot('H');
                    return;
                }
            }
            else
                detectionLaser.enabled = false;


        }
    }
}

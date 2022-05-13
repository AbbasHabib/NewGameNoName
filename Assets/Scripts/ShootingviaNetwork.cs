using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingviaNetwork : MonoBehaviour
{
    [Header("FromAnotherScripts")]

    [SerializeField] private Transform WeaponTip;
    [SerializeField] private Transform WeaponTipY;
    [SerializeField] private GameObject GunGob;
    [SerializeField] private CameraController CC;
    [SerializeField] private MoveviaNetwork PM;

    private float ShootingTemp;
    private Animator Anim;


    private GameObject bullet;
    private GameObject MuzzleFlash;
    private string ShootingSFX;



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

    void ShootNetwork(char direction)
    {
        if (direction == 'x')
            return;
        if (GunGob.activeSelf && !this.GetComponent<PlayerController>().IsFrozen)
        {

            if (direction == 'v')
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
            else if (direction == 'h')
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
        }
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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    [Header("Configrations")]
    [SerializeField] private GameObject bullet;
    public GameObject Bullet { get { return bullet; } }
    [SerializeField]private GameObject muzzleFlash;
    public GameObject MuzzleFlash { get { return muzzleFlash; }}

    [SerializeField]private Sprite gunShape;
    public Sprite GunShape  { get { return gunShape; } }

    [SerializeField]private float shootingTime;
    public float ShootingTime { get { return shootingTime; }}

    [SerializeField]private string shootingSound;
    public string ShootingSound { get { return shootingSound; }  }

    [SerializeField] private string explosionSFX;
    public string ExplosionSFX { get { return explosionSFX; } }

    [Header("Transform")]

    [SerializeField]private Vector3 scale;
    public Vector3 Scale { get { return scale; } }

    public Weapons CloneWeapon()
    {
        Weapons sc = gameObject.AddComponent<Weapons>() as Weapons;

        sc.bullet = this.bullet;
        sc.muzzleFlash = this.muzzleFlash;
        sc.gunShape = this.gunShape;
        sc.shootingTime = this.shootingTime;
        sc.shootingSound = this.shootingSound;
        sc.explosionSFX = this.explosionSFX;
        sc.scale = this.scale;
        return sc;
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    [Header("FromAnotherGobs")]
    [SerializeField] private GameObject GunGob;
    [SerializeField] private GameObject ExplosionOfPlayer;
    [SerializeField] private Animator Anim;
    [SerializeField] private GameStylingController Gstyle;
    [SerializeField] private GameObject BulletPoint;
    [SerializeField] private PlayerMovement PlayerMov;
    [SerializeField] private GameObject WeaponEff;
    [SerializeField] private GameObject PowerUpEff;
    [SerializeField] private GameObject FreezeIceCube;
    [SerializeField] private GameObject PlayerDataBar;


    [Header("ChangableValues")]
    [SerializeField] private float Health = 10;
    [SerializeField] private float HitEffectBloom;



    [HideInInspector] public static bool IsDead = false;


    private bool isFrozen = false;
    public bool IsFrozen { get => isFrozen; set => isFrozen = value; }


    private Rigidbody2D Rb;
    private Vector3 Deathpos;
    private PlayerShooting PShooting;
    private string CurrentGround;
    private float FreezeTime = 0.0f; // this variable recieves a value when 
    // player gets shot by a freezing bullet

    private List<Weapons> ownedWeapons;
    private int currentWeapon = -1;

    public float Get_Health()
    {
        return Health;
    }

    private void Start()
    {
        PShooting = this.gameObject.GetComponent<PlayerShooting>();
        Rb = gameObject.GetComponent<Rigidbody2D>();
        Deathpos = transform.position;
        PlayerDataBar.GetComponent<HealthBar>().SetMaxValue(Health);
        ownedWeapons = new List<Weapons>();
    }

    private void AddWeapon(Weapons wp)
    {
        bool isOwned = false;
        if (ownedWeapons.Count == 0)
        {
            ChangeWeapon(wp);
            currentWeapon = 0;
        }
        ownedWeapons.ForEach(weapon => {
            if (weapon.GunShape == wp.GunShape)           
                isOwned = true;        
        });
        if (!isOwned)
        {
            ownedWeapons.Add(wp.CloneWeapon());
        }
    }

    private void ChangeWeapon(Weapons CollisionWep)
    {
        GunGob.transform.localScale = CollisionWep.Scale;
        GunGob.GetComponent<SpriteRenderer>().sprite = CollisionWep.GunShape;
        PShooting.GotA_Weapon(CollisionWep);
        AudioManager.Instance.Play("ChnageWeaponsSFX");
        PlayerDataBar.GetComponent<GunBar>().ChangeUIWeapon(CollisionWep.GunShape);
        GunGob.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "GunTag")
        {
            Weapons CollisionWep = collision.gameObject.GetComponent<Weapons>();
            Instantiate(WeaponEff, BulletPoint.transform.position, transform.rotation);
            AddWeapon(CollisionWep);
            AudioManager.Instance.Play("PowerUpSFX");
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "PowerUpJump")
        {
            PowerUpJump(collision);
            Instantiate(PowerUpEff, BulletPoint.transform.position, transform.rotation);
            Debug.Log("JumpTime powerup");
            AudioManager.Instance.Play("PowerUpSFX");
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Bullet")
        {
            Bullet B = collision.gameObject.GetComponent<Bullet>();
            if ((B.BulletType == 'F' || B.BulletType == 'P') && GameOverAllController.Instance.IS_TWO_PLAYER_GAME_COOP)
                return;
            if (B.BulletType == 'F' && B.OwnedBy != this.gameObject.name) // Freezing bullet
            {
                StartCoroutine(FreezePlayer(B.BulletPower));
            }
            else if (B.BulletType == 'E')
                TakeDamage(B.BulletPower);
            else if (B.BulletType == 'P' && B.OwnedBy != this.gameObject.name)
                TakeDamage(B.BulletPower);
        }
    }

    void PowerUpJump(Collider2D collision)
    {
        PlayerMov._JumpTimeIncreament(collision.gameObject.GetComponent<PowerUps>().JumpTimeIncreament);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            CurrentGround = collision.gameObject.name;

        if (collision.gameObject.tag == "Enemy")
        {
            RobotEnemySystem EScript = collision.gameObject.GetComponent<RobotEnemySystem>();
            TakeDamage(EScript.GetBullet().GetComponent<Bullet>().BulletPower + 1);
            EScript.DestroyEnemy();
        }

        if (collision.gameObject.tag == "Bullet")
        {
            Bullet B = collision.gameObject.GetComponent<Bullet>();
            if ((B.BulletType == 'F' || B.BulletType == 'P') && GameOverAllController.Instance.IS_TWO_PLAYER_GAME_COOP)
                return;
            if (B.BulletType == 'F' && B.OwnedBy != this.gameObject.name) // Freezing bullet
            {
                FreezeTime = B.BulletPower;
            }
            else if (B.BulletType == 'E')
                TakeDamage(B.BulletPower);
            else if (B.BulletType == 'P' && B.OwnedBy != this.gameObject.name)
                TakeDamage(B.BulletPower);
        }
    }

    public void TakeDamage(float Damage)
    {
        Health -= Damage;
        PlayerDataBar.GetComponent<HealthBar>().SetHealth(Health);
        Gstyle.EffectChromaticAb(1);
        Gstyle.EffectBloom(HitEffectBloom);
    }

    public string Get_PlayerGroundName()
    {
        return CurrentGround;
    }

    void PlayerFell()
    {
        if (this.transform.position.y < -150f)
        {
            Gstyle.EffectBloom(80f);
            Gstyle.EffectChromaticAb(1);
            PlayerDataBar.GetComponent<HealthBar>().SetHealth(0.0f);
            StartCoroutine(KillPlayer());
        }
    }

    IEnumerator FreezePlayer(float freezTime)
    {
        Vector3 freezingPos = this.gameObject.transform.position;
        GameObject iceCube = null;
        iceCube = Instantiate(FreezeIceCube, this.transform.position, this.gameObject.transform.rotation) as GameObject;
        IsFrozen = true;
        while (freezTime > 0.0f)
        {
            freezTime -= Time.deltaTime;
            IsFrozen = true;
            this.gameObject.transform.position = freezingPos;
            if (iceCube != null)
            {
                IceCube iceCubeScript = iceCube.GetComponent<IceCube>();
                iceCubeScript.MeltIce(freezTime);
            }
            yield return null;
        }
        if (IsFrozen && iceCube != null)
        {
            IceCube Script = iceCube.GetComponent<IceCube>();
            Script.RemoveIce();
        }
        IsFrozen = false;
    }

    private void Update()
    {
        if(Health <= 0 && !IsDead)
        {
            IsDead = true;
            Instantiate(ExplosionOfPlayer, BulletPoint.transform.position, BulletPoint.transform.rotation);
            Gstyle.EffectBloom(80);
            Gstyle.EffectChromaticAb(1);
            Deathpos = this.transform.position;
            PlayerDataBar.GetComponent<HealthBar>().SetHealth(0.0f, hard_set:true);
            
            StartCoroutine(KillPlayer());
        }
        if (IsDead)
        {
            this.transform.position = Deathpos;
            PlayerDataBar.GetComponent<HealthBar>().SetHealth(0.0f, hard_set: true);
            this.transform.eulerAngles = new Vector3(0, 0, 0);
            GunGob.SetActive(false);
        }
        if (Input.GetButtonDown(this.PlayerMov.playerMov.ChangeWeapon) && ownedWeapons.Count > 1)
        {
            ChangeWeapon(ownedWeapons[currentWeapon + 1 >= ownedWeapons.Count? currentWeapon = 0 : ++currentWeapon]);
        }

        PlayerFell(); //  this function checks if the player has fallen
    }

    IEnumerator KillPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        IsDead = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}

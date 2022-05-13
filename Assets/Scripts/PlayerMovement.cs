using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControlInputs
{
    public string XMovement;
    public string YMovement;
    public string Jump;
    public string Shoot;
    public string Dash;
    public string ChangeWeapon;
}

public class PlayerMovement : MonoBehaviour
{
    private Animator Anim;
    private Rigidbody2D Rb;
    private string _input_string;
    public ControlInputs playerMov = new ControlInputs();


    private float JumpLimitTemp;
    private float RunFasterTemp;

    [Header("ChangableValues")]
    [SerializeField]
    private float JumpLimit = 10.0f;
    [SerializeField]
    private float RunFaster = 0.3f;
    [SerializeField]
    private int Speed;
    [SerializeField]
    private int JumpForce;
    [SerializeField]
    private Client _client;

    [HideInInspector] public bool Left = false;
    [HideInInspector] public bool Right = true;

    [HideInInspector] public bool IsGrounded { get; set; }
    [HideInInspector] public bool IsRunning { get; set; }
    [HideInInspector] public bool IsDodging { get; set; }
    [HideInInspector] public bool DuringJump { get; set; }


    [Header("FromAnotherGOBS")]
    [SerializeField] private Transform GroundChecker;
    [SerializeField] private Transform GroundChecker2;
    [SerializeField] private Transform GroundChecker3;

    public LayerMask GroundLayerMask;

    private void Start()
    {
        JumpLimitTemp = JumpLimit;
        RunFasterTemp = RunFaster;
        IsGrounded  = false;
        IsRunning = false;
        IsDodging  = false;
        DuringJump = false;
        Anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }

   

    void Run_()
    {
        
        if (PlayerController.IsDead)
            return;
        float faster = 1;
        float Movement = Input.GetAxisRaw(playerMov.XMovement);
        if(Movement == 0.0f)
            Movement = Input.GetAxisRaw(playerMov.XMovement+'J');
        float YAxis = Input.GetAxisRaw(playerMov.YMovement);
        if (YAxis == 0.0f)
            YAxis = Input.GetAxisRaw(playerMov.YMovement + 'J');


        if (Input.GetButton(playerMov.Dash))
        {
            faster = 1.35f;
        }

        if (Movement > 0)
        {
            IsRunning = true;
            transform.eulerAngles = new Vector3(0, 0, 0);
            Right = true;
            Left = false;
        }
        else if (Movement < 0)
        {
            IsRunning = true;
            Left = true;
            Right = false;
            transform.eulerAngles = new Vector3(0, 180, 0);

        }
        if (YAxis == -1 && IsGrounded)
        {
            Anim.SetBool("IsDodging", true);
            Anim.SetBool("IsRunning", false);
            IsRunning = false;
            IsDodging = true;
            Rb.velocity = new Vector2(0.0f, Rb.velocity.y);

            return;
        }
        else
        {
            Anim.SetBool("IsDodging", false);
            IsDodging = false;
        }
        if (Movement == 0)
        {
            Anim.SetBool("IsRunning", false);
            RunFasterTemp = RunFaster;
            
            IsRunning = false;
        }
        else
        {
            Anim.SetBool("IsRunning", true);
            IsRunning = true;
        }

        

        Rb.velocity = new Vector2(Movement * Speed * faster, Rb.velocity.y);
    }
    void Jump_()
    {
        if (PlayerController.IsDead)
            return;
        if (gameObject.GetComponent<PlayerController>().IsFrozen)
            return;
        if (IsGrounded == false && DuringJump == false)
        {
            Anim.SetBool("InSpace", true);
        }
        else
        {
            Anim.SetBool("InSpace", false);

        }
        if (Input.GetButton(playerMov.Jump) && IsGrounded == true)
        {
            DuringJump = true;
            Rb.velocity = Vector2.up * JumpForce;
            Anim.SetTrigger("TakeOff");
            AudioManager.Instance.Play("Jump");
            JumpLimitTemp = JumpLimit; //reseting the jumping limit
            IsGrounded = false;
        }
        if (IsGrounded)
        {
            Anim.SetBool("IsJumping", false);
        }
        else
        {
            Anim.SetBool("IsJumping", true);
        }

        if (Input.GetButtonUp(playerMov.Jump))
        {
            DuringJump = false;
        }
        if (DuringJump)
        {
            if (JumpLimitTemp > 0)
            {
                Rb.velocity = Vector2.up * JumpForce;
                JumpLimitTemp -= Time.deltaTime;
            }
            else
            {
                DuringJump = false;
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (PlayerController.IsDead)
            return;
        IsGrounded = Physics2D.OverlapCircle(GroundChecker.position, 2, GroundLayerMask);
        bool IsGrounded2 = Physics2D.OverlapCircle(GroundChecker2.position, 2, GroundLayerMask);
        bool IsGrounded3 = Physics2D.OverlapCircle(GroundChecker3.position, 2, GroundLayerMask);


        if (IsGrounded || IsGrounded2 || IsGrounded3)
            IsGrounded = true;

        Jump_();
        Run_();
        //bool ischanged = false;
        //string s = GenerateInputString(out ischanged);
        //if (ischanged)
        //    _client.SendInputs(s);
    }

    private string GenerateInputString(out bool change)
    {
        change = false;
        string inp = "";
        if (Right)
            inp += 'r';
        else if (Left)
            inp += 'l';
        if (IsRunning)        
            inp += 'g';
        else
            inp += 'x';
        if (IsDodging)
            inp += 'd';
        else
            inp += 'x';
        if (DuringJump)
            inp += 'j';
        else
            inp += 'x';

        if(IsRunning || IsDodging || DuringJump)
            change = true;

        if (this.GetComponent<PlayerShooting>()._shooting_dir == 'H')  // shooting
        {
            inp += 'h';
            change = true;
        }
        else if (this.GetComponent<PlayerShooting>()._shooting_dir == 'V')
        {
            inp += 'v';
            change = true;
        }
        return inp;
    }

    public void _JumpTimeIncreament(float val)
    {
        JumpLimit = val;
    }

}

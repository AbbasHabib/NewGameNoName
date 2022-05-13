using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveviaNetwork : MonoBehaviour
{
    private Animator Anim;
    private Rigidbody2D Rb;
    private string _input_string;



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
        IsGrounded = false;
        IsRunning = false;
        IsDodging = false;
        DuringJump = false;
        Anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
    }



    void Run_(Movement mv)
    {

        if (PlayerController.IsDead)
            return;

        if (mv.isRunning)
            IsRunning = true;
        else
            IsRunning = false;
        if (mv.right)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            Right = true;
            Left = false;
        }
        else if (mv.left)
        {
            Left = true;
            Right = false;
            transform.eulerAngles = new Vector3(0, 180, 0);

        }
        if (mv.isDodging)
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
        if (!mv.isRunning)
        {
            Anim.SetBool("IsRunning", false);
         
            IsRunning = false;
        }
        else
        {
            Anim.SetBool("IsRunning", true);
            IsRunning = true;
        }


        if (IsGrounded == false && DuringJump == false)
        {
            Anim.SetBool("InSpace", true);
        }
        else
        {
            Anim.SetBool("InSpace", false);

        }
        if (mv.isJumping)
        {
            DuringJump = true;
            Anim.SetTrigger("TakeOff");
            AudioManager.Instance.Play("Jump");
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
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public ParticleSystem Dust;
    public Animator Animator;
    public AudioSource jump;
    public AudioSource axemove;

    public GameObject victoryText;
    public float speed;
    public float jumpForce;
    public float floatForce;
    private float moveInput;
    private float axeSwingsRemaining;
    public float jumpTime;
    public float floatTime;
    private float floatTimeCounter;
    private float jumpTimeCounter;
    private float coyoteTime = 0.5f;
    private float coyoteTimeCounter;

    private Rigidbody2D rb;

    public bool facingRight = true;
    private bool isGrounded;
    private bool isJumping;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatisGround;

    private int extraJumps;
    public int extraJumpValue;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        extraJumps = extraJumpValue;
    }

    private void Update()
    {
        if (isGrounded == true)
        {
            coyoteTimeCounter = coyoteTime;
            speed = 7;
            axeSwingsRemaining = 1;
            Animator.SetBool("Isjumping", false);
            Animator.SetBool("Isspinning", false);

        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatisGround);

        if (coyoteTimeCounter > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            CreateDust();
            jump.Play();
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping == true)
        {
            Animator.SetBool("Isjumping", true);

            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            coyoteTimeCounter = 0f;
        }

        if (Input.GetKeyDown(KeyCode.None) && isJumping == false && isGrounded == false) 
        {
            axemove.Play();
            floatTimeCounter = floatTime;
            Animator.SetBool("Isspinning", true);
        }

        if (Input.GetKey(KeyCode.None) && axeSwingsRemaining == 1)
        {
            if (floatTimeCounter > 0f)
            {
                CreateDust();
                speed = 2;
                rb.velocity = Vector2.up * floatForce;
                floatTimeCounter -= Time.deltaTime;
            }

            if (floatTimeCounter <= 0)
            {
                axeSwingsRemaining--;
                axemove.Stop();
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            axeSwingsRemaining--;
            axemove.Stop();
        }

    }

    private void FixedUpdate()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        Animator.SetFloat("Speed", Mathf.Abs(moveInput));

        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;

    }

    void CreateDust()
    {
        Dust.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("death"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (collision.gameObject.CompareTag("victory"))
        {
            victoryText.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}

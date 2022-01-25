using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 1.4f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbingSpeed = 2.5f;
    [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    float playerGravity;
    Vector2 moveInput;

    bool isAlive = true;

    Rigidbody2D playerRigidbody;
    Animator playerAnimator;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D feetCollider;


    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerGravity = playerRigidbody.gravityScale;
        playerAnimator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (!isAlive) { return; }

        Run();
        FlipSprite();
        ClimbLadder();
        CheckEnemiesOrHazardsTouched();
    }

    void FlipSprite()
    {
        if (IsRunning())
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidbody.velocity.x), 1f);
        }
    }

    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (!IsTouchingTheGround()) { return; }

        if (value.isPressed)
        {
            playerRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }

        Instantiate(bullet, gun.position, gun.rotation);
    }

    void ClimbLadder()
    {
        playerAnimator.SetBool("isClimbing", IsClimbing());

        if (!IsTouchingALeader())
        {
            playerRigidbody.gravityScale = playerGravity;
            return;
        }
        Vector2 playerClimb = new Vector2(moveInput.x * runSpeed, moveInput.y * climbingSpeed);
        playerRigidbody.velocity = playerClimb;
        playerRigidbody.gravityScale = 0f;



        /* if (IsClimbing())
        {
            playerAnimator.StopPlayback();
        }
        else
        {
            playerAnimator.StartPlayback();
        } */

    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = playerVelocity;

        playerAnimator.SetBool("isRunning", IsRunning());
    }

    void CheckEnemiesOrHazardsTouched()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            Die();
        }
    }

    bool IsRunning()
    {
        return Mathf.Abs(playerRigidbody.velocity.x) > Mathf.Epsilon;
    }

    bool IsClimbing() => Mathf.Abs(playerRigidbody.velocity.y) > Mathf.Epsilon && IsTouchingALeader();
    bool IsTouchingTheGround() => feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    bool IsTouchingALeader() => bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
    void Die()
    {
        isAlive = false;
        playerAnimator.SetTrigger("Dying");
        playerRigidbody.velocity = deathKick;
        playerRigidbody.freezeRotation = false;
    } 
}

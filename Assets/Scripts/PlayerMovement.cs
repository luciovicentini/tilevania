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

    float playerGravity;
    Vector2 moveInput;
    Rigidbody2D playerRigidbody;
    Animator playerAnimator;
    CapsuleCollider2D playerCollider;


    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerGravity = playerRigidbody.gravityScale;
        playerAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
    }

    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
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
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!IsTouchingTheGround()) { return; }

        if (value.isPressed)
        {
            playerRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void ClimbLadder()
    {
        if (!IsTouchingALeader())
        {
            playerRigidbody.gravityScale = playerGravity;
            playerAnimator.SetBool("isClimbing", false);
            return;
        }

        Vector2 playerClimb = new Vector2(moveInput.x * runSpeed, moveInput.y * climbingSpeed);
        playerRigidbody.velocity = playerClimb;
        playerRigidbody.gravityScale = 0f;

        playerAnimator.SetBool("isClimbing", true);

        if (IsClimbing())
        {
            playerAnimator.StopPlayback();
        }
        else
        {
            playerAnimator.StartPlayback();
        }

    }


    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, playerRigidbody.velocity.y);
        playerRigidbody.velocity = playerVelocity;

        playerAnimator.SetBool("isRunning", IsRunning());
    }

    bool IsRunning()
    {
        return Mathf.Abs(playerRigidbody.velocity.x) > Mathf.Epsilon;
    }

    bool IsClimbing() => Mathf.Abs(playerRigidbody.velocity.y) > Mathf.Epsilon;
    bool IsTouchingTheGround() => playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    bool IsTouchingALeader() => playerCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
}

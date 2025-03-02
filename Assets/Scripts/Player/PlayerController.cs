using System;
using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidBody2D;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float diagonalMovementMultiplier = 1f;
    [SerializeField] internal Vector2 startPosition;

    private Tween rockingTween;
    private Vector2 movementDirection;


    private void Start()
    {
        ResetToStartPosition();
    }
    void Update()
    {
        AnimationRockingWhileWalking();
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
       WalkingWithFixedDiagonalMovement(diagonalMovementMultiplier);
    }

    private void AnimationRockingWhileWalking()
    {
        if (movementDirection != Vector2.zero)
        {
            if (rockingTween == null || !rockingTween.IsPlaying())
            {
                rockingTween = transform.DOLocalRotate(new Vector3(0, 0, 10), 0.15f, RotateMode.Fast)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .ChangeStartValue(new Vector3(0, 0, -10)); // Zaczyna od -15 stopni
            }
        }
        else
        {
            rockingTween?.Kill(); 
            transform.DOLocalRotate(Vector3.zero, 0.2f, RotateMode.Fast).SetEase(Ease.InOutSine);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Obstacle"))
            Debug.Log("Hit Obstacle");
    }

    private void WalkingWithFixedDiagonalMovement(float diagonalMovementSpeedMultiplier)
    {
        if (movementDirection.x != 0 && movementDirection.y != 0)
        {
            playerRigidBody2D.AddForce(movementDirection * (moveSpeed * diagonalMovementSpeedMultiplier));
            return;
        }
        
        playerRigidBody2D.AddForce(movementDirection * moveSpeed);
    }

    internal void ResetToStartPosition()
    {
        startPosition = transform.position;
    }
    
}

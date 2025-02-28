using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerRigidBody2D;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float diagonalMovementSpeedMultiplier = 1f;
    [SerializeField] internal Vector2 startPosition;
    private Vector2 movementDirection;


    private void Start()
    {
        transform.position = startPosition;
    }
    void Update()
    {
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (movementDirection.x != 0 && movementDirection.y != 0)
        {
            playerRigidBody2D.AddForce(movementDirection * (moveSpeed * diagonalMovementSpeedMultiplier));
            return;
        }
        
        playerRigidBody2D.AddForce(movementDirection * moveSpeed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.CompareTag("Obstacle"))
            Debug.Log("Hit Obstacle");
    }
}

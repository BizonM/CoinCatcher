using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LevelConfigurationSO levelConfiguration;
    [SerializeField] private Rigidbody2D playerRigidBody2D;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float diagonalMovementMultiplier = 1f;
    [SerializeField] private LayerMask obstacleLayer;

    private Tween rockingTween;
    private Vector2 movementDirection;
    private bool gameIsRunning = true;

    private void Start()
    {
        GameEvents.Current.OnSetPlayerPosition += SetStartPlayerPosition;
        GameEvents.Current.OnGameRunning += GameIsRunning;

        SetStartPlayerPosition();
    }

    private void OnDisable()
    {
        GameEvents.Current.OnSetPlayerPosition -= SetStartPlayerPosition;
        GameEvents.Current.OnGameRunning -= GameIsRunning;
    }

    void Update()
    {
        movementDirection.x = Input.GetAxisRaw("Horizontal");
        movementDirection.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (gameIsRunning)
        {
            AnimationRockingWhileWalking();
            if (movementDirection.x != 0 && movementDirection.y != 0)
            {
                WalkingWithFixedDiagonalMovement(diagonalMovementMultiplier);
                return;
            }

            Walking();
        }
    }

    private void AnimationRockingWhileWalking()
    {
        if (movementDirection != Vector2.zero)
        {
            if (rockingTween == null || !rockingTween.IsPlaying())
            {
                rockingTween = transform.DOLocalRotate(new Vector3(0, 0, 10), 0.15f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .ChangeStartValue(new Vector3(0, 0, -10));
            }
        }
        else
        {
            rockingTween?.Kill();
            transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine);
        }
    }

    private void GameIsRunning(bool isRunning)
    {
        gameIsRunning = isRunning;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((obstacleLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            GameEvents.Current.GotHit();
        }
    }

    private void Walking()
    {
        WalkingWithFixedDiagonalMovement(diagonalMovementMultiplier);

        playerRigidBody2D.AddForce(movementDirection * moveSpeed);
    }

    private void WalkingWithFixedDiagonalMovement(float diagonalMovementSpeedMultiplier)
    {
        playerRigidBody2D.AddForce(movementDirection * (moveSpeed * diagonalMovementSpeedMultiplier));
    }

    private void SetStartPlayerPosition()
    {
        transform.position = new Vector2(levelConfiguration.LevelWidth / 2f
            , levelConfiguration.LevelHeight / 2f);
    }
}
using DG.Tweening;
using UnityEngine;

public class ObstacleSaw : MonoBehaviour
{
    [SerializeField] private float durationOfTurningAround;
    [SerializeField] private Transform centerTransform;

    [SerializeField] private bool cirleAround;
    private void Start()
    {
        transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
        
        if (cirleAround)
            CircleAroundCenter();
    }

    private void CircleAroundCenter()
    {
        centerTransform.DORotate(new Vector3(0, 0, 360), durationOfTurningAround, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
}

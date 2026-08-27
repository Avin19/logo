using UnityEngine;
using DG.Tweening;

public class CoinRotation : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationDuration = 2f;

    [SerializeField] private bool rotateClockwise = true;

    private Tween rotationTween;

    private void OnEnable()
    {
        StartRotation();
    }

    private void StartRotation()
    {
        transform.DOKill();

        float rotation = rotateClockwise ? -360f : 360f;

        rotationTween = transform
    .DORotate(
        new Vector3(0f, rotation, 0f),
        rotationDuration,
        RotateMode.FastBeyond360
    )
    .SetEase(Ease.Linear)
    .SetLoops(-1, LoopType.Restart);
    }

    private void OnDisable()
    {
        if (rotationTween != null)
        {
            rotationTween.Kill();
            rotationTween = null;
        }
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
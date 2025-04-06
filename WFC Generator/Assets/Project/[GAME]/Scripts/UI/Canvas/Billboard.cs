using DG.Tweening;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    #region Animation
    private float bounceDistance = 0.3f; 
    private float duration = 0.6f;       

    private Vector3 originalLocalPosition;
    private RectTransform rectTransform;
    #endregion

    Canvas canvas;
    private Transform _mainCameraTransform;

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        originalLocalPosition = rectTransform.localPosition;
        StartBouncing();
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        _mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + _mainCameraTransform.forward);
    }

    void StartBouncing()
    {
        DOTween.To(() => 0f, t =>
        {
            Vector3 bounceDir = Camera.main.transform.up;
            Vector3 currentWorldPosition = rectTransform.parent.TransformPoint(originalLocalPosition);
            rectTransform.position = currentWorldPosition + bounceDir * Mathf.Sin(t * Mathf.PI) * bounceDistance;
        }, 1f, duration)
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}

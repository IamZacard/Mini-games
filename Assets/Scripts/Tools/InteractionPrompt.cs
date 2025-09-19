using UnityEngine;
using DG.Tweening;
using System.Collections;

public class InteractionPrompt : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float idlePulseScale = 1.05f;
    [SerializeField] private float idlePulseDuration = 0.8f;
    [SerializeField] private float urgentPulseScale = 1.15f;
    [SerializeField] private float urgentPulseDuration = 0.35f;
    [SerializeField] private float attentionDelay = 5f;

    private Tween currentPulseTween;
    private Coroutine attentionCoroutine;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StartIdlePulse();

        if (attentionCoroutine != null) StopCoroutine(attentionCoroutine);
        attentionCoroutine = StartCoroutine(AttentionRoutine());
    }

    public void Hide()
    {
        StopAllFeedback();
        gameObject.SetActive(false);
    }

    public void StopAllFeedback()
    {
        if (attentionCoroutine != null)
        {
            StopCoroutine(attentionCoroutine);
            attentionCoroutine = null;
        }
        ResetScale();
    }

    private IEnumerator AttentionRoutine()
    {
        yield return new WaitForSeconds(attentionDelay);

        if (gameObject.activeSelf)
            StartUrgentPulse();

        attentionCoroutine = null;
    }

    private void StartIdlePulse()
    {
        KillCurrentPulse();
        currentPulseTween = transform
            .DOScale(originalScale * idlePulseScale, idlePulseDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StartUrgentPulse()
    {
        KillCurrentPulse();
        currentPulseTween = transform
            .DOScale(originalScale * urgentPulseScale, urgentPulseDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void KillCurrentPulse()
    {
        currentPulseTween?.Kill();
        currentPulseTween = null;
    }

    private void ResetScale()
    {
        KillCurrentPulse();
        transform.localScale = originalScale;
    }

    private void OnDisable() => StopAllFeedback();
    private void OnDestroy() => StopAllFeedback();
}
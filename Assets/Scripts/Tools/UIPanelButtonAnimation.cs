using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPanelButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private Button button;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private float pressDuration = 0.1f;
    //[SerializeField] private AudioClip hoverSound;
    //[SerializeField] private AudioClip clickSound;

    private Vector3 originalScale;
    private AudioSource audioSource;
    private void Awake()
    {
        button = GetComponentInParent<Button>();

        originalScale = transform.localScale;
        //audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;

        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, hoverDuration).SetEase(Ease.OutQuad);
        
        //audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;

        transform.DOKill();
        transform.DOScale(originalScale, hoverDuration).SetEase(Ease.OutQuad);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;

        transform.DOKill();
        Sequence pressSequence = DOTween.Sequence();
        pressSequence.Append(transform.DOScale(originalScale * pressScale, pressDuration).SetEase(Ease.InOutQuad));
        pressSequence.Append(transform.DOScale(originalScale, pressDuration).SetEase(Ease.InOutQuad));
        //audioSource.PlayOneShot(clickSound);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}

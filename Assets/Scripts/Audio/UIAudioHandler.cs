using UnityEngine;
using UnityEngine.EventSystems;

public class UIAudioHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioEvent mClickSFX;
    [SerializeField] private AudioEvent mHoverSFX;

    public void OnPointerClick(PointerEventData eventData)
    {
        mClickSFX.Play2DSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mHoverSFX.Play2DSound();
    }
}

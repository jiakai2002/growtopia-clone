using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using System;

public class Drag : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    CinemachineComponentBase componentBase;

    public void Awake()
    {
        rectTransform = transform.parent.GetComponent<RectTransform>();
        componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = rectTransform.anchoredPosition;
        float targetY = position.y + eventData.delta.y * 0.6f;
        targetY = Mathf.Clamp(targetY, -190, 120);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, targetY);
        if (rectTransform.anchoredPosition != position)
        {
            if (componentBase is CinemachineFramingTransposer)
            {
                (componentBase as CinemachineFramingTransposer).m_ScreenY += eventData.delta.y * 0.0003f;
            }
        }

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
    }
    public void OnEndDrag(PointerEventData eventData)
    {
    }
}

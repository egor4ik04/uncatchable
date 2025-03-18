using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClicked : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private Sprite baseSprite;
    [SerializeField] private Sprite clickedSprite;
    
    public event Action OnClick;
    public event Action OnClickUp;
    public event Action OnClickDown;

    private void Start()
    {
        Image image = GetComponent<Image>();
        baseSprite ??= image.sprite;
        clickedSprite ??= image.sprite;        
    }

    public void SwapSprite(bool isPressed)
    {
        Image image = GetComponent<Image>();
        if (!isPressed)
            image.sprite = baseSprite;
        else
            image.sprite = clickedSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SwapSprite(true);
        OnClickDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SwapSprite(false);
        OnClickUp?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}

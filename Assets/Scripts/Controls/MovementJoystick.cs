using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

public class MovementJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public static MovementJoystick Instance;

    [Header("Properties")]
    [SerializeField] RectTransform pad;
    [SerializeField] RectTransform stick;
    [SerializeField]  Transform[] positionfocus;

    [Header("Settings")]
    [SerializeField] private bool fixedJoystick;
    [SerializeField] private bool hideJoystick;

    public Action OnTap;
    public Action OnDrag;
    private bool isMovingStick;
    private float releaseTime;
    private Vector3 move;
    private Vector3 padStartPos;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        stick.anchoredPosition = Vector3.zero;
        padStartPos = pad.anchoredPosition;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        pad.gameObject.SetActive(true);
        if (!fixedJoystick) pad.position = eventData.position;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!isMovingStick) isMovingStick = true;
        
        releaseTime += Time.deltaTime;

        stick.anchoredPosition = eventData.position;
        stick.anchoredPosition = Vector3.ClampMagnitude(eventData.position - (Vector2)pad.position, pad.rect.width * 0.5f);

        move = new Vector3(stick.localPosition.x, stick.localPosition.y,0 ).normalized;

        SetFocus();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        isMovingStick = false;

        if (releaseTime < 0.2f)
        {
            if (stick.localPosition.magnitude < pad.rect.width * 0.4f)
            {
                if (OnTap != null) OnTap.Invoke();
            }

            if (stick.localPosition.magnitude >= pad.rect.width * 0.4f)
            {
                if (OnDrag != null) OnDrag.Invoke();
            }
        }

        releaseTime = 0;
        stick.anchoredPosition = Vector3.zero;
        move = Vector3.zero;

        if (hideJoystick) pad.gameObject.SetActive(false);
        if (!fixedJoystick) pad.anchoredPosition = padStartPos;

        foreach (var item in positionfocus) item.gameObject.SetActive(false);
    }

    void SetFocus()
    {
        positionfocus[0].gameObject.SetActive(move.x < 0 && move.y > 0);
        positionfocus[1].gameObject.SetActive(move.x > 0 && move.y > 0);
        positionfocus[2].gameObject.SetActive(move.x > 0 && move.y < 0);
        positionfocus[3].gameObject.SetActive(move.x < 0 && move.y < 0);
    }

    public bool IsDragingStick() {return isMovingStick;}

    public Vector3 Input() {return move;}
}

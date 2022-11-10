using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScreenSwapHandler : MonoBehaviour
{
    public static ScreenSwapHandler Instance { get; private set; }

    public EventHandler OnRightSwipe;
    public EventHandler OnLeftSwipe;
    public EventHandler OnUpSwipe;
    public EventHandler OnDownSwipe;
    public EventHandler OnTap;

    [SerializeField] int screenPercentageForSwipe;

    private Vector3? firstTochPos = Vector3.zero;
    private  Vector3 lastTouchPos = Vector3.zero;
    private float dragDistance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dragDistance = Screen.height * screenPercentageForSwipe / 100;
    }

    void Update()
    {
      HandleSwipe();
    }

    public void HandleSwipe()
    {   
        if (Input.touchCount == 1)
        {    
            Touch touch = Input.GetTouch(0);

            if (firstTochPos != null && InputHandler.Instance.GetMoveVector() != Vector2.zero)
            {
                firstTochPos = null;
                return;
            }

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    firstTochPos = touch.position;
                    lastTouchPos = touch.position;

                    break;
                case TouchPhase.Moved:

                    lastTouchPos = touch.position;

                    break;
                case TouchPhase.Ended:

                    if (firstTochPos == null) return;
                    
                    lastTouchPos = touch.position;
                    if (Mathf.Abs(lastTouchPos.x - firstTochPos.Value.x) > dragDistance ||   Mathf.Abs(lastTouchPos.y - firstTochPos.Value.y) > dragDistance)
                    {
                        if (Mathf.Abs(lastTouchPos.x - firstTochPos.Value.x) > Mathf.Abs(lastTouchPos.y - firstTochPos.Value.y))
                        {
                            if (lastTouchPos.x > firstTochPos.Value.x) OnRightSwipe?.Invoke(this, EventArgs.Empty);
                            else OnLeftSwipe?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            if (lastTouchPos.y > firstTochPos.Value.y) OnUpSwipe?.Invoke(this, EventArgs.Empty);
                            else OnDownSwipe?.Invoke(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        OnTap?.Invoke(this, EventArgs.Empty);
                    }

                    break;

                default: break;
            }
        }
    }

    public bool IsPointerOverUI(int fingerId)
    {
        EventSystem eventSystem = EventSystem.current; return (eventSystem.IsPointerOverGameObject(fingerId) && eventSystem.currentSelectedGameObject != null);
    }
}

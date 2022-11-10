using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    [SerializeField] private bool onlyShowOnMobile;
    [SerializeField] Transform[] positionFocus;
    private Vector2 moveInput;

    void Start()
    {
        if (onlyShowOnMobile) gameObject.SetActive(Application.isMobilePlatform);
    }
    
    void Update()
    {
        SetFocus();
    }
    
    void SetFocus()
    {
        moveInput = InputHandler.Instance.GetMoveVector();
        positionFocus[0].gameObject.SetActive(moveInput.x < 0 && moveInput.y > 0);
        positionFocus[1].gameObject.SetActive(moveInput.x > 0 && moveInput.y > 0);
        positionFocus[2].gameObject.SetActive(moveInput.x > 0 && moveInput.y < 0);
        positionFocus[3].gameObject.SetActive(moveInput.x < 0 && moveInput.y < 0);
    }
}

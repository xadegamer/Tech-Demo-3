using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCombatHandler : MonoBehaviour
{
    [SerializeField] private Transform selectedTraget;

    private RaycastHit2D hitInfo;
    
    [Header("Ref")]
    private AbilityControllerBase abilityControllerBase;
    private Animator animator;

    private void Awake()
    {
        abilityControllerBase = GetComponent<AbilityControllerBase>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        CheckTargetTouched();

        HandleAttack();
    }

    public void CheckTargetTouched()
    {
        if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Moved) hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector3.forward);
        else if (Input.GetMouseButtonDown(0)) hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
        else return;

        if (hitInfo.collider != null)
        {
            Debug.Log("Hit: " + hitInfo.collider.name);
            selectedTraget = hitInfo.collider.transform;
        }
    }

    void DetectUI()
    {
        var results = new List<RaycastResult>();

        PointerEventData pointerEventData = new(EventSystem.current) { position = Input.mousePosition };

        EventSystem.current.RaycastAll(pointerEventData, results);

        if (results.Count > 0)
        {
            GameObject UI = results[0].gameObject;

            if (UI) Debug.Log(UI.name);
        }
    }

    public void HandleAttack()
    {
        if (InputHandler.Instance.GetAttackInput().WasPressedThisFrame())
        {
            Debug.Log("Attack");
        }
    }
}

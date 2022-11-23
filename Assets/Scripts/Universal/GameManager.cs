using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private PlayerUnit player;
    [SerializeField] private Vector2 playerStartPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerStartPos = player.transform.position;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnPlayerRoutine());
    }

    public IEnumerator RespawnPlayerRoutine()
    {
        AbilityUIManager.Instance.ToggleActive(false);
        Targetter.SetTarget(null);
        yield return new WaitForSeconds(2);
        player.Respawn(playerStartPos);
    }
}

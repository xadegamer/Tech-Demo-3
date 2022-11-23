using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        yield return new WaitForSeconds(2);
        player.Respawn(playerStartPos);
    }
}

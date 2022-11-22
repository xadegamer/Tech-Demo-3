using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerUnit player;
    [SerializeField] private Vector2 playerStartPos;

    private void Start()
    {
        playerStartPos =  player.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnPlayerRoutine());
    }

    public IEnumerator RespawnPlayerRoutine()
    {
        yield return new WaitForSeconds(3);
        player.Respawn(playerStartPos);
    }
}

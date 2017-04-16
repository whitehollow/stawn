using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private List<IPlayerRespawnListener> listeners;

    public void Awake()
    {
        listeners = new List<IPlayerRespawnListener>();
    }

    public void PlayerCheckPointHit()
    {
        StartCoroutine(PlayerCheckPointHitCo(LevelManager.Instance.CurrentTimeBonus));
    }

    private IEnumerator PlayerCheckPointHitCo(int bonus)
    {
        FloatingText.Show("Checkpoint!", "CheckpointText", new CenterTextPositioner(0.5f));
        yield return new WaitForSeconds(.5f);

        FloatingText.Show(string.Format("+{0}!", bonus), "CheckpointText", new  CenterTextPositioner(0.5f));

    }

    public void PlayerLeftCheckPoint()
    {

    }

    public void SpawnPlayer(Player player)
    {
        player.RespwanAt(transform);

        foreach (var list in listeners)
        {
            list.OnPlayerRespawnInThisCheckpoint(this, player);
        }
    }

    public void AssignObjectToCheckPoint(IPlayerRespawnListener l)
    {
        listeners.Add(l);
    }
}


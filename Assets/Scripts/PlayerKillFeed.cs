using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKillFeed : MonoBehaviour
{
    [SerializeField]
    GameObject killFeedItem;

    void Start()
    {
        GameManager.instance.onPlayerKilledCallback += OnKill;
    }

    public void OnKill(string player, string source)
    {
        Debug.Log(source + " killed " + player);
        GameObject go = (GameObject)Instantiate(killFeedItem, this.transform);
        go.GetComponent<PlayerKillFeedItem>().Setup(player, source);
        go.transform.SetAsFirstSibling();
        Destroy(go, 10f);
    }

}

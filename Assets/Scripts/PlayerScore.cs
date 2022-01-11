using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerScore : MonoBehaviour
{
    int lastKills = 0;
    int lastDeaths = 0;

    PlayerManager player;

    void Start()
    {
        player = GetComponent<PlayerManager>();
        StartCoroutine(SyncScoreLoop());
    }

    IEnumerator SyncScoreLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);
            SyncNow();
        }
    }

    void OnDestroy()
    {
        if(player != null)
            SyncNow();
    }

    void SyncNow()
    {
        if (UserAccManager.IsLoggedIn)
        {
            UserAccManager.instance.GetDataBtn(OnDataRecieved);
        }
    }

    void OnDataRecieved(string data)
    {
        if (player.kills <= lastKills && player.deaths <= lastDeaths)  //only sync while necessary
            return;

        int killsSinceLast = player.kills - lastKills;
        int deathsSinceLast = player.deaths - lastDeaths;

        if (killsSinceLast == 0 && deathsSinceLast == 0)
            return;

        int kills = UserAccDataTrans.ExtractKills(data);
        int deaths = UserAccDataTrans.ExtractDeaths(data);

        int newKills = killsSinceLast + kills;
        int newDeaths = deathsSinceLast + deaths;

        string newData = UserAccDataTrans.ImportData(newKills, newDeaths);

        Debug.Log("Syncing: " + newData);

        lastKills = player.kills;
        lastDeaths = player.deaths;

        UserAccManager.instance.SendDataBtn(newData);
    }


}

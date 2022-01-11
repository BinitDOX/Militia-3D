using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreBoard : MonoBehaviour
{
    [SerializeField]
    GameObject playerSBItem;
    [SerializeField]
    Transform playerSBList;
    //[SerializeField]
    //Prototype.NetworkLobby.LobbyPlayer lobbyPlayer;

    void OnEnable()
    {
        //lobbyManager = GameObject.Find("LobbyManager");
        //lobbyPlayer = 

        //Get array of players
        PlayerManager[] players = GameManager.GetAllPlayers();

        //Loop thru and set up list items
        foreach(PlayerManager player in players)
        {
            Debug.Log(player.userName + " - " + player.kills + " - " + player.deaths);
            GameObject itemGO = Instantiate(playerSBItem, playerSBList);
            PlayerScoreBoardItem item = itemGO.GetComponent<PlayerScoreBoardItem>();
            if(item != null)
            {
                item.Setup(player.userName, player.kills, player.deaths);//, Color.blue;
            }
        }

    }

    void OnDisable()
    {
        //Clean up list.. bcz player may join in btw game
        foreach(Transform child in playerSBList)
        {
            Destroy(child.gameObject);
        }
    }
}

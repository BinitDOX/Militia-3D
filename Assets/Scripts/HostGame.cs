using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostGame : NetworkBehaviour
{
    [SerializeField]
    private uint roomSize = 8;

    private string roomName;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
        if(networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker();
        }
    }

    public void SetRoomName (string name)
    {
        roomName = name;
    }

    public void CreateRoom()
    {
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating Room: " + roomName + " Size: " + roomSize);
            networkManager.matchMaker.CreateMatch(roomName, roomSize, true, "","","",0,0,networkManager.OnMatchCreate);
        }
    }
}

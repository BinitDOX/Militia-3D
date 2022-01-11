using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerManager))]
//[RequireComponent(typeof(PlayerControllerUC))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] components; //to disable
    [SerializeField]
    string remoteLayerName = "RemotePlayer";
    [SerializeField]
    string dontRender = "DontRender";
    [SerializeField]
    GameObject playerGraphics;
    [SerializeField]
    GameObject playerUIPrefab;
    [HideInInspector]
    public GameObject playerUIInstance;

    //private Camera sceneCam;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            //sceneCam = Camera.main;
            //if (sceneCam != null)
            //{
            //    sceneCam.gameObject.SetActive(false);
            //}
            //Disable player graphics
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontRender));

            //Create player UI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //Configure player UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if(ui == null)
            {
                Debug.LogError("No UI");
            }
            ui.SetPlayer(GetComponent<PlayerManager>()); //changed from controller to player

            GetComponent<PlayerManager>().PlayerSetup();

            string username = "Loading...";
            if (UserAccManager.IsLoggedIn)
                username = UserAccManager.PlayerUsername;
            else
                username = transform.name;

            CmdSetUserName(transform.name, username);
        }
        //RegisterPlayer(); GameManager handles this now

        //GetComponent<PlayerManager>().Setup(); //moving this up
        GetComponent<PlayerManager>().currentHealth = 100f;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerManager player = GetComponent<PlayerManager>();

        GameManager.RegisterPlayer(netID, player);
    }

    //void RegisterPlayer() //give unique name
    //{
    //    string ID = "Player " + GetComponent<NetworkIdentity>().netId;
    //    transform.name = ID;
    //}

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < components.Length; i++)
        {
            components[i].enabled = false;
        }
    }

    void OnDisable()
    {
        Destroy(playerUIInstance);

        //Re-enable scene cam
        //if(sceneCam != null)
        //{
        //    sceneCam.gameObject.SetActive(true);
        //}
        if(isLocalPlayer)
            GameManager.instance.SetSceneCamActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }

    [Command]
    void CmdSetUserName(string playerID, string userName)
    {
        RpcSetUserName(playerID, userName);
        PlayerManager player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
            //Debug.Log(userName + " has arrived!");
            player.userName = userName;
        }
    }

    [ClientRpc]
    void RpcSetUserName(string playerID, string userName)
    {
        PlayerManager player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
            Debug.Log(userName + " has arrived!");
            player.userName = userName;
        }
    }
}

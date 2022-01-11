using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMotorUC))]
public class PlayerThruster : NetworkBehaviour
{
    private PlayerMotorUC pm;

    public GameObject[] burners;

    void Start()
    {
        pm = GetComponent<PlayerMotorUC>();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if(pm.thrustActivated)
        {
            CmdActivateThruster();
        }
        else
        {
            CmdDeactivateThruster();
        }
    }

    [Command]
    public void CmdActivateThruster()
    {
        burners[0].SetActive(true);
        burners[1].SetActive(true);
        RpcActivateThruster();
    }

    [Command]
    public void CmdDeactivateThruster()
    {
        burners[0].SetActive(false);
        burners[1].SetActive(false);
        RpcDeactivateThruster();
    }

    [ClientRpc]
    void RpcActivateThruster()
    {
        burners[0].SetActive(true);
        burners[1].SetActive(true);
    }

    [ClientRpc]
    void RpcDeactivateThruster()
    {
        burners[0].SetActive(false);
        burners[1].SetActive(false);
    }
}

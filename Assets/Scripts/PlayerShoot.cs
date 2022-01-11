using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private PlayerWeapon currWeapon;
    private PlayerWeapon currWeapon2;

    private float fireTime;
    private float fireTime2;

    private bool hitPlayer = false;

    private float launchForce = 45f;


    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
      
        weaponManager = GetComponent<WeaponManager>();
    }


    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Weapon1
        currWeapon = weaponManager.GetCurrentWeapon();
        if (currWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            fireTime -= Time.deltaTime;
            if (Input.GetButtonDown("Fire1") && fireTime <= 0f)
            {
                InvokeRepeating("Shoot", 0f, (float)1f/currWeapon.fireRate);
                fireTime = (float)1f / currWeapon.fireRate;
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
            if (fireTime < -60f)
                fireTime = -1f;
        }

        //Weapon2
        currWeapon2 = weaponManager.GetCurrentWeapon2();
        if (currWeapon2.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                Shoot2();
            }
        }
        else
        {
            fireTime2 -= Time.deltaTime;
            if (Input.GetButtonDown("Fire2") && fireTime2 <= 0f)
            {
                InvokeRepeating("Shoot2", 0f, (float)1f / currWeapon2.fireRate);
                fireTime2 = (float)1f / currWeapon2.fireRate;
            }
            else if (Input.GetButtonUp("Fire2"))
            {
                CancelInvoke("Shoot2");
            }
            if (fireTime2 < -60f)
                fireTime2 = -1f;
        }
    }

    [Command]
    void CmdOnShoot()
    {
        //We shooting, tell the other clients
        RpcOnShoot();
    }
    [ClientRpc]
    void RpcOnShoot()
    {
        weaponManager.GetWeaponGraphics().muzzleFlash.Play();
    }

    [Command]
    void CmdOnShoot2()
    {
        //We shooting, tell the other clients
        RpcOnShoot2();
    }
    [ClientRpc]
    void RpcOnShoot2()
    {
        GameObject shellInstance = Instantiate(weaponManager.GetWeaponGraphics2().shell, weaponManager.GetWeaponGraphics2().launchPos.position, weaponManager.GetWeaponGraphics2().launchPos.rotation);
        Rigidbody rb = shellInstance.GetComponent<Rigidbody>();
        Shell shell = shellInstance.GetComponent<Shell>();
        shell.launchedBy = transform.name;
        rb.velocity = launchForce * weaponManager.GetWeaponGraphics2().launchPos.forward;
        //weaponManager.GetWeaponGraphics2().muzzleFlash.Play();
    }

    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal, bool hit)
    {
        RpcOnHit(pos, normal, hit);
    }
    [ClientRpc]
    void RpcOnHit(Vector3 pos, Vector3 normal, bool hit)
    {
        if (!hit)
        {
            GameObject hitEff = (GameObject)Instantiate(weaponManager.GetWeaponGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
            Destroy(hitEff, 1f);
            GameObject bulletEff = (GameObject)Instantiate(weaponManager.GetWeaponGraphics().bulletHole, pos, Quaternion.LookRotation(normal));
            Destroy(bulletEff, 3f);
        }
        else
        {
            GameObject hitEff2 = (GameObject)Instantiate(weaponManager.GetWeaponGraphics().hitEffectPlayerPrefab, pos, Quaternion.LookRotation(normal));
            Destroy(hitEff2, 1f);
            GameObject bloodEff = (GameObject)Instantiate(weaponManager.GetWeaponGraphics().bloodEffect, pos, Quaternion.LookRotation(normal));
            Destroy(bloodEff, 2f);
        }

    }


    [Client]
    public void Shoot()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //We are shooting, tell the server
        CmdOnShoot();

        //Debug.Log("Shoo");
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currWeapon.range, mask))
        {
            //We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG) //can use layer too
            {
                CmdPlayerShot(hit.collider.name, currWeapon.damage, transform.name);
                hitPlayer = true;
            }

            CmdOnHit(hit.point, hit.normal, hitPlayer);
        }
        hitPlayer = false;
    }

    [Client]
    public void Shoot2()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //GameObject shellInstance = Instantiate(weaponManager.GetWeaponGraphics2().shell, weaponManager.GetWeaponGraphics2().launchPos.position, weaponManager.GetWeaponGraphics2().launchPos.rotation);
        //Rigidbody rb = shellInstance.GetComponent<Rigidbody>();
        //rb.velocity = launchForce * weaponManager.GetWeaponGraphics2().launchPos.forward;
        //We are shooting, tell the server
        CmdOnShoot2();

        //Debug.Log("Shoo2");
        //RaycastHit hit;
        //if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currWeapon.range, mask))
        //{
        //    //We hit something
        //    //Debug.Log("We hit " + hit.collider.name);
        //    if (hit.collider.tag == PLAYER_TAG) //can use layer too
        //    {
        //        CmdPlayerShot(hit.collider.name, currWeapon2.damage);
        //    }
        //}


    }

    [Command]
    void CmdPlayerShot(string playerID, float damage, string sourceID)
    {
        Debug.Log(playerID + " has been shot!");

        PlayerManager player = GameManager.GetPlayer(playerID);
        player.RpcTakeDamage(damage, sourceID);
    }

    //public void Explode(string name, float dm)
    //{
    //    CmdPlayerExplo(name, dm);
    //}

    //[Command]
    //void CmdPlayerExplo(string playerID, float damage)
    //{
    //    Debug.LogError(playerID + " has been exploded!");

    //    PlayerManager player = GameManager.GetPlayer(playerID);
    //    player.RpcTakeDamage(damage);
    //}
}

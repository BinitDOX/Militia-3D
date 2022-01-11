using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerShootOld : NetworkBehaviour
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

    [Client]
    void Shoot()
    {
        Debug.Log("Shoo");
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currWeapon.range, mask))
        {
            //We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG) //can use layer too
            {
                CmdPlayerShot(hit.collider.name, currWeapon.damage);
            }
        }
    }

    [Client]
    void Shoot2()
    {
        Debug.Log("Shoo2");
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currWeapon.range, mask))
        {
            //We hit something
            //Debug.Log("We hit " + hit.collider.name);
            if (hit.collider.tag == PLAYER_TAG) //can use layer too
            {
                CmdPlayerShot(hit.collider.name, currWeapon2.damage);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string playerID, float damage)
    {
        Debug.Log(playerID + " has been shot!");

        PlayerManager player = GameManager.GetPlayer(playerID);
        //player.RpcTakeDamage(damage);
    }
}

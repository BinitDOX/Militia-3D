using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerShootJoy : NetworkBehaviour
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

    [SerializeField]
    private GameObject joyCanvas;
    private Button joyFireBtn1;
    private Button joyFireBtn2;
    private Button joyReloadBtn1;
    private Button joyReloadBtn2;
    private bool joyFlag = false;

    public bool fire1 = false;
    public bool fire2 = false;

    private bool invokeRepeat = false;
    private bool invokeRepeat2 = false;


    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
      
        weaponManager = GetComponent<WeaponManager>();

    }

    public void OnFire1()
    {
        fire1 = true;
    }
    public void OffFire1()
    {
        fire1 = false;
    }
    public void OnFire2()
    {
        fire2 = true;
    }
    public void OffFire2()
    {
        fire2 = false;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (joyCanvas == null)
        {
            joyCanvas = GameObject.Find("PlayerUI");
        }
        else if (!joyFlag)
        {
            joyFireBtn1 = joyCanvas.transform.Find("ButtonP").GetComponent<Button>();
            joyFireBtn2 = joyCanvas.transform.Find("ButtonS").GetComponent<Button>();
            joyReloadBtn1 = joyCanvas.transform.Find("ReloadButton1").GetComponent<Button>();
            joyReloadBtn2 = joyCanvas.transform.Find("ReloadButton2").GetComponent<Button>();

            EventTrigger eventTrigger = joyFireBtn1.gameObject.AddComponent<EventTrigger>();

            // Pointer down
            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { OnFire1(); });
            eventTrigger.triggers.Add(pointerDownEntry);

            // Pointer up
            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { OffFire1(); });
            eventTrigger.triggers.Add(pointerUpEntry);

            EventTrigger eventTrigger2 = joyFireBtn2.gameObject.AddComponent<EventTrigger>();

            // Pointer down
            EventTrigger.Entry pointerDownEntry2 = new EventTrigger.Entry();
            pointerDownEntry2.eventID = EventTriggerType.PointerDown;
            pointerDownEntry2.callback.AddListener((data) => { OnFire2(); });
            eventTrigger2.triggers.Add(pointerDownEntry2);

            // Pointer up
            EventTrigger.Entry pointerUpEntry2 = new EventTrigger.Entry();
            pointerUpEntry2.eventID = EventTriggerType.PointerUp;
            pointerUpEntry2.callback.AddListener((data) => { OffFire2(); });
            eventTrigger2.triggers.Add(pointerUpEntry2);

            EventTrigger eventTrigger3 = joyReloadBtn1.gameObject.AddComponent<EventTrigger>();

            // Pointer down
            EventTrigger.Entry pointerDownEntry3 = new EventTrigger.Entry();
            pointerDownEntry3.eventID = EventTriggerType.PointerDown;
            pointerDownEntry3.callback.AddListener((data) => { if (currWeapon.bullets < currWeapon.maxBullets) { weaponManager.Reload(); return; } });
            eventTrigger3.triggers.Add(pointerDownEntry3);

            //// Pointer up
            //EventTrigger.Entry pointerUpEntry3 = new EventTrigger.Entry();
            //pointerUpEntry3.eventID = EventTriggerType.PointerUp;
            //pointerUpEntry3.callback.AddListener((data) => { OffFire2(); });
            //eventTrigger3.triggers.Add(pointerUpEntry3);

            EventTrigger eventTrigger4 = joyReloadBtn2.gameObject.AddComponent<EventTrigger>();

            // Pointer down
            EventTrigger.Entry pointerDownEntry4 = new EventTrigger.Entry();
            pointerDownEntry4.eventID = EventTriggerType.PointerDown;
            pointerDownEntry4.callback.AddListener((data) => { if (currWeapon2.bullets < currWeapon2.maxBullets) { weaponManager.Reload2(); return; } });
            eventTrigger4.triggers.Add(pointerDownEntry4);

            //// Pointer up
            //EventTrigger.Entry pointerUpEntry4 = new EventTrigger.Entry();
            //pointerUpEntry4.eventID = EventTriggerType.PointerUp;
            //pointerUpEntry4.callback.AddListener((data) => { OffFire2(); });
            //eventTrigger4.triggers.Add(pointerUpEntry4);

            joyFlag = true;
        }

        //Weapon1
        currWeapon = weaponManager.GetCurrentWeapon();
        if (currWeapon.fireRate <= 0f)
        {
            //if (Input.GetButtonDown("Fire1"))
            //{
            //    Shoot();
            //}
        }
        else
        {

            fireTime -= Time.deltaTime;
            if (fire1 && fireTime <= 0f)
            {
                if (!invokeRepeat)
                {
                    //Debug.Log("shootOn");
                    InvokeRepeating("Shoot", 0f, (float)1f / currWeapon.fireRate);
                    invokeRepeat = true;
                }
                fireTime = (float)1f / currWeapon.fireRate;
            }
            else if (!fire1)
            {
                //Debug.Log("shootOff");
                CancelInvoke("Shoot");
                invokeRepeat = false;
            }
            if (fireTime < -60f)
                fireTime = -1f;
        }

        //Weapon2
        currWeapon2 = weaponManager.GetCurrentWeapon2();
        if (currWeapon2.fireRate <= 0f)
        {
            if (fire2)
            {
                Shoot2();
            }
        }
        else
        {
            fireTime2 -= Time.deltaTime;
            if (fire2 && fireTime2 <= 0f)
            {
                if (!invokeRepeat2)
                {
                    InvokeRepeating("Shoot2", 0f, (float)1f / currWeapon2.fireRate);
                    invokeRepeat2 = true;
                }
                fireTime2 = (float)1f / currWeapon2.fireRate;
            }
            else if (!fire2)
            {
                CancelInvoke("Shoot2");
                invokeRepeat2 = false;
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
        if (!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if (currWeapon.bullets <= 0)
        {
            //Debug.Log("Out of ammo");
            weaponManager.Reload();
            return;
        }
        currWeapon.bullets--;
        Debug.Log("Bullets: " + currWeapon.bullets);


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

        if(currWeapon.bullets <= 0)
        {
            weaponManager.Reload();
        }
    }

    [Client]
    public void Shoot2()
    {
        if (!isLocalPlayer || weaponManager.isReloading2)
        {
            return;
        }

        if (currWeapon2.bullets <= 0)
        {
            //Debug.Log("Out of ammo");
            weaponManager.Reload2();
            return;
        }
        currWeapon2.bullets--;
        Debug.Log("Bullets2: " + currWeapon2.bullets);


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
        if (currWeapon2.bullets <= 0)
        {
            weaponManager.Reload2();
        }

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

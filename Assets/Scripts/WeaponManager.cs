using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class WeaponManager : NetworkBehaviour
{
    private const string WEAPON_LAYER = "Weapon";

    [SerializeField]
    private Transform weaponHolder;
    [SerializeField]
    private Transform weaponHolder2;
    [SerializeField]
    private PlayerWeapon primWeapon;
    [SerializeField]
    private PlayerWeapon secWeapon;

    private PlayerWeapon currWeapon;
    private PlayerWeapon currWeapon2;

    private WeaponGraphics currGraphics;
    private WeaponGraphics2 currGraphics2;

    public bool isReloading = false;
    public bool isReloading2 = false;

    void Start()
    {
        EquipWeapon(primWeapon, secWeapon);
    }

    public PlayerWeapon GetCurrentWeapon() 
    {
        return currWeapon;
    }
    public PlayerWeapon GetCurrentWeapon2()
    {
        return currWeapon2;
    }

    public WeaponGraphics GetWeaponGraphics()
    {
        return currGraphics;
    }
    public WeaponGraphics2 GetWeaponGraphics2()
    {
        return currGraphics2;
    }

    void EquipWeapon(PlayerWeapon weapon, PlayerWeapon weapon2)
    {
        currWeapon = weapon;
        currWeapon2 = weapon2;

        GameObject weaponIns = (GameObject)Instantiate(weapon.graphics, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        //weaponIns.transform.SetParent(weaponHolder);
        GameObject weaponIns2 = (GameObject)Instantiate(weapon2.graphics, weaponHolder2.position, weaponHolder2.rotation, weaponHolder2);
        //weaponIns2.transform.SetParent(weaponHolder2);

        currGraphics = weaponIns.GetComponent<WeaponGraphics>();
        currGraphics2 = weaponIns2.GetComponent<WeaponGraphics2>();
        if(currGraphics == null || currGraphics2 == null)
        {
            Debug.LogError("No WeaponGFX compnent on weapon obj");
        }

        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(WEAPON_LAYER));
            Util.SetLayerRecursively(weaponIns2, LayerMask.NameToLayer(WEAPON_LAYER));
        }

    }

    public void Reload()
    {
        if (isReloading)
            return;

        StartCoroutine(ReloadCoroutine());
    }

    public void Reload2()
    {
        if (isReloading2)
            return;

        StartCoroutine(ReloadCoroutine2());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        CmdOnReload();

        yield return new WaitForSeconds(currWeapon.reloadTime);
        currWeapon.bullets = currWeapon.maxBullets;

        isReloading = false;
    }

    private IEnumerator ReloadCoroutine2()
    {
        isReloading2 = true;

        CmdOnReload2();


        yield return new WaitForSeconds(currWeapon2.reloadTime);
        currWeapon2.bullets = currWeapon2.maxBullets;

        isReloading2 = false;
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = currGraphics.GetComponent<Animator>();
        if(anim != null)
        {
            anim.SetTrigger("Reload");
        }
    }

    [Command]
    void CmdOnReload2()
    {
        RpcOnReload2();
    }

    [ClientRpc]
    void RpcOnReload2()
    {
        Animator anim2 = currGraphics2.GetComponent<Animator>();
        if (anim2 != null)
        {
            anim2.SetTrigger("Reload2");
        }
    }
}

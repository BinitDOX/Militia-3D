using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "AK47";

    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0f;
    public int maxBullets = 30;
    [HideInInspector]
    public int bullets = 30;
    public float reloadTime = 1.4f;

    public GameObject graphics;

    //public PlayerWeapon()
    //{
    //    bullets = maxBullets;
    //}
}

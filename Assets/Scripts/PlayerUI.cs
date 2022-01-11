using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFAmt;

    [SerializeField]
    Slider healthSlider;
    [SerializeField]
    Color fullHealthColor = Color.green;
    [SerializeField]
    Color zeroHealthColor = Color.red;
    [SerializeField]
    Image fillHealthImage;

    private PlayerManager player;
    private PlayerControllerUCJoy controller;
    private WeaponManager weaponManager;

    [SerializeField]
    Button scoreButton;
    [SerializeField]
    GameObject scoreBoard;

    [SerializeField]
    Text ammoText;
    [SerializeField]
    Text ammoText2;

    void Start()
    {
        EventTrigger eventTrigger = scoreButton.gameObject.AddComponent<EventTrigger>();

        // Pointer down
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { ShowScoreOn(); });
        eventTrigger.triggers.Add(pointerDownEntry);

        // Pointer up
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { ShowScoreOff(); });
        eventTrigger.triggers.Add(pointerUpEntry);

    }

    //public void SetController(PlayerControllerUCJoy cont)
    //{
    //    controller = cont;
    //}

    public void SetPlayer(PlayerManager plyr)
    {
        player = plyr;
        controller = player.GetComponent<PlayerControllerUCJoy>();
        weaponManager = player.GetComponent<WeaponManager>();
    }

    void Update()
    {
        SetFuelAmt(controller.GetThrusterFuelAmt());
        SetHealthAmt(player.currentHealth);
        SetAmmoAmt(weaponManager.GetCurrentWeapon().bullets);
        SetAmmoAmt2(weaponManager.GetCurrentWeapon2().bullets);
    }

    void SetFuelAmt(float amt)
    {
        thrusterFuelFAmt.localScale = new Vector3(1f, amt, 1f);
    }

    void ShowScoreOn()
    {
        scoreBoard.SetActive(true);
    }

    void ShowScoreOff()
    {
        scoreBoard.SetActive(false);

    }

    void SetHealthAmt(float amt)
    {
        healthSlider.value = amt;
        fillHealthImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, amt / player.maxHealth);
    }

    void SetAmmoAmt(int amt)
    {
        ammoText.text = amt.ToString();
    }
    void SetAmmoAmt2(int amt)
    {
        ammoText2.text = amt.ToString();
    }
}

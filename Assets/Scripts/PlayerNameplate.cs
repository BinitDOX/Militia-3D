using UnityEngine;
using UnityEngine.UI;

public class PlayerNameplate : MonoBehaviour
{
    [SerializeField]
    private Text userNameText;
    [SerializeField]
    private PlayerManager player;

    [SerializeField]
    Slider healthSlider;
    [SerializeField]
    Color fullHealthColor = Color.green;
    [SerializeField]
    Color zeroHealthColor = Color.red;
    [SerializeField]
    Image fillHealthImage;

    void Update()
    {
        userNameText.text = player.userName;
        SetHealthAmt(player.currentHealth);
    }

    void SetHealthAmt(float amt)
    {
        healthSlider.value = amt;
        fillHealthImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, amt / player.maxHealth);
    }
}

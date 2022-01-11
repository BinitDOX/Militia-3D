using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreBoardItem : MonoBehaviour
{
    [SerializeField]
    Text usernameText;
    [SerializeField]
    Text killsText;
    [SerializeField]
    Text deathsText;
    //[SerializeField]
    //Image image;

    public void Setup(string username, int kills, int deaths)//, Color color)
    {
        usernameText.text = username;
        killsText.text = "Kills: " + kills;
        deathsText.text = "Deaths: " + deaths;
        //image = GetComponent<Image>();
        //image.color = color;
    }

}

using UnityEngine;
using UnityEngine.UI;

public class UserAccLobby : MonoBehaviour
{
    public Text usernameText;

    void Start()
    {
        if(UserAccManager.IsLoggedIn)
            usernameText.text = UserAccManager.PlayerUsername;
    }

    public void LogOutLobby()
    {
        if (UserAccManager.IsLoggedIn)
            UserAccManager.instance.LogOut();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DatabaseControl;
using UnityEngine.SceneManagement;

public class UserAccManager : MonoBehaviour
{
    public static UserAccManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    //These store the username and password of the player when they have logged in
    public static string PlayerUsername { get; protected set; }
    private static string PlayerPassword = "";
    //public static string LoggedInData { get; protected set; }

    public static bool IsLoggedIn { get; protected set; }

    public string loginScene = "LobbyScene";
    public string logoutScene = "LoginScene";

    //You can use these/this function call back(s) to call a funtion in local script when you exec a funtion from here using another script..GIT GUD!
    //We use this cuz it takes time to get data from server.. and if we just called a func to manipulate the data.. it wont work cuz data still not recieved yet
    public delegate void OnDataReceivedCallback(string data); 

    public void LogOut()
    {
        PlayerUsername = "";
        PlayerPassword = "";

        IsLoggedIn = false;

        Debug.Log("User logged out!");

        SceneManager.LoadScene(logoutScene);
        Destroy(GameObject.Find("LobbyManager"));
    }

    public void LogIn(string username, string password)
    {
        PlayerUsername = username;
        PlayerPassword = password;

        IsLoggedIn = true;

        Debug.Log("Logged in as " + PlayerUsername);

        SceneManager.LoadScene(loginScene);
    }

    IEnumerator GetData(OnDataReceivedCallback onDataReceived)
    {
        string data = "ERROR";

        IEnumerator e = DCF.GetUserData(PlayerUsername, PlayerPassword); // << Send request to get the player's data string. Provides the username and password
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Error")
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
            //ResetAllUIElements();
            //playerUsername = "";
            //playerPassword = "";
            //loginParent.gameObject.SetActive(true);
            //loadingParent.gameObject.SetActive(false);
            //Login_ErrorText.text = "Error: Unknown Error. Please try again later.";
        }
        else
        {
            //The player's data was retrieved. Goes back to loggedIn UI and displays the retrieved data in the InputField
            //loadingParent.gameObject.SetActive(false);
            //loggedInParent.gameObject.SetActive(true);
            //LoggedIn_DataOutputField.text = response;
            string DataRecieved = response;
            data = DataRecieved;
        }
        if(onDataReceived != null)
            onDataReceived.Invoke(data);
    }
    IEnumerator SetData(string data)
    {
        IEnumerator e = DCF.SetUserData(PlayerUsername, PlayerPassword, data); // << Send request to set the player's data string. Provides the username, password and new data string
        while (e.MoveNext())
        {
            yield return e.Current;
        }
        string response = e.Current as string; // << The returned string from the request

        if (response == "Success")
        {
            //The data string was set correctly. Goes back to LoggedIn UI
            //loadingParent.gameObject.SetActive(false);
            //loggedInParent.gameObject.SetActive(true);
        }
        else
        {
            //There was another error. Automatically logs player out. This error message should never appear, but is here just in case.
            //ResetAllUIElements();
            //playerUsername = "";
            //playerPassword = "";
            //loginParent.gameObject.SetActive(true);
            //loadingParent.gameObject.SetActive(false);
            //Login_ErrorText.text = "Error: Unknown Error. Please try again later.";
        }
    }

    public void SendDataBtn(string data)
    {
        if (IsLoggedIn)
        {
            //Called when the player hits 'Set Data' to change the data string on their account. Switches UI to 'Loading...' and starts coroutine to set the players data string on the server
            //loadingParent.gameObject.SetActive(true);
            //loggedInParent.gameObject.SetActive(false);
            StartCoroutine(SetData(data));
        }
    }
    public void GetDataBtn(OnDataReceivedCallback onDataReceived)
    {
        if (IsLoggedIn)
        {
            //Called when the player hits 'Get Data' to retrieve the data string on their account. Switches UI to 'Loading...' and starts coroutine to get the players data string from the server
            //loadingParent.gameObject.SetActive(true);
            //loggedInParent.gameObject.SetActive(false);
            StartCoroutine(GetData(onDataReceived));
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public Text killCount;
    public Text deathCount;

    void Start()
    {
        Refresh();
    }

    void OnReceivedData(string data)
    {
        if (killCount != null || deathCount != null)
        {
            killCount.text = "KILLS:  " + UserAccDataTrans.ExtractKills(data).ToString();
            deathCount.text = "DEATHS:  " + UserAccDataTrans.ExtractDeaths(data).ToString();
        }
    }

    public void Refresh()
    {
        if (UserAccManager.IsLoggedIn)
            UserAccManager.instance.GetDataBtn(OnReceivedData);
    }

}

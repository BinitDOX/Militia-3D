using System;
using UnityEngine;

public class UserAccDataTrans : MonoBehaviour
{
    private static string KILLS_TAG = "[KILLS]";
    private static string DEATHS_TAG = "[DEATHS]";


    public static string ImportData(int kills, int deaths)
    {
        return KILLS_TAG + kills + "/" + DEATHS_TAG + deaths;
    }

    public static int ExtractKills(string data)
    {
        return int.Parse(ExtractData(data, KILLS_TAG));
    }

    public static int ExtractDeaths(string data)
    {
        return int.Parse(ExtractData(data, DEATHS_TAG));
    }

    private static string ExtractData (string data, string symbol)
    {
        string[] pieces = data.Split('/');
        foreach (string piece in pieces)
        {
            if (piece.StartsWith(symbol))
            {
                return piece.Substring(symbol.Length);
            }
        }
        Debug.LogError("In data extraction");
        return "";
    }

}

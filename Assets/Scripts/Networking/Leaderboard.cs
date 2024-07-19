using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Leaderboard : MonoBehaviour
{

    public TMP_Text playerNameText, killsText, deathsText, moneyText, rankText; 

    public void SetDetails(string name, int kills, int deaths, int money,int rank){

        playerNameText.text = name; 
        killsText.text = kills.ToString(); 
        deathsText.text = deaths.ToString(); 
        moneyText.text = money.ToString();
        rankText.text = rank.ToString();
        



    }
}
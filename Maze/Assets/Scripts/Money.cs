using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    [SerializeField] private TMP_Text Coins, Plus;
    [SerializeField] List<Animator> Anims;
    private int countMoney;
    
    void Start()
    {
        countMoney = PlayerPrefs.GetInt("Coins");
        Coins.text = countMoney.ToString();
    }

    public void MoneyPlus(int value){
        countMoney += value;
        Coins.text = countMoney.ToString();
        Plus.text = $"+ {value}";
        foreach (Animator Anim in Anims){
            Anim.SetTrigger("MoneyPlus");
            }
        PlayerPrefs.SetInt("Coins", countMoney);
        PlayerPrefs.Save();
    }

    public bool Buy(int price){
        if (price < countMoney){
            countMoney -= price;
            Coins.text = countMoney.ToString();
            PlayerPrefs.SetInt("Coins", countMoney);
            PlayerPrefs.Save();
            return true;
        }
        else{
            return false;
        }
    }

}

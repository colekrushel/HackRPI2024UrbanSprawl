using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CashHandler : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI curTxt;

    [SerializeField]
    string preText;

    [SerializeField]
    int cash;

    int Cash => cash;


    void UpdateCash(int amount) {
        cash = amount;
        curTxt.text = preText + cash;
    }

    public void AddCash(int amount) {
        UpdateCash(cash + amount);
    }

    public bool RemoveCashNoDebt(int amount) {
        amount = cash - amount;
        if (amount >= 0) {
            UpdateCash(amount);
            return true;
        }
        else {
            return false;
        }

    }
}

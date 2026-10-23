using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class UpgradeUI
{
    public UpgradeType type;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI levelText;
}

[Serializable]
public class ItemUI
{
    public string name;
    public TextMeshProUGUI costText;
}

public class Computer : MonoBehaviour
{
    [SerializeField] private List<UpgradeUI> upgradesUI = new List<UpgradeUI>();
    [SerializeField] private List<ItemUI> itemsUI = new List<ItemUI>();
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI yearText;
    [SerializeField] public Transform cameraPOV;

    [SerializeField] private GameObject browserPage;
    [SerializeField] private GameObject shopPage;

    void OnEnable()
    {
        GameManager.OnUpgradePurchased += UpdateUpgradeUI;
        GameManager.OnMoneyAddedCallback += UpdateMoneyUI;
        GameManager.OnEpochChangedCallback += UpdateEpochUI;
    }

    void OnDisable()
    {
        GameManager.OnUpgradePurchased -= UpdateUpgradeUI;
        GameManager.OnMoneyAddedCallback -= UpdateMoneyUI;
        GameManager.OnEpochChangedCallback -= UpdateEpochUI;
    }

    void Start()
    {
        UpdateMoneyUI(0);
        UpdateEpochUI(0);
    }

    void Update()
    {
        yearText.text = $"Year: {(int)GameManager.Singletone.year}";
    }

    void UpdateUpgradeUI(UpgradeType type, int newLevel)
    {
        UpgradeUI updatedUpgrade = upgradesUI.Find(vr => vr.type == type);
        updatedUpgrade.costText.text = $"Cost: ${GameManager.Singletone.GetUpgradeCost(type)}";
        updatedUpgrade.levelText.text = $"Level: {newLevel}";
    }

    void UpdateItemUI(string name)
    {
        ItemUI updateditem = itemsUI.Find(vr => vr.name == name);
        updateditem.costText.text = $"Cost: ${GameManager.Singletone.GetItemCost(name)}";
    }

    void UpdateMoneyUI(int amount)
    {
        int money = GameManager.Singletone.GetCurrentMoney();
        moneyText.text = $"Balance: ${money}";
    }

    void UpdateEpochUI(int index)
    {
        
    }

    public void ToggleBrowserWindow(bool state)
    {
        browserPage.SetActive(state);
    }

    public void ToggleShopWindow(bool state)
    {
        shopPage.SetActive(state);
    }
}

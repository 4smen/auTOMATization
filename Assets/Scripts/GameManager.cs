using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpgradeType
{
    RobotSpeed,

    TomatoGrowthRate,
    TomatoWateringCycle,
    TomatoStartSize,
    TomatoHeal,
    TomatoBurnRate,
    TomatoMax,

    CostOfTomato,
    Discounts,

    GardenBeds,
    WaterStations,
    Defense,

    AutoWatering,
    AutoDoorHelper,
    AutoShredding,
}

[Serializable]
public class EpochData
{
    public string epochName;
    public float startYear;
    public int tomatoPrice;
    public float burnRate;
    public GameObject environmentPreset;
}

[Serializable]
public class UpgradeItem
{
    public string upgradeName;
    public UpgradeType type;
    public int maxLevel = 5;
    public int currentLevel = 0;
    public int baseCost = 50;
    public float costMultiplier = 1.5f;

    public int GetCurrentCost()
    {
        int rawCost = Mathf.RoundToInt(baseCost * Mathf.Pow(costMultiplier, currentLevel));
        if (GameManager.Singletone.GetUpgradeLevel(UpgradeType.Discounts) > 0)
        {
            int discountLevel = GameManager.Singletone.GetUpgradeLevel(UpgradeType.Discounts);
            float discountPercent = discountLevel * 0.05f;
            rawCost = Mathf.RoundToInt(rawCost * (1f - discountPercent));
        }
        return rawCost;
    }

    public bool IsMaxed()
    {
        return currentLevel >= maxLevel;
    }
}

[Serializable]
public class ShopItem
{
    public string name;
    public int cost;

    public int GetCurrentCost()
    {
        if (GameManager.Singletone.GetUpgradeLevel(UpgradeType.Discounts) > 0)
        {
            int discountLevel = GameManager.Singletone.GetUpgradeLevel(UpgradeType.Discounts);
            float discountPercent = discountLevel * 0.05f;
            cost = Mathf.RoundToInt(cost * (1f - discountPercent));
        }
        return cost;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singletone;

    [Header("Time")]
    public float year;
    public float yearRate = 1f;
    int currentEpochIndex = -1;
    EpochData currentEpoch;
    [SerializeField] public List<EpochData> epochs = new List<EpochData>();
    [Header("Upgrades & Items")]
    [SerializeField] private List<UpgradeItem> upgrades = new List<UpgradeItem>();
    [SerializeField] private List<ShopItem> items = new List<ShopItem>();
    [Header("Tomato")]
    public float currentTomato;
    public float tomatoHeal = 20f;
    public float maxTomato = 100f;
    public float burnRate = 2f;
    [Header("Money")]
    public int money;
    [Header("Links")]
    [SerializeField] private Image tomatoBar;
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private bool tutorial = true;

    public static event Action<string> OnItemPurchased;
    public static event Action<UpgradeType, int> OnUpgradePurchased;

    public static event Action<int> OnEpochChangedCallback;
    public static event Action<int> OnMoneyAddedCallback;

    public static event Action OnTomatoEatenCallback;
    public static event Action OnTomatoSeedsPlantedCallback;
    public static event Action OnTomatoPlantWateredCallback;

    public static event Action OnDeathCallback;

    void OnEnable()
    {
        OnUpgradePurchased += OnUpgradeReceived;
    }

    void OnDisable()
    {
        OnUpgradePurchased -= OnUpgradeReceived;
    }

    void Awake()
    {
        if (Singletone != null) Destroy(this);
        else Singletone = this;
    }

    void Start()
    {
        currentTomato = maxTomato;
        StartCoroutine(Tutorial());
        UpdateEpochCheck();
        UpdateUI();
    }

    void Update()
    {
        if (tutorial) return;

        year += yearRate * Time.deltaTime;
        UpdateEpochCheck();

        currentTomato -= burnRate * Time.deltaTime;
        if (currentTomato <= 0)
        {
            OnDeathCallback?.Invoke();
        }

        UpdateUI();
    }

    public void TryBuyItem(string itemName)
    {
        ShopItem item = items.Find(vr => vr.name == itemName);

        if (item == null)
        {
            Debug.LogError($"фигни под названием {itemName} нет, иди ты уже");
            return;
        }

        if (money >= item.cost)
        {
            ChangeMoney(-item.cost);

            OnItemPurchased?.Invoke(itemName);

            print($"заимел {item.name}");
        }
        else
        {
            print("мало бачей");
        }
    }

    public void TryBuyUpgrade(string upgradeName)
    {
        UpgradeItem item = upgrades.Find(vr => vr.upgradeName == upgradeName);

        if (item == null)
        {
            Debug.LogError($"фигни под названием {upgradeName} нет, иди ты уже");
            return;
        }

        if (item.currentLevel >= item.maxLevel)
        {
            print($"{item.type} уже вмакшен, борщишь");
            return;
        }

        int cost = item.GetCurrentCost();

        if (money >= cost)
        {
            ChangeMoney(-cost);
            item.currentLevel++;

            OnUpgradePurchased?.Invoke(item.type, item.currentLevel);

            print($"заимел {item.type}, уровень этой бурмалды: {item.currentLevel}. В след. раз должен мне {item.GetCurrentCost()}");
        }
        else
        {
            print("мало бачей");
        }
    }

    public void ChangeMoney(int amount = 0, string itemName = "")
    {
        if (itemName != "")
        {
            amount = items.Find(vr => itemName.StartsWith(vr.name)).cost;
            print(items.Find(vr => itemName.StartsWith(vr.name)).name);
        }
        money += amount;
        print($"яндекс плюс оплачен за {amount}");
        OnMoneyAddedCallback?.Invoke(amount);
    }

    public void EatTomato()
    {
        tomatoHeal = tomatoHeal + (GetUpgradeLevel(UpgradeType.TomatoHeal) * 5f);
        maxTomato = maxTomato + (GetUpgradeLevel(UpgradeType.TomatoMax) * 20f);

        float amount = tomatoHeal * (1 - Mathf.Pow(currentTomato / maxTomato, 2f));
        currentTomato += amount;
        currentTomato = Mathf.Clamp(currentTomato, 0f, maxTomato);
        UpdateUI();

        print($"залетел вкусненький на {amount}");
    }

    void ChangeEpoch(int newIndex)
    {
        currentEpochIndex = newIndex;
        currentEpoch = epochs[currentEpochIndex];

        items[0].cost = currentEpoch.tomatoPrice + (GetUpgradeLevel(UpgradeType.CostOfTomato) * 3);
        burnRate = currentEpoch.burnRate;

        Debug.Log($"эпохность: {currentEpoch.epochName}");

        for (int i = 0; i < epochs.Count; i++)
        {
            if (epochs[i].environmentPreset != null)
            {
                epochs[i].environmentPreset.SetActive(i == currentEpochIndex);
            }
        }

        OnEpochChangedCallback?.Invoke(currentEpochIndex);
    }

    void UpdateEpochCheck()
    {
        int targetEpochIndex = currentEpochIndex;

        for (int i = 0; i < epochs.Count; i++)
        {
            if (year >= epochs[i].startYear)
            {
                targetEpochIndex = i;
            }
        }

        if (targetEpochIndex != currentEpochIndex && targetEpochIndex >= 0)
        {
            ChangeEpoch(targetEpochIndex);
        }
    }

    void OnUpgradeReceived(UpgradeType type, int newLevel)
    {
        switch (type)
        {
            case UpgradeType.CostOfTomato:
                items[0].cost = items[0].cost + (newLevel * 5);
                break;
            case UpgradeType.TomatoBurnRate:
                burnRate = burnRate - (newLevel * 0.1f);
                break;
        }
    }

    public int GetUpgradeLevel(UpgradeType type)
    {
        UpgradeItem item = upgrades.Find(vr => vr.type == type);
        return item != null ? item.currentLevel : 0;
    }

    public int GetUpgradeCost(UpgradeType type)
    {
        UpgradeItem item = upgrades.Find(vr => vr.type == type);
        return item != null ? item.GetCurrentCost() : 0;
    }

    public int GetItemCost(string name)
    {
        ShopItem item = items.Find(vr => vr.name == name);
        return item != null ? item.GetCurrentCost() : 0;
    }

    public float GetCurrentTomatoPercent() { return currentTomato / maxTomato; }

    public int GetCurrentMoney() { return money; }

    void UpdateUI()
    {
        tomatoBar.fillAmount = currentTomato / maxTomato;
    }

    IEnumerator Tutorial()
    {
        yield return new WaitForSeconds(120f);
        tutorialCanvas.SetActive(false);
        tutorial = false;
    }

    public void SkipTutorial()
    {
        tutorialCanvas.SetActive(false);
        tutorial = false;
    }

    public void InvokeOnTomatoEatenCallback() => OnTomatoEatenCallback?.Invoke();
    public void InvokeOnTomatoPlantWateredCallback() => OnTomatoPlantWateredCallback?.Invoke();
    public void InvokeOnTomatoSeedsPlantedCallback() => OnTomatoSeedsPlantedCallback?.Invoke();
}

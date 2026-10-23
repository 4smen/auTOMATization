using System.Collections.Generic;
using UnityEngine;

public class HouseManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> gardenBeds = new List<GameObject>();
    [SerializeField] private GameObject waterStationNew;
    [SerializeField] private Transform deliverySpawn;

    void OnEnable()
    {
        GameManager.OnItemPurchased += DeliverItem;

        GameManager.OnUpgradePurchased += OnUpgradeReceived;
    }

    void OnDisable()
    {
        GameManager.OnItemPurchased -= DeliverItem;

        GameManager.OnUpgradePurchased -= OnUpgradeReceived;
    }

    void DeliverItem(string itemName)
    {
        GameObject item = itemPrefabs.Find(vr => vr.name == itemName);
        Instantiate(item, deliverySpawn.position, Quaternion.identity);
        print("я никогда не злюсь на доставщика");
    }

    void OnUpgradeReceived(UpgradeType type, int newLevel)
    {
        switch (type)
        {
            case UpgradeType.TomatoGrowthRate:
                foreach (var gardenBed in gardenBeds)
                {
                    var bed = gardenBed.GetComponent<GardenBed>();
                    bed.growRate = bed.growRate + (newLevel * 0.5f);
                }
                break;
            case UpgradeType.TomatoWateringCycle:
                foreach (var gardenBed in gardenBeds)
                {
                    var bed = gardenBed.GetComponent<GardenBed>();
                    bed.wateringDuration = bed.wateringDuration + (newLevel * 1.5f);
                }
                break;
            case UpgradeType.TomatoStartSize:
                foreach (var gardenBed in gardenBeds)
                {
                    var bed = gardenBed.GetComponent<GardenBed>();
                    bed.startSize = new Vector3(bed.startSize.x + (newLevel * 0.05f), bed.startSize.y + (newLevel * 0.05f), bed.startSize.z + (newLevel * 0.05f));
                    bed.startAge = newLevel * (bed.maxAge * 0.05f);
                }
                break;
            case UpgradeType.AutoWatering:
                foreach (var gardenBed in gardenBeds) gardenBed.GetComponent<GardenBed>().autoWatering = true;
                break;
            case UpgradeType.GardenBeds:
                gardenBeds[newLevel - 1].SetActive(true);
                print("новая хрядка");
                break;
            case UpgradeType.WaterStations:
                waterStationNew.SetActive(true);
                print("новая водычка");
                break;
        }
    }
}

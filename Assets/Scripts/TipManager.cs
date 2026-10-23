using UnityEngine;

public class TipManager : MonoBehaviour
{
    [SerializeField] private GameObject tomatoTips;
    [SerializeField] private GameObject tomatoSeedsTips;
    [SerializeField] private GameObject wateringCanTips;

    public void UpdateTips(string name)
    {
        if (name.StartsWith("Tomato"))
        {
            tomatoTips.SetActive(true);
            tomatoSeedsTips.SetActive(false);
            wateringCanTips.SetActive(false);
        }
        else if (name.StartsWith("SeedsOfTomato"))
        {
            tomatoTips.SetActive(false);
            tomatoSeedsTips.SetActive(true);
            wateringCanTips.SetActive(false);
        }
        else if (name.StartsWith("WateringCan"))
        {
            tomatoTips.SetActive(false);
            tomatoSeedsTips.SetActive(false);
            wateringCanTips.SetActive(true);
        }
        else
        {
            tomatoTips.SetActive(false);
            tomatoSeedsTips.SetActive(false);
            wateringCanTips.SetActive(false);
        }
    }
}

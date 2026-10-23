using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GardenBed : MonoBehaviour
{
    [SerializeField] private List<GameObject> tomatoes = new List<GameObject>();
    [SerializeField] private Transform plantOrigin;
    [SerializeField] private GameObject plant;
    [SerializeField] public GameObject wateringWarning;

    public int health = 0;
    public float age = 0f;
    public float startAge = 0f;
    public float maxAge = 50f;
    public float growRate = 0.5f;
    public Vector3 startSize = new Vector3(0.1f, 0.1f, 0.1f);

    public bool watered;
    public float wateringDuration = 5f;
    public bool autoWatering = false;
    public bool growing;

    [SerializeField] private GameObject plantPrefab;

    void Update()
    {
        if (watered && growing || autoWatering && growing)
        {
            age += growRate * Time.deltaTime;

            float percentageGrowth = age / maxAge;
            float smoothGrowthPercentage = Mathf.SmoothStep(0f, 1f, percentageGrowth);

            plantOrigin.localScale = Vector3.Lerp(startSize, new Vector3(1f, 1f, 1f), smoothGrowthPercentage);
        }
        if (age >= maxAge && growing)
        {
            EndGrowth();
        }
        if (health <= 0) DestroyPlant();
    }

    public void StartGrowth()
    {
        if (plant != null) return;

        plant = Instantiate(plantPrefab, plantOrigin.position, Quaternion.identity);
        plant.transform.parent = plantOrigin;
        plantOrigin.localScale = startSize;

        health = 3;
        age = startAge;
        growing = true;

        wateringWarning.SetActive(true);

        for (int i = 0; i < plant.transform.childCount; i++)
        {
            var child = plant.transform.GetChild(i);
            if (child.name.Contains("Tomato"))
            {
                tomatoes.Add(child.gameObject);
            }
        }
    }

    void EndGrowth()
    {
        growing = false;

        foreach (var tomato in tomatoes)
        {
            tomato.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void DestroyPlant()
    {
        Destroy(plant);
        wateringWarning.SetActive(false);
        age = 0f;
    }

    public IEnumerator WateringCycle()
    {
        yield return new WaitForSeconds(wateringDuration);
        if (growing == true)
        {
            watered = false;
            wateringWarning.SetActive(true);
        }
    }
}

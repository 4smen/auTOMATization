using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    [SerializeField] private float interactRange;
    [SerializeField] private float pickDuration;
    [SerializeField] private Transform grabPoint;
    [SerializeField] private LayerMask layersToInteract;

    [SerializeField] private bool isHoldingItem;
    [SerializeField] private bool isComputer;
    [SerializeField] private bool isMonitor;

    [SerializeField] private float wateringCanValue = 1f;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCam playerCam;
    [SerializeField] private PlayerMovement2D playerMovement2D;
    [SerializeField] private TipManager tipManager;

    [SerializeField] private GameObject monitor;
    [SerializeField] private Image wateringCanValueBar;

    [SerializeField] private GameObject item;
    [SerializeField] private TriggerProxy triggerProxy;
    [SerializeField] private GameObject targetedGardenBed;

    void Update()
    {
        if (Input.GetKeyDown(Controls.Singletone.interactKey))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray.origin, ray.direction, out hit, interactRange, layersToInteract))
            {
                print(hit.collider.gameObject.name);
                InteractHadler(hit.collider.gameObject);
            }
        }
        if (Input.GetKeyDown(Controls.Singletone.useKey) && isHoldingItem) UseHandler();
        if (Input.GetKeyDown(Controls.Singletone.dropKey) && isHoldingItem)
        {
            DropItem();
            tipManager.UpdateTips("");
        }
        if (Input.GetKeyDown(Controls.Singletone.monitorKey) && !isComputer && !isHoldingItem) ToggleMonitor();
        if (Input.GetKeyDown(Controls.Singletone.skipTutorialKey)) GameManager.Singletone.SkipTutorial();
    }

    void InteractHadler(GameObject hit)
    {
        switch (hit.tag)
        {
            case "Item":
                if (!isHoldingItem)
                {
                    item = hit;
                    if (item.GetComponentInChildren<TriggerProxy>()) 
                    {
                        triggerProxy = item.GetComponentInChildren<TriggerProxy>();
                        TriggerProxySubscribe();
                    }
                    if (item.transform.GetComponentInParent<GardenBed>())
                    {
                        item.transform.GetComponentInParent<GardenBed>().health--;
                    }
                    tipManager.UpdateTips(item.name);
                    GrabItem();
                }
                break;
            case "Sell":
                if (isHoldingItem)
                {
                    GameManager.Singletone.ChangeMoney(0, item.name);
                    DestroyItem();
                }
                break;
            case "GardenBed":
                if (isHoldingItem && item.name.StartsWith("SeedsOfTomato"))
                {
                    hit.GetComponent<GardenBed>().StartGrowth();
                    GameManager.Singletone.InvokeOnTomatoSeedsPlantedCallback();
                    DestroyItem();
                }
                break;
            case "Computer":
                ToggleComputer(hit.gameObject);
                break;
            case "Water":
                if (isHoldingItem && item.name.StartsWith("WateringCan"))
                {
                    wateringCanValue = 1f;
                    wateringCanValueBar.fillAmount = wateringCanValue;
                }
                break;
        }
    }

    void UseHandler()
    {
        if (item.name.StartsWith("Tomato"))
        {
            GameManager.Singletone.InvokeOnTomatoEatenCallback();
            DestroyItem();
        }
        else if (item.name.StartsWith("WateringCan") && wateringCanValue >= 0)
        {
            var gardenBed = targetedGardenBed.GetComponent<GardenBed>();
            gardenBed.watered = true;
            gardenBed.wateringWarning.SetActive(false);
            gardenBed.StartCoroutine(gardenBed.WateringCycle());

            wateringCanValue -= 0.2f;
            wateringCanValueBar.fillAmount = wateringCanValue;

            GameManager.Singletone.InvokeOnTomatoPlantWateredCallback();
        }
    }

    void TriggerProxySubscribe()
    {
        triggerProxy.OnCustomTriggerEnterCallback += AddTargetedGardenBed;
        triggerProxy.OnCustomTriggerExitCallback += RemoveTargetedGardenBed;
    }

    void TriggerProxyUnsubscribe()
    {
        triggerProxy.OnCustomTriggerEnterCallback -= AddTargetedGardenBed;
        triggerProxy.OnCustomTriggerExitCallback -= RemoveTargetedGardenBed;
    }

    void AddTargetedGardenBed(Collider target)
    {
        if (target.CompareTag("GardenBed"))
        {
            targetedGardenBed = target.gameObject;
            print("грядка попалась");
        }
    }

    void RemoveTargetedGardenBed(Collider target)
    {
        if (target.CompareTag("GardenBed"))
        {
            targetedGardenBed = null;
            print("грядка пропала");
        }
    }

    void DestroyItem()
    {
        Destroy(item);
        item = null;
        isHoldingItem = false;
    }

    void GrabItem()
    {
        isHoldingItem = true;

        item.GetComponent<BoxCollider>().enabled = false;
        var rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.freezeRotation = true;
        item.transform.parent = grabPoint;

        if (item.name.StartsWith("WateringCan"))
        {
            wateringCanValueBar = item.GetComponent<WateringCan>().valueBar;
            wateringCanValueBar.fillAmount = wateringCanValue;
        }

        StartCoroutine(SmoothMove(item.transform, grabPoint, pickDuration));
    }

    void DropItem()
    {
        if (triggerProxy != null)
        {
            TriggerProxyUnsubscribe();
            triggerProxy = null;
        }

        isHoldingItem = false;

        item.transform.parent = null;
        item.GetComponent<BoxCollider>().enabled = true;
        var rb = item.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.freezeRotation = false;

        if (wateringCanValueBar != null) wateringCanValueBar = null;

        item = null;
    }

    void ToggleMonitor()
    {
        isMonitor = !isMonitor;

        playerMovement.enabled = !isMonitor;
        playerCam.enabled = !isMonitor;
        playerMovement2D.enabled = isMonitor;
        monitor.SetActive(isMonitor);

        Cursor.visible = isMonitor;
        if (isMonitor) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    void ToggleComputer(GameObject computer)
    {
        isComputer = !isComputer;

        if (isComputer)
        {
            playerMovement.enabled = false;
            playerCam.enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            playerCam.cam.transform.parent = null;
            StartCoroutine(SmoothMove(playerCam.cam.transform, computer.GetComponentInParent<Computer>().cameraPOV, 0.5f));
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            StartCoroutine(SmoothMove(playerCam.cam.transform, playerCam.camHolder, 0.5f));
            playerCam.cam.transform.parent = playerCam.camHolder;

            playerMovement.enabled = true;
            playerCam.enabled = true;
        }
    }

    IEnumerator SmoothMove(Transform target, Transform destination, float duration) 
    {
        Vector3 startPosition = target.position;
        Quaternion startRotation = target.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float percentageComplete = elapsedTime / duration;
            float smoothPercentage = Mathf.SmoothStep(0f, 1f, percentageComplete);

            target.position = Vector3.Lerp(startPosition, destination.position, smoothPercentage);
            target.rotation = Quaternion.Lerp(startRotation, destination.rotation, smoothPercentage);

            yield return null;
        }

        target.position = destination.position;
    }
}

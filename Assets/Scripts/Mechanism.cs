using UnityEngine;
using UnityEngine.UI;

public class Mechanism : MonoBehaviour
{
    [SerializeField] private Transform tomatoSpawner;
    [SerializeField] private GameObject tomatoPrefab;

    [SerializeField] private float shreddingDuration = 5f;
    [SerializeField] private float shreddingProgress = 0f;
    [SerializeField] private float shreddingRate = 1f;
    [SerializeField] private bool shredderOpened = false;
    [SerializeField] private bool shredding = false;

    [SerializeField] private Material greenMat;
    [SerializeField] private Material redMat;
    [SerializeField] private Animator shredderAnimator;
    [SerializeField] private CollisionProxy shredderButtonProxy;
    [SerializeField] private CollisionProxy doorButtonProxy;
    [SerializeField] private PlayerMovement2D playerMovement2D;
    [SerializeField] private GameObject targetedTomato;

    void OnEnable()
    {
        shredderButtonProxy.OnCustomColliderEnterCallback += StartShredding;
        shredderButtonProxy.OnCustomColliderExitCallback += StopShredding;

        doorButtonProxy.OnCustomColliderEnterCallback += ToggleShredderDoor;

        GameManager.OnUpgradePurchased += OnUpgradeReceived;
        GameManager.OnTomatoEatenCallback += SpawnTomato;
    }

    void OnDisable()
    {
        shredderButtonProxy.OnCustomColliderEnterCallback -= StartShredding;
        shredderButtonProxy.OnCustomColliderExitCallback -= StopShredding;

        doorButtonProxy.OnCustomColliderEnterCallback -= ToggleShredderDoor;

        GameManager.OnUpgradePurchased += OnUpgradeReceived;
        GameManager.OnTomatoEatenCallback -= SpawnTomato;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tomato2D"))
        {
            targetedTomato = other.gameObject;
            if (GameManager.Singletone.GetUpgradeLevel(UpgradeType.AutoDoorHelper) > 0) ToggleShredderDoor(new Collision());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tomato2D"))
        {
            targetedTomato = null;
        }
    }

    void Update()
    {
        if (shredding)
        {
            shreddingProgress += shreddingRate * Time.deltaTime;
        }
        if (shreddingProgress >= shreddingDuration)
        {
            StopShredding(new Collision());
            ToggleShredderDoor(new Collision());

            shreddingProgress = 0;

            Destroy(targetedTomato);
            GameManager.Singletone.EatTomato();
        }
    }

    void OnUpgradeReceived(UpgradeType type, int newLevel)
    {
        switch (type)
        {
            case UpgradeType.RobotSpeed:
                playerMovement2D.moveSpeed = playerMovement2D.moveSpeed + (newLevel * 1.5f);
                break;
        }
    }

    void SpawnTomato()
    {
        var tomato = Instantiate(tomatoPrefab, tomatoSpawner.position, Quaternion.identity);
    }

    void StartShredding(Collision collision)
    {
        if (targetedTomato == null) return;

        shredding = true;
        shredderButtonProxy.GetComponent<MeshRenderer>().material = redMat;
        shredderAnimator.SetBool("shredding", true);
    }

    void StopShredding(Collision collision)
    {
        if (targetedTomato == null) return;

        shredding = false;
        shredderButtonProxy.GetComponent<MeshRenderer>().material = greenMat;
        shredderAnimator.SetBool("shredding", false);
    }

    void ToggleShredderDoor(Collision collision)
    {
        if (targetedTomato == null) return;

        shredderOpened = !shredderOpened;

        if (shredderOpened) doorButtonProxy.GetComponent<MeshRenderer>().material = greenMat;
        else doorButtonProxy.GetComponent<MeshRenderer>().material = redMat;

        shredderAnimator.SetBool("shredderOpened", shredderOpened);

        if (shredderOpened == false && GameManager.Singletone.GetUpgradeLevel(UpgradeType.AutoShredding) > 0) StartShredding(new Collision());
    }
}
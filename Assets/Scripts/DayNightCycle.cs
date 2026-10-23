using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] private float dayDurationInSeconds = 60f; // ƒлина полных суток в реальных секундах

    private float rotationSpeed;

    void Start()
    {
        // ¬ычисл€ем, на сколько градусов солнце должно повернутьс€ за одну секунду
        // (360 градусов / врем€ суток)
        rotationSpeed = 360f / dayDurationInSeconds;
    }

    void Update()
    {
        // ѕлавно вращаем солнце вокруг мировой оси X
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.World);
    }
}

using UnityEngine;

public class Tomato2D : MonoBehaviour
{
    [SerializeField] private float attractSpeed = 12f;

    private Transform targetFacePoint;
    [SerializeField] private Rigidbody rb;
    private bool isBeingAttracted = false;

    void FixedUpdate()
    {
        if (isBeingAttracted && targetFacePoint != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;

            // Плавно перемещаем томат к точке на лице робота
            Vector3 currentPos = rb.position;
            Vector3 targetPos = targetFacePoint.position;

            // Если игра в плоскости X-Y, удерживаем Z координату стабильной
            targetPos.z = currentPos.z;

            Vector3 newPos = Vector3.MoveTowards(currentPos, targetPos, attractSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            rb.linearVelocity = Vector3.zero;

            // Как только томат вплотную долетел до лица, «приклеиваем» его к роботу
            if (Vector3.Distance(currentPos, targetPos) < 0.1f)
            {
                AttachToRobotFace();
            }
        }
    }

    public void StartAttraction(Transform facePoint)
    {
        targetFacePoint = facePoint;
        isBeingAttracted = true;

        // Отключаем коллайдер, чтобы томат не толкал робота во время полета в лицо
        if (TryGetComponent<Collider>(out var col)) col.enabled = false;
    }

    private void AttachToRobotFace()
    {
        isBeingAttracted = false;

        // Жестко привязываем томат в иерархию к точке лица робота
        transform.SetParent(targetFacePoint, true);
        transform.localPosition = Vector3.zero; // Сбрасываем в локальный ноль точки лица

        // Сообщаем роботу, что предмет успешно доставлен на лицо
        targetFacePoint.GetComponentInParent<PlayerMovement2D>().OnTomatoAttached(gameObject);
    }

    public void Drop()
    {
        // 1. Отвязываем от лица робота обратно в корень сцены
        transform.SetParent(null);

        // 2. Возвращаем физическое поведение
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // Слегка толкаем томат вперед, чтобы он реалистично отлетел от бампера
            //rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }

        // 3. Возвращаем коллайдер, чтобы томат снова бился о стены и пол
        if (TryGetComponent<Collider>(out var col))
        {
            col.enabled = true;
        }
    }

}

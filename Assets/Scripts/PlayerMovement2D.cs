using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 8f;
    [SerializeField] public float reverseSpeed = 4f;
    [SerializeField] private float rotationSpeed = 180f;
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float viewAngle = 60f;
    [Header("Links")]
    [SerializeField] private Transform facePoint;
    Rigidbody rb;

    GameObject currentAttachedTomato;
    bool isHoldingTomato;
    float moveInput;
    float rotationInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        rotationInput = Input.GetAxis("Horizontal");

        // Проверяем нажатие клавиши взаимодействия (используем клавишу из вашего синглтона)
        if (Input.GetKeyDown(Controls.Singletone.interactKey))
        {
            if (!isHoldingTomato)
            {
                TryMagnetTomato();
            }
        }
        // НОВОЕ: Проверка сброса томата по кнопке Drop
        if (Input.GetKeyDown(Controls.Singletone.dropKey))
        {
            if (isHoldingTomato && currentAttachedTomato != null)
            {
                DropTomato();
            }
        }
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
    }

    private void HandleRotation()
    {
        float turn = rotationInput * rotationSpeed * Time.fixedDeltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        rb.MoveRotation(rb.rotation * turnRotation);
    }

    private void HandleMovement()
    {
        float currentSpeed = moveInput > 0 ? moveSpeed : reverseSpeed;

        Vector3 movement = transform.forward * moveInput * currentSpeed;

        movement.y = rb.linearVelocity.y;

        rb.linearVelocity = movement;
    }


    private void TryMagnetTomato()
    {
        // 1. Ищем все коллайдеры в радиусе вокруг робота
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, magnetRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<Tomato2D>(out var tomato))
            {
                // 2. Вычисляем направление от робота к томату
                Vector3 directionToTomato = (tomato.transform.position - transform.position).normalized;

                // 3. Получаем направление «вперед» для робота. 
                // В зависимости от ваших настроек это может быть transform.right (ось X) или transform.up (Y)
                Vector3 robotForward = transform.forward; // ПОМЕНЯЙТЕ на transform.up, если робот едет «вверх» по оси Y

                // Игнорируем Z координату для расчета угла в 2D
                directionToTomato.z = 0;
                robotForward.z = 0;

                // 4. Проверяем угол с помощью скалярного произведения (Dot Product)
                float dot = Vector3.Dot(robotForward, directionToTomato);
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                // Если томат находится в пределах угла обзора нашего «лица»
                if (angle <= viewAngle)
                {
                    isHoldingTomato = true;
                    tomato.StartAttraction(facePoint);
                    print($"заметили братанчика {hitCollider.name}");
                    break;
                }
            }
        }
    }

    public void OnTomatoAttached(GameObject tomatoObj)
    {
        currentAttachedTomato = tomatoObj;
        print("прихватил голубка");

        // Здесь вы можете активировать логику: "Теперь при удержании Ctrl/Space томат измельчается"
    }

    private void DropTomato()
    {
        // Получаем скрипт томата, который сейчас висит на лице
        if (currentAttachedTomato.TryGetComponent<Tomato2D>(out var tomato))
        {
            // Определяем направление плевка (куда смотрит лицо робота)
            // Напоминаю: если робот смотрит вверх, замените на transform.up
            Vector3 pushDirection = transform.right;

            // Вызываем метод сброса у томата
            tomato.Drop();
        }

        // Сбрасываем переменные робота, теперь он снова готов магнитить
        currentAttachedTomato = null;
        isHoldingTomato = false;
        print("Томат выброшен обратно на пол!");
    }

    // Публичный метод для шредера, чтобы уничтожить томат при переработке
    public void ConsumeTomato()
    {
        if (currentAttachedTomato != null)
        {
            Destroy(currentAttachedTomato);
            currentAttachedTomato = null;
            isHoldingTomato = false;
        }
    }
}

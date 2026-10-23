using UnityEngine;
using UnityEngine.UI;

public class ScanlineMovement : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float scrollSpeedY = 0.2f; // Скорость бега линий вниз

    void Awake()
    {
        if (rawImage == null) rawImage = GetComponent<RawImage>();
    }

    void Update()
    {
        if (rawImage == null) return;

        // Плавно сдвигаем UV-координаты текстуры по вертикали со временем
        Rect currentUV = rawImage.uvRect;
        currentUV.y += scrollSpeedY * Time.deltaTime;
        rawImage.uvRect = currentUV;
    }
}

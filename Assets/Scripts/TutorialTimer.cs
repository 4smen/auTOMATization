using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTimer : MonoBehaviour
{
    [SerializeField] private float totalTimeInSeconds = 120f;

    [SerializeField] private Image timerImageFill;

    private float currentTime = 120f;

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            float progress = currentTime / totalTimeInSeconds;
            timerImageFill.fillAmount = progress;
        }
        else
        {
            currentTime = 0;
            float progress = currentTime / totalTimeInSeconds;
            timerImageFill.fillAmount = progress;
            Destroy(this);
        }
    }
}

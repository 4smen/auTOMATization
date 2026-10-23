using UnityEngine;
using UnityEngine.UI;

public class PlayerEffectsManager : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float criticalThreshold = 0.35f;
    public bool critical = false;

    [SerializeField] private GameObject deathScreen;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerMovement2D playerMovement2D;
    [SerializeField] private PlayerCam playerCam;
    [SerializeField] private Interact interact;
    [SerializeField] private Mechanism mechanism;

    [SerializeField] private float baseMoveSpeed = 8f;
    [SerializeField] private float minMoveSpeed = 2f;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float maxFadeAlpha = 0.8f;

    void OnEnable()
    {
        GameManager.OnDeathCallback += Death;
    }

    void OnDisable()
    {
        GameManager.OnDeathCallback -= Death;
    }

    void Start()
    {
        baseMoveSpeed = playerMovement.moveSpeed;
    }

    void Update()
    {
        float currentTomatoPercent = GameManager.Singletone.GetCurrentTomatoPercent();

        if (currentTomatoPercent >= criticalThreshold)
        {
            if (critical) playerMovement.moveSpeed = baseMoveSpeed;
            critical = false;
            SetFadeAlpha(0f);
            return;
        }
        critical = true;

        float dangerFactor = 1f - (currentTomatoPercent / criticalThreshold);

        float smoothDanger = Mathf.SmoothStep(0f, 1f, dangerFactor);

        float targetSpeed = Mathf.Lerp(baseMoveSpeed, minMoveSpeed, smoothDanger);
        playerMovement.moveSpeed = targetSpeed;

        float targetAlpha = Mathf.Lerp(0f, maxFadeAlpha, smoothDanger);
        SetFadeAlpha(targetAlpha);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }

    void Death()
    {
        playerMovement.enabled = false;
        playerMovement2D.enabled = false;
        playerCam.enabled = false;
        interact.enabled = false;

        fadeImage.enabled = false;

        deathScreen.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}

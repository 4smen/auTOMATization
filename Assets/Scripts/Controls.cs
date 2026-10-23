using UnityEngine;

public class Controls : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.F;
    public KeyCode useKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode monitorKey = KeyCode.Tab;
    public KeyCode skipTutorialKey = KeyCode.M;
    public static Controls Singletone;

    void Awake()
    {
        if (Singletone != null) Destroy(this);
        else Singletone = this;
    }
}

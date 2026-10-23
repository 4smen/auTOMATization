using UnityEngine;
using System;

public class TriggerProxy : MonoBehaviour
{
    public event Action<Collider> OnCustomTriggerEnterCallback;
    public event Action<Collider> OnCustomTriggerStayCallback;
    public event Action<Collider> OnCustomTriggerExitCallback;

    private void OnTriggerEnter(Collider other)
    {
        OnCustomTriggerEnterCallback?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        OnCustomTriggerStayCallback?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnCustomTriggerExitCallback?.Invoke(other);
    }
}

using System;
using UnityEngine;

public class CollisionProxy : MonoBehaviour
{
    public event Action<Collision> OnCustomColliderEnterCallback;
    public event Action<Collision> OnCustomColliderStayCallback;
    public event Action<Collision> OnCustomColliderExitCallback;

    public void OnCollisionEnter(Collision other)
    {
        OnCustomColliderEnterCallback?.Invoke(other);
    }

    public void OnCollisionStay(Collision other)
    {
        OnCustomColliderStayCallback?.Invoke(other);
    }

    public void OnCollisionExit(Collision other)
    {
        OnCustomColliderExitCallback?.Invoke(other);
    }
}

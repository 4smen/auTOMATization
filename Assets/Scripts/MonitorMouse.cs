using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonitorMouse : GraphicRaycaster
{
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        //if (Cursor.lockState == CursorLockMode.Locked)
        //{
        //    eventData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
        //}

        base.Raycast(eventData, resultAppendList);
    }
}

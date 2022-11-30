using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float y_Value;
    Transform playerPos;

    void Start()
    {
        playerPos = transform.root.GetChild(1);
    }

    void Update()
    {
        if (!Managers.Game.isInventory)
            CameraLookAround();
    }

    // TPS형 카메라 조작
    private void CameraLookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = transform.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 70f);
        else
            x = Mathf.Clamp(x, 335f, 361f);

        transform.position = playerPos.position + Vector3.up*y_Value;
        transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}

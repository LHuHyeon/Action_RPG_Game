using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float y_Value;
    public Transform cameraPos;

    Transform playerPos;    // 현재 플레이어 위치

    Vector3 gunPos;
    Vector3 basePos;
    Vector3 currentPos;

    void Start()
    {
        playerPos = transform.root.GetChild(1);

        gunPos = new Vector3(-0.04f, 1.2f ,-3f);
        basePos = new Vector3(-0.04f, 1f, -3.6f);
        currentPos = basePos;
    }

    void Update()
    {
        if (TalkManager.instance.isDialouge == false)
        {
            if (Managers.Game.isInventory == false)
                CameraLookAround();
            
            CameraUpdate();
            transform.position = playerPos.position + Vector3.up*y_Value;
        }
    }

    // 카메라 위치
    private void CameraUpdate()
    {
        if (Managers.Weapon.weaponState == Define.WeaponState.Gun)
            currentPos = gunPos;    // 총을 들었을 때 시점
        else
            currentPos = basePos;   // 기본 시점

        // 0에 가까워지면
        if (Vector3.Distance(cameraPos.localPosition, currentPos) < 0.0003f)
            cameraPos.localPosition = currentPos;

        // 시점 자연스럽게 변경
        if (cameraPos.localPosition != currentPos)
            cameraPos.localPosition = Vector3.Lerp(cameraPos.localPosition, currentPos, 0.1f);
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

        transform.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }
}

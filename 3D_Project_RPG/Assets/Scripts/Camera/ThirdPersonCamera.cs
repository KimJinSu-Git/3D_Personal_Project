using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform player;

    [Header("Camera Settings")] public float rotationSpeed = 2f;
    public float CameraDistance = 3f;
    public float CameraHeight = 1.5f;

    public float MinPitch = -40f;
    public float MaxPitch = 60f;

    private float Yaw = 0f;
    private float Pitch = 0f;
    private float saveYaw = 0f;
    private float savePitch = 0f;

    private bool Inputf;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        Yaw = angles.y;
        Pitch = angles.x;
    }

    private void Update()
    {
        UpdateRotation();
        UpdateCameraPosition();
    }

    private void UpdateRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxisRaw("Mouse Y") * rotationSpeed;

        Yaw += mouseX;
        Pitch -= mouseY;

        Pitch = Mathf.Clamp(Pitch, MinPitch, MaxPitch);

        if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetMouseButton(0))
        {
            saveYaw = Yaw;
            savePitch = Pitch;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt) && Input.GetMouseButton(0))
        {
            Yaw = saveYaw;
            Pitch = savePitch;
        }

        if (Input.GetKey(KeyCode.LeftAlt) == false)
        {
            player.rotation = Quaternion.Euler(0, Yaw, 0);
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
        Vector3 desiredCameraPos = player.position + rotation * new Vector3(0, CameraHeight, -CameraDistance);
        
        // 카메라 충돌 감지
        Vector3 rayOrigin = player.position + Vector3.up * CameraHeight;
        Vector3 direction = (desiredCameraPos - rayOrigin).normalized;
        float maxDistance = CameraDistance;
        
        if (Physics.Raycast(rayOrigin, direction, out RaycastHit hit, CameraDistance))
        {
            // 충돌 지점까지 카메라 위치 조정
            desiredCameraPos = hit.point + new Vector3(0, 0.2f, 0);
        }
        
        transform.position = desiredCameraPos;
        transform.LookAt(player.position + Vector3.up * 1f);
    }
}

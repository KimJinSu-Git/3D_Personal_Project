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

        if (Input.GetKeyDown(KeyCode.F))
        {
            saveYaw = Yaw;
            savePitch = Pitch;
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            Yaw = saveYaw;
            Pitch = savePitch;
        }

        if (Input.GetKey(KeyCode.F) == false)
        {
            player.rotation = Quaternion.Euler(0, Yaw, 0);
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(Pitch, Yaw, 0);
        Vector3 offset = rotation * new Vector3(0, CameraHeight, -CameraDistance);
        transform.position = player.position + offset;
        transform.LookAt(player.position + Vector3.up * 1f);
    }
}

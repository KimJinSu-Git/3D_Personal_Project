using System;
using System.Collections;
using System.Collections.Generic;
using Suntail;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform camPivot;

    [Header("Settings")]
    [SerializeField] private float mouseSensitivity = 3f; // 마우스 회전 감도
    [SerializeField] private float cameraDistance = 4f; 
    [SerializeField] private float cameraHeight = 2f;  
    [SerializeField] private float rotationSmoothTime = 0.1f; 
    [SerializeField] private Vector2 pitchClamp = new Vector2(-30f, 60f);
    
    [SerializeField] private LayerMask collisionLayers; // 몬스터가 플레이어 주변에 왔을 때 카메라를 줌인시키는 현상을 발생시켜서 제외하기 위해 추가함.
    [SerializeField] private PlayerController playerController;
    
    private float yaw; // 좌우 회전값
    private float pitch; // 상하 회전값
    private Vector3 currentRotation; // 현재 카메라 회전 상태를 저장
    private Vector3 smoothVelocity; // 회전을 부드럽게 할 속도 변수

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null || camPivot == null || playerController.isDialogue) return;
        
        HandleCameraRotation();
        HandleCameraPosition();
        HandlePlayerRotation();
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // yaw는 좌우 회전값 pitch는 상하 회전값 의미
        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, pitchClamp.x, pitchClamp.y);

        Vector3 targetRotation = new Vector3(pitch, yaw, 0f);

        // 현재 회전을 목표 회전까지 부드럽게 보간시키는 함수 (SmoothDamp)
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref smoothVelocity, rotationSmoothTime);

        // 피벗 오브젝트는 yaw만 반영하여 수평 회전 시키기
        camPivot.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    }

    private void HandleCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);

        Vector3 offset = rotation * new Vector3(0f, cameraHeight, -cameraDistance);

        // 피벗(카메라를 위한) 기준으로 카메라가 있어야 할 위치 계산
        Vector3 desiredPosition = camPivot.position + offset;

        Vector3 rayOrigin = camPivot.position;
        Vector3 direction = (desiredPosition - rayOrigin).normalized;

        if (Physics.SphereCast(rayOrigin, 0.2f, direction, out RaycastHit hit, cameraDistance, collisionLayers))
        {
            desiredPosition = rayOrigin + direction * (hit.distance - 0.1f);
        }

        // 최종 카메라 위치
        transform.position = desiredPosition;

        transform.LookAt(camPivot.position + Vector3.up * 1f);
    }

    private void HandlePlayerRotation()
    {
        if (Input.GetKey(KeyCode.LeftAlt)) return;

        // 플레이어의 Y축 회전을 카메라 yaw에 맞게 일치시키기 (카메라가 도는 방향만큼 플레이어도 같이 돌도록)
        target.rotation = Quaternion.Euler(0f, yaw, 0f);
    }
}

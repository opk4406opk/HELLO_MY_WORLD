﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerCameraManager : MonoBehaviour
{
    public static GamePlayerCameraManager Instance = null;
    private Camera PlayerCamera;
    #region cam_option
    private List<float> CamRotArrayX = new List<float>();
    private List<float> CamRotArrayY = new List<float>();
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    private readonly int FrameCounter = 20;
    [Range(1.0f, 100.0f)]
    public float CamSensitivityY = 1.0f;
    [Range(1.0f, 100.0f)]
    public float CamSensitivityX = 1.0f;
    private Quaternion CameraOrigRotation;
    #endregion

    public Quaternion CameraYQuaternion { get; private set; }
    public Quaternion CameraXQuaternion { get; private set; }

    public bool bInitialzed { get; private set; } = false;

    public void Init()
    {
        Instance = this;
        var camInstance = Instantiate<GameObject>(GameResourceSupervisor.GetInstance().GamePlayerCameraPrefab.LoadSynchro());
        PlayerCamera = camInstance.GetComponent<Camera>();
        CameraOrigRotation = PlayerCamera.transform.localRotation;
        //
        bInitialzed = true;
    }

    private void Update()
    {
        if (bInitialzed == false) return;
        //
        float camRotationX = 0f, camRotationY = 0f;
        float camRotAverageY = 0f, camRotAverageX = 0f;
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer || 
            Application.platform == RuntimePlatform.LinuxEditor ||
            Application.platform == RuntimePlatform.LinuxPlayer)
        {
            camRotationY += Input.GetAxis("Mouse Y") * CamSensitivityY;
            camRotationX += Input.GetAxis("Mouse X") * CamSensitivityX;
        }
        else if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var virtualJoystick = VirtualJoystickManager.singleton;
            if (virtualJoystick != null)
            {
                camRotationY += virtualJoystick.GetLookDirection().y * CamSensitivityY;
                camRotationX += virtualJoystick.GetLookDirection().x * CamSensitivityX;
            }
        }
        CamRotArrayY.Add(camRotationY);
        CamRotArrayX.Add(camRotationX);

        if (CamRotArrayY.Count >= FrameCounter)
        {
            CamRotArrayY.RemoveAt(0);
        }
        if (CamRotArrayX.Count >= FrameCounter)
        {
            CamRotArrayX.RemoveAt(0);
        }

        for (int j = 0; j < CamRotArrayY.Count; j++)
        {
            camRotAverageY += CamRotArrayY[j];
        }
        for (int i = 0; i < CamRotArrayX.Count; i++)
        {
            camRotAverageX += CamRotArrayX[i];
        }

        camRotAverageY /= CamRotArrayY.Count;
        camRotAverageX /= CamRotArrayX.Count;

        // 상하 움직임 최대/최소.
        camRotAverageY = Mathf.Clamp(camRotAverageY, minimumY, maximumY);
        // 좌우 움직임은 최대값/최소값 제한두지 않고 적용.
        //camRotAverageX = Mathf.Clamp(camRotAverageX, minimumX, maximumX);

        //KojeomLogger.DebugLog(string.Format("CamRotAvgX : {0}, CamRotAvgY : {1}", camRotAverageX, camRotAverageY), LOG_TYPE.DEBUG_TEST );

        CameraYQuaternion = Quaternion.AngleAxis(camRotAverageY, Vector3.left);
        CameraXQuaternion = Quaternion.AngleAxis(camRotAverageX, Vector3.up);

        // rot cam (상하좌우)
        PlayerCamera.transform.localRotation = CameraOrigRotation * CameraYQuaternion * CameraXQuaternion;
    }

    public Camera GetPlayerCamera()
    {
        return PlayerCamera;
    }

    public void SetPosition(Vector3 newPosition)
    {
        PlayerCamera.transform.position = newPosition;
    }
}

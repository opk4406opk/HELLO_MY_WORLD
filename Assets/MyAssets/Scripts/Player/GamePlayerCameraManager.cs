using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayerCameraManager : MonoBehaviour
{
    public static GamePlayerCameraManager Instance = null;
    private Camera PlayerCamera;
    #region cam_option
    private List<float> CamRotArrayX = new List<float>();
    private List<float> CamRotArrayY = new List<float>();
    public float MinimumAngleY = -360F;
    public float MaximumAngleY = 360F;
    public float MinimumAngleX = -60F;
    public float MaximumAngleX = 60F;
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

    private void FixedUpdate()
    {
        if (bInitialzed == false) return;
        //
        float camRotationX = 0f, camRotationY = 0f;
        float camRotAverageY = 0f, camRotAverageX = 0f;
        bool bMobilePlatform = Application.platform == RuntimePlatform.Android ||
                               Application.platform == RuntimePlatform.IPhonePlayer;
        bool bPCPlatform = Application.platform == RuntimePlatform.WindowsEditor ||
                           Application.platform == RuntimePlatform.WindowsPlayer ||
                           Application.platform == RuntimePlatform.LinuxEditor ||
                           Application.platform == RuntimePlatform.LinuxPlayer;
        if (bPCPlatform == true)
        {
            camRotationY += Input.GetAxis("Mouse Y") * CamSensitivityY;
            camRotationX += Input.GetAxis("Mouse X") * CamSensitivityX;
        }
        else if (bMobilePlatform == true)
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

        CameraYQuaternion = Quaternion.AngleAxis(camRotAverageY, Vector3.left);
        CameraXQuaternion = Quaternion.AngleAxis(camRotAverageX, Vector3.up);



        Quaternion newCamQuaternion = PlayerCamera.transform.localRotation;
        newCamQuaternion *= CameraYQuaternion;
        float rotationXValue = newCamQuaternion.eulerAngles.x;
        //if (rotationXValue > 360.0f) rotationXValue -= 360.0f;
        //KojeomLogger.DebugLog(string.Format("x rotation : {0}", rotationXValue));
        //if (rotationXValue >= MaximumAngleX || rotationXValue <= MinimumAngleX) return;

        // rotation camera. ( Y )
        PlayerCamera.transform.localRotation = newCamQuaternion;
        //KojeomLogger.DebugLog(string.Format("euler angle : {0}", PlayerCamera.transform.localRotation.eulerAngles));
    }

    public Camera GetPlayerCamera()
    {
        return PlayerCamera;
    }

    public void AttachTo(Transform toAttach)
    {
        PlayerCamera.transform.parent = toAttach;
    }

    public void SetPosition(Vector3 newPosition)
    {
        PlayerCamera.transform.position = newPosition;
    }
}

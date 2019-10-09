using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AInput
{
    private readonly int MaximumInputDatas = 10;
    public abstract void UpdateProcess();
    public abstract void Init(ModifyTerrain modifyTerrain);
   
    public InputData GetInputData()
    {
        InputData inputData;
        inputData.InputState = INPUT_STATE.NONE;
        inputData.KeyCodeValues = new List<KeyCode>();
        inputData.KeyCodeValues.Add(KeyCode.None);
        inputData.MobileInputType = MOBILE_INPUT_TYPE.NONE;
        switch (InputDeviceType)
        {
            case INPUT_DEVICE_TYPE.WINDOW:
                if(WindowInputDatas.Count > 0)
                {
                    inputData = WindowInputDatas[0];
                    WindowInputDatas.RemoveAt(0);
                }
                break;
            case INPUT_DEVICE_TYPE.MOBILE:
                if(MobileInputDatas.Count > 0)
                {
                    inputData = MobileInputDatas[0];
                    MobileInputDatas.RemoveAt(0);
                }
                break;
        }
        return inputData;
    }

    public InputData PeekInputData()
    {
        InputData inputData;
        inputData.InputState = INPUT_STATE.NONE;
        inputData.KeyCodeValues = new List<KeyCode>();
        inputData.KeyCodeValues.Add(KeyCode.None);
        inputData.MobileInputType = MOBILE_INPUT_TYPE.NONE;
        switch (InputDeviceType)
        {
            case INPUT_DEVICE_TYPE.WINDOW:
                if (WindowInputDatas.Count > 0)
                {
                    inputData = WindowInputDatas[0];
                }
                break;
            case INPUT_DEVICE_TYPE.MOBILE:
                if (MobileInputDatas.Count > 0)
                {
                    inputData = MobileInputDatas[0];
                }
                break;
        }
        return inputData;
    }

    protected void CreateWindowInputData(INPUT_STATE inputState, KeyCode singleKeyCode)
    {
        InputData newInputData;
        newInputData.InputState = inputState;
        newInputData.MobileInputType = MOBILE_INPUT_TYPE.NONE;
        newInputData.KeyCodeValues = new List<KeyCode>();
        newInputData.KeyCodeValues.Add(singleKeyCode);
        if (WindowInputDatas.Count < MaximumInputDatas && WindowInputDatas.Contains(newInputData) == false)
        {
            WindowInputDatas.Add(newInputData);
        }
    }

    protected void CreateWindowInputData(INPUT_STATE inputState, List<KeyCode> keyCodes)
    {
        InputData newInputData;
        newInputData.InputState = inputState;
        newInputData.MobileInputType = MOBILE_INPUT_TYPE.NONE;
        newInputData.KeyCodeValues = keyCodes;
        if (WindowInputDatas.Count < MaximumInputDatas && WindowInputDatas.Contains(newInputData) == false)
        {
            WindowInputDatas.Add(newInputData);
        }
    }

    protected void CreateMobileInputData(INPUT_STATE inputState, MOBILE_INPUT_TYPE mobileInputType)
    {
        InputData newInputData;
        newInputData.InputState = inputState;
        newInputData.MobileInputType = mobileInputType;
        newInputData.KeyCodeValues = new List<KeyCode>();
        newInputData.KeyCodeValues.Add(KeyCode.None);
        if (MobileInputDatas.Count < MaximumInputDatas)
        {
            MobileInputDatas.Add(newInputData);
        }
    }

    protected List<InputData> WindowInputDatas = new List<InputData>();
    protected List<InputData> MobileInputDatas = new List<InputData>();
    protected ModifyTerrain ModifyTerrainInstance;
    protected INPUT_DEVICE_TYPE InputDeviceType = INPUT_DEVICE_TYPE.NONE;

    protected abstract void CheckInputState();
}

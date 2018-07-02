using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AInput
{
    public abstract void UpdateProcess();
    public abstract void Init(ModifyTerrain modifyTerrain);
    public abstract InputData GetInputData();
    public abstract Queue<InputData> GetOverlappedInputData();

    protected InputData curInputData;
    protected Queue<InputData> overlappedInputs;
    protected ModifyTerrain modifyTerrain;

    protected abstract void CheckOverlapInputState();
    protected abstract void CheckSingleInputState();
}

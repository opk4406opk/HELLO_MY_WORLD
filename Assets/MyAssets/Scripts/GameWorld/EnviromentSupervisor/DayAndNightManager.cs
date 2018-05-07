using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayAndNightManager : MonoBehaviour, IManager
{
    [SerializeField]
    private Transform sun;
    [SerializeField]
    private Transform moon;
    private Vector3 targetPos;
    private Coroutine movingRoutine;
    
    public void Init()
    {
        targetPos = Vector3.zero;
    }

    public void StartDayAndNight()
    {
        movingRoutine = StartCoroutine(MovingProcess());
    }

    public void ResetManager()
    {
    }

    private IEnumerator MovingProcess()
    {
        while (true)
        {
            Moving(sun);
            Moving(moon);
            yield return null;
        }
    }

    private void Moving(Transform planet)
    {
        planet.RotateAround(targetPos, Vector3.right, 10.0f * Time.deltaTime);
        planet.LookAt(targetPos);
    }
}

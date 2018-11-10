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
        // sun, moon이 회전하는 중심지점을 (0,0,0)으로 지정한다.
        // 임시적인 값으로, 전체 생성되는 월드맵의 정 중앙을 기준으로 회전해야한다.
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

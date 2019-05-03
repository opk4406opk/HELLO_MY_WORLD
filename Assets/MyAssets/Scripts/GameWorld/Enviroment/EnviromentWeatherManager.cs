using System.Collections;
using UnityEngine;
/*
 * 태양의 고도를 계산하는 공식은 다음과 같다.

sin h = (sin δ * sin φ) + (cos δ * cos φ * cos H)

    h (태양의 고도각) : -90° ~ +90°
    δ (태양의 적위) : -23.5° ~ +23.5°
    φ (위도) : -90° ~ +90° (남극 ~ 북극)
    H (시각) : -180° ~ +180° (00시는 -180°, 06시는 -90°, 12시는 0°, 18시는 +90°, 24시는 +180°)

위의 식에 따라 계산된 태양의 고도가

    h > 0° 일 때는 태양이 뜬 상태이고,
    h = 0° 일 때는 태양이 지평선(또는 수평선)에 있는 상태, 즉, 일출, 일몰인 상태이며,
    h < 0° 일 때는 태양이 아직 뜨지 않거나, 진 상태이다.

 */
public class EnviromentWeatherManager : MonoBehaviour {

    [SerializeField]
    private Transform Sun;
    [SerializeField]
    private Transform Moon;
    public void Init()
    {
        StartCoroutine(MovingProcess());
    }

    private IEnumerator MovingProcess()
    {
        while (true)
        {
            Moving(Sun);
            Moving(Moon);
            yield return null;
        }
    }

    private void Moving(Transform planet)
    {
        // 임시값인 zero 벡터를 기준으로 회전하고 바라보게한다.
        planet.RotateAround(Vector3.zero, Vector3.right, 10.0f * Time.deltaTime);
        planet.LookAt(Vector3.zero);
    }

    private IEnumerator SunController()
    {
        while (true)
        {
            float curTimeSec = System.DateTime.Now.Second;
            float timeAngle = curTimeSec * 0.25f;
            float h = (Mathf.Sin(45.0f) * Mathf.Sin(23.5f)) + (Mathf.Cos(23.5f) * Mathf.Cos(45.0f) * Mathf.Cos(timeAngle));
            Quaternion rot = Quaternion.Euler(0, Mathf.Asin(h) * Mathf.Rad2Deg, 0);
            //KojeomLogger.DebugLog(Mathf.Asin(h).ToString());
            Sun.transform.localRotation = rot;

            //worldLight.transform.RotateAround(Camera.main.transform.position, Vector3.up, Mathf.Asin(h));
            //yield return new WaitForSeconds(1.0f);
            yield return new WaitForSeconds(1.0f);
        }
    }
}

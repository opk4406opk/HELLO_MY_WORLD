using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_OBB : MonoBehaviour {

    [SerializeField]
    private GameObject A;
    [SerializeField]
    private Transform aMaxExtent;
    [SerializeField]
    private GameObject B;
    [SerializeField]
    private Transform bMaxExtent;

    void Start()
    {

        A.GetComponent<CustomOBB>().Init(A.transform, aMaxExtent);
        B.GetComponent<CustomOBB>().Init(B.transform, bMaxExtent);
        StartCoroutine(TEST());
    }
    private IEnumerator TEST()
    {
        for (;;)
        {
            if (CustomOBB.Collide(A.GetComponent<CustomOBB>(),
                B.GetComponent<CustomOBB>())) Debug.Log("Colldie A with B");
            yield return new WaitForSeconds(1.0f);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(A.transform.position, B.transform.position);
    }

}

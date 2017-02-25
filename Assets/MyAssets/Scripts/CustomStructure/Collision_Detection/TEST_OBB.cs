using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_OBB : MonoBehaviour {

    [SerializeField]
    private GameObject A;
    [SerializeField]
    private GameObject B;
    
    void Start()
    {

        A.GetComponent<CustomOBB>().Init(A.transform, 0.25f, 0.5f, 0.25f);
        B.GetComponent<CustomOBB>().Init(B.transform, 0.25f, 0.5f, 0.25f);
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

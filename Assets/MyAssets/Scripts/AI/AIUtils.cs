using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUtils : MonoBehaviour
{
    public static Vector3 RandomWorldPositionFromActorPos(Vector3 actorPosition)
    {
        List<Vector3> direcitons = new List<Vector3>();
        direcitons.Add(new Vector3(0, 0, 1));
        direcitons.Add(new Vector3(0, 0, -1));
        direcitons.Add(new Vector3(1, 0, 0));
        direcitons.Add(new Vector3(-1, 0, 0));
        direcitons.Add(new Vector3(1, 0, 1));
        direcitons.Add(new Vector3(1, 0, -1));
        direcitons.Add(new Vector3(-1, 0, 1));
        direcitons.Add(new Vector3(-1, 0, -1));

        int randValue = KojeomUtility.RandomInteger(0, direcitons.Count);
        return actorPosition += direcitons[randValue];
    }
}

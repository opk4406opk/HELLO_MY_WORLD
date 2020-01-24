// Cristian Pop - https://boxophobic.com/

using UnityEngine;

namespace Boxophobic
{

    //[CreateAssetMenu(fileName = "BoxoVersion", menuName = "Boxophbic/BoxoVersion", order = 1)]
    public class BVersion : ScriptableObject
    {

        [Header("File")]
        public int FileVersion;

        [Header("Generic")]
        public int GeneralVersion;

        [Space(10)]
        public int StandardLitVersion;
        public int SimpleLitVersion;
        public int LWLitVersion;
        public int HDLitVersion;

        [Space(10)]
        public int AdvancedVersion;
        public int SimpleVersion;

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorGeneratorTool.Sources
{
    public enum NPCResourceID
    {
        NPC0,
        NPC1,
        //...
        COUNT
    }
    public enum MonsterResourceID
    {
        MON0,
        MON1,
        //...
        COUNT
    }

    public enum AnimalResourceID
    {
        ANIMAL0,
        ANIMAL1,
        ANIMAL2,
        ANIMAL3,
        ANIMAL4,
        ANIMAL5,
        //...
        COUNT
    }
    public enum NPC_TYPE
    {
        Merchant,
        Guard,
        COUNT
    }
    public enum ANIMAL_TYPE
    {
        Pig,
        Cow,
        Chiken,
        Fox,
        Lion,
        Dog,
        COUNT
    }
    public enum ANIMAL_CATEGORY
    {
        None,
        Herbivore, // 초식 동물.
        FleshEating, // 육식 동물.
        Polyphagia // 잡식 동물.
    }

    public enum GenerateActorType
    {
        NPC,
        ANIMAL,
        MONSTER,
        COUNT
    }

    public class BaseGenerateData
    {
        #region classification and Identification
        public string TYPE;
        public string RESOURCE_ID;
        public string UNIQUE_ID;
        #endregion

        #region stat
        public string AP;
        public string HP;
        public string MP;
        #endregion

        #region info
        public string NAME;
        #endregion
    }

    abstract class AGenerator
    {
        abstract public void Init();
        abstract public bool Generate(bool isDefaultGenerate, string savePath);
        abstract public void Release();
    }
}

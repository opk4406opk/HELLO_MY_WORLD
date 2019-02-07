using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorGeneratorTool.Sources
{
    public enum NPC_CATEGORY
    {
        MERCHANT
    }
    public enum NPC_TYPE
    {
        ROAMING_MERCHANT
    }

    class NPCGenerator : AGenerator
    {
        private struct GenerateData
        {
            #region classification
            public NPC_CATEGORY CATEGORY;
            public NPC_TYPE TYPE;
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
        private class NPCJsonData
        {
            public List<GenerateData> Npcs = new List<GenerateData>();
        }

        public bool IsDefaultGenerate = false;

        public override bool Generate()
        {
            return true;
        }

        public override void Init()
        {
        }

        public override void Release()
        {
        }
    }
}

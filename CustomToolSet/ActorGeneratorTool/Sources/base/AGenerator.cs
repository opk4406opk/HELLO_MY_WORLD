using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorGeneratorTool.Sources
{
    abstract class AGenerator
    {
        abstract public void Init();
        abstract public bool Generate(bool isDefaultGenerate, string savePath);
        abstract public void Release();
    }
}

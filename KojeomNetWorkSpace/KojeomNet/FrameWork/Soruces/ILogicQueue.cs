﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public interface ILogicQueue
    {
        void Enqueue(CPacket msg);
        Queue<CPacket> GetAll();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GM.Core
{
    public interface ILock
    {
        void Enter();
        void Exit();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataCollector
{
    public enum SocketState
    {
        Initialize,
        Identifying,
        AwaitAck,
        MainLoop
    }
}

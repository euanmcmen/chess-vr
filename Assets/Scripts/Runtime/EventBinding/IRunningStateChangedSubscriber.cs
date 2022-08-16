using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRunningStateChangedSubscriber
{
    void HandleRunningStateChanged(bool value);

    void HandleRunningStateChangedClient(bool value);
}


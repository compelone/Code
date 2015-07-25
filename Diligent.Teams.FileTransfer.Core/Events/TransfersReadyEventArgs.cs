using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diligent.Teams.FileTransfer.Core.Events
{
    public delegate void TransfersReadyEventHandler(object sender, TransfersReadyEventArgs args);
    public class TransfersReadyEventArgs
    {
        public int Count { get; set; }
        public bool AreItemsAvailable { get; set; }
    }
}

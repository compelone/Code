using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diligent.Teams.FileTransfer.Core.Events
{
    public class StatusChangedEventArgs : EventArgs
    {
        public Guid DocumentContainerId { get; set; }
        public Guid DocumentVersionId { get; set; }
        public FileTransferStatus PreviousStatus { get; set; }
        public FileTransferStatus Status { get; set; }
    }
}

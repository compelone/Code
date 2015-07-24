using System;

namespace Diligent.Teams.FileTransfer.Core.Events
{
    public class PriorityChangedEventArgs
    {
        public Guid DocumentContainerId { get; set; }
        public Guid DocumentVersionId { get; set; }
        public FileTransferPriority PreviousPriority { get; set; }
        public FileTransferPriority Priority { get; set; }
    }
}
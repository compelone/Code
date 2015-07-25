using System;
using System.Threading;
using System.Threading.Tasks;
using Diligent.Teams.FileTransfer.Core.Events;

namespace Diligent.Teams.FileTransfer.Core.Managers
{
    public interface IFileTransferManager
    {
        bool IsStarted { get; }
        void Add(FileTransferContext fileTransferContext);
        Task Start(CancellationTokenSource cancellationTokenSource);
        void Shutdown();
    }
}

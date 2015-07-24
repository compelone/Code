using System.Threading;
using System.Threading.Tasks;

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

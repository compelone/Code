using System.Threading.Tasks.Dataflow;

namespace Diligent.Teams.FileTransfer.Core.Managers
{
    public interface ITransfer
    {
        void SetTransferManager(IFileTransferManager fileTransferManager);

        ITargetBlock<FileTransferContext> Handle();
    }
}

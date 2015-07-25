using System;
using System.Threading.Tasks.Dataflow;

namespace Diligent.Teams.FileTransfer.Core.Managers
{
    public class Download : ITransfer
    {
        private IFileTransferManager _fileTransferManager;

        public void SetTransferManager(IFileTransferManager fileTransferManager)
        {
            _fileTransferManager = fileTransferManager;
        }

        public ITargetBlock<FileTransferContext> Handle()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }
    }
}

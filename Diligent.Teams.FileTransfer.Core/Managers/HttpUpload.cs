using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Diligent.Teams.FileTransfer.Core.Events;

namespace Diligent.Teams.FileTransfer.Core.Managers
{
    public class HttpUpload : ITransfer
    {
        private IFileTransferManager _fileTransferManager;

        public void SetTransferManager(IFileTransferManager fileTransferManager)
        {
            _fileTransferManager = fileTransferManager;
        }

        public ITargetBlock<FileTransferContext> Handle()
        {
            var action = new ActionBlock<FileTransferContext>(async a =>
            {
                if (a == null)
                {
                    return;
                }
                a.PercentComplete = 39;
                //File.Delete(a.StagingFileName.LocalPath);
                //a.SetStatus(FileTransferStatus.CancellingUpload);
                Console.WriteLine("Http upload called for document " + a.FileName);
                await  Task.Delay(1000);
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3});

            return action;
        }
    }
}

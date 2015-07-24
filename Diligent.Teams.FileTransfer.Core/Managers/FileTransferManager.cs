using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Diligent.Teams.FileTransfer.Core.Events;

namespace Diligent.Teams.FileTransfer.Core.Managers
{
    public class FileTransferManager : IFileTransferManager
    {
        private static ConcurrentBag<FileTransferContext> _transferFilesList;
        private readonly BufferBlock<FileTransferContext> _tranferItems; 

        public FileTransferManager()
        {
            _transferFilesList = new ConcurrentBag<FileTransferContext>();
            _tranferItems = new BufferBlock<FileTransferContext>(new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 3,
                BoundedCapacity = 5,
            });
        }

        #region Properties
        public bool IsStarted { get; private set; }

        #endregion

        public static void CreateDirectory(string localPath)
        {
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
        }


        public virtual void Add(FileTransferContext fileTransferContext)
        {
            var existing = _transferFilesList.Any(f => f.DocumentContainerId == fileTransferContext.DocumentContainerId);

            if (!existing)
            {
                fileTransferContext.StatusChanged += FileTransferContextOnStatusChanged;
                fileTransferContext.PriorityChanged += FileTransferContextOnPriorityChanged;
                _transferFilesList.Add(fileTransferContext);
            }
        }

        public  Task Start(CancellationTokenSource cancellationTokenSource)
        {
            IsStarted = true;
            cancellationTokenSource.Token.Register(Shutdown);
            return Task.Run(() =>
            {
                while (true)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    foreach (var fileTransferContext in _transferFilesList.OrderByDescending(f => f.Priority))
                    {
                        _tranferItems.Post(fileTransferContext);
                        Console.WriteLine($"File was received {fileTransferContext.FileName}");
                        Console.WriteLine($"Total items in buffer are {_tranferItems.Count}");
                    }
                }
            });
        }

        public void Shutdown()
        {
            IsStarted = false;
            Console.WriteLine("Cancel was called");
        }

        #region Event Handlers
        private void FileTransferContextOnPriorityChanged(object sender, PriorityChangedEventArgs agrs)
        {
            throw new NotImplementedException();
        }

        private void FileTransferContextOnStatusChanged(object sender, StatusChangedEventArgs agrs)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
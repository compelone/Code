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
        private readonly Download _download;
        private readonly HttpUpload _upload;

        public FileTransferManager()
        {
            _transferFilesList = new ConcurrentBag<FileTransferContext>();
            _download = new Download();
            _upload = new HttpUpload();

            _download.SetTransferManager(this);
            _upload.SetTransferManager(this);

            _tranferItems = new BufferBlock<FileTransferContext>(new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 3,
                BoundedCapacity = 5,
            });
        }

        #region Properties
        public bool IsStarted { get; private set; }

        #endregion

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

        public virtual Task Start(CancellationTokenSource cancellationTokenSource)
        {
            IsStarted = true;
            cancellationTokenSource.Token.Register(Shutdown);
            _tranferItems.LinkTo(_upload.Handle(), f => f.Direction == Direction.Upload);

            return Task.Run(async () =>
            {
                while (true)
                {
                    if (cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    while (!_transferFilesList.IsEmpty)
                    {
                        FileTransferContext availableItem;
                        
                        _transferFilesList.TryTake(out availableItem);

                        if (availableItem != null)
                        {
                            _tranferItems.Post(availableItem);
                            Console.WriteLine($"File was received {availableItem.FileName}");
                            Console.WriteLine($"Total items in buffer are {_tranferItems.Count}");
                            Console.WriteLine($"Total items in bag are {_transferFilesList.Count}");
                        }
                    }
                    
                    // Asynchronous delay to prevent us from
                    // spinning through the while loop when there is no
                    // work.
                    await Task.Delay(2000);
                }
            });
        }



        public virtual void Shutdown()
        {
            IsStarted = false;
            Console.WriteLine("Cancel was called");
        }

        #region Event Handlers
        protected virtual void FileTransferContextOnPriorityChanged(object sender, PriorityChangedEventArgs agrs)
        {
            throw new NotImplementedException();
        }

        protected virtual void FileTransferContextOnStatusChanged(object sender, StatusChangedEventArgs agrs)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Helpers
        public static void CreateDirectory(string localPath)
        {
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }
        }

        #endregion
    }
}
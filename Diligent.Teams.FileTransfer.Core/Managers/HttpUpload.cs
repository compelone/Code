using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

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
            var action = new ActionBlock<FileTransferContext>(async ftc =>
            {
                if (ftc == null)
                {
                    return;
                }

                // TODO: CHECK PAUSE OR CANCEL STATES

                ftc.SetStatus(FileTransferStatus.CalculatingCrc);
                ftc.Crc = await _fileTransferManager.CalculateCrc(ftc.StagingFileName.LocalPath);

                ftc.SetStatus(FileTransferStatus.CreatingChunks);
                await CreateChunks(ftc);

                ftc.PercentComplete = 39;
                //File.Delete(a.StagingFileName.LocalPath);
                //a.SetStatus(FileTransferStatus.CancellingUpload);
                Console.WriteLine("Http upload called for document " + ftc.FileName);
                await  Task.Delay(1000);
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3});

            return action;
        }

        private Task CreateChunks(FileTransferContext fileTransferContext)
        {

            return Task.Run(async () =>
            {
                var chunkSize = fileTransferContext.ChunkSize;
                var numChunks = fileTransferContext.NumberOfChunks;
                var numBytes = fileTransferContext.FileSize;

                for (var i = 0; i < numChunks; i++)
                {
                    var sourceOffset = i * chunkSize;
                    var chunkBufferSize = i != numChunks - 1 ? chunkSize : numBytes - sourceOffset;
                    var unencryptedChunk = new byte[chunkBufferSize];

                    using (var fileStream = new FileStream(fileTransferContext.StagingFileName.LocalPath, FileMode.Open))
                    {
                        await fileStream.ReadAsync(unencryptedChunk, 0, chunkBufferSize);
                        var chunkFileName = fileTransferContext.TrackingCode + "_" + i;
                        var chunkFilePath = Path.Combine(fileTransferContext.ChunksPath.LocalPath, chunkFileName);
                        using (var writeStream = new FileStream(chunkFilePath, FileMode.Create))
                        {
                            await writeStream.WriteAsync(unencryptedChunk, 0, chunkBufferSize);
                            writeStream.Flush();
                        }

                        fileTransferContext.Chunks.Add(chunkFilePath);
                    }
                }
            });
        }
    }
}

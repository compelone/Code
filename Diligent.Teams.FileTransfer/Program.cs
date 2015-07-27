using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Diligent.Teams.FileTransfer.Core;
using Diligent.Teams.FileTransfer.Core.Managers;

namespace Diligent.Teams.FileTransfer
{
    class Program
    {
        private const int DefaultChunkSize = 65000;
        static readonly Uri _inputUri = new Uri(@"d:\QA Test Data\PDF");
        static readonly Uri _outputUri = new Uri(@"d:\QA Test Data\PDF\Output");
        static readonly Uri _chunksPath = new Uri(@"d:\QA Test Data\PDF\Chunks");

        static readonly Uri _inputUri2 = new Uri(@"d:\QA Test Data\Excel");
        static readonly Uri _outputUri2 = new Uri(@"d:\QA Test Data\Excel\Output");
        static readonly Uri _chunksPath2 = new Uri(@"d:\QA Test Data\Excel\Chunks");

        static void Main(string[] args)
        {

            var cancellationTokenSource = new CancellationTokenSource();

            var actionBlock = new ActionBlock<string>(s => Console.WriteLine(s));
            var fileTransfer = new FileTransferManager();
            fileTransfer.Start(cancellationTokenSource);

            var task = BuildTransferListAsync(_inputUri, _outputUri, actionBlock);

            task.Wait();

            var fileTransfers = task.Result;

            foreach (var fileTransferContext in fileTransfers)
            {
                fileTransfer.Add(fileTransferContext);
            }

            actionBlock.Completion.Wait(cancellationTokenSource.Token);

            Task.Run(async () =>
            {
                await Task.Delay(30000);

                for (int i = 0; i < 100; i++)
                {
                    var files = Directory.EnumerateFiles(_inputUri2.LocalPath);
                    var sectionId = Guid.NewGuid();


                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        var ftc = new FileTransferContext(fileInfo, DefaultChunkSize)
                        {
                            SectionId = sectionId,
                            DocumentContainerId = Guid.NewGuid(),
                            DocumentVersionId = Guid.NewGuid(),
                            StagingPath = _outputUri2,
                            Direction = Direction.Upload,
                            ChunksPath = _chunksPath2,
                            TrackingCode = Guid.NewGuid().ToString(),
                        };
                        fileTransfer.Add(ftc);
                        await actionBlock.SendAsync($"Begin copy file {ftc.FileName}");
                        File.Copy(file, Path.Combine(_outputUri2.LocalPath, ftc.FileName), true);
                        await actionBlock.SendAsync($"Completed copying file {ftc.FileName}");
                    }
                }
            });

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        public static Task<List<FileTransferContext>> BuildTransferListAsync(Uri inputUri, Uri outputUri, ActionBlock<string> actionBlock)
        {
            FileTransferManager.CreateDirectory(outputUri.LocalPath);
            List<FileTransferContext> transferFilesList = new List<FileTransferContext>();

            return Task.Run(async () =>
            {
                var files = Directory.EnumerateFiles(inputUri.LocalPath);
                var sectionId = Guid.NewGuid();

                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    var ftc = new FileTransferContext(fileInfo, DefaultChunkSize)
                    {
                        SectionId = sectionId,
                        DocumentContainerId = Guid.NewGuid(),
                        DocumentVersionId = Guid.NewGuid(),
                        StagingPath = outputUri,
                        Direction = Direction.Upload,
                        ChunksPath = _chunksPath,
                        TrackingCode = Guid.NewGuid().ToString(),
                    };
                    transferFilesList.Add(ftc);
                    await actionBlock.SendAsync($"Begin copy file {ftc.FileName}");
                    File.Copy(file, Path.Combine(outputUri.LocalPath, ftc.FileName), true);
                    await actionBlock.SendAsync($"Completed copying file {ftc.FileName}");
                }

                actionBlock.Complete();
                return transferFilesList;

            });
        }
    }
}

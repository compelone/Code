using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Diligent.Teams.FileTransfer.Core.Annotations;
using Diligent.Teams.FileTransfer.Core.Events;

namespace Diligent.Teams.FileTransfer.Core
{
    public class FileTransferContext : INotifyStatusChanged, INotifyPropertyChanged, INotifyPriorityChanged
    {
        #region Fields

        private readonly FileInfo _fileInfo;
        private int _percentComplete;
        private FileTransferStatus _status;
        private FileTransferPriority _priority;
        private int _lastChunk;
        public event StatusChangedEventHandler StatusChanged;
        public event PriorityChangedEventHandler PriorityChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        public FileTransferContext(FileInfo fileInfo, int chunkSize)
        {
            _fileInfo = fileInfo;
            ChunkSize = chunkSize;
            CanPause = true;
            CanCancel = true;
            Chunks = new List<string>();
            Status = FileTransferStatus.Pending;
        }

        #region Properties

        public Uri UploadUri { get; set; }
        public int Crc { get; set; }
        public Guid DocumentContainerId { get; set; }
        public Guid DocumentVersionId { get; set; }
        public Guid SectionId { get; set; }
        public string TrackingCode { get; set; }
        public bool CanPause { get; set; }
        public bool CanCancel { get; set; }
        public List<string> Chunks { get; set; }
        public Uri StagingPath { get; set; }
        public Uri ChunksPath { get; set; }
        public int ChunkSize { get; }

        public Uri InputFileName
        {
            get { return new Uri(_fileInfo.FullName); }
        }

        public int FileSize
        {
            get { return (int) _fileInfo.Length; }
        }

        public string FileName
        {
            get { return _fileInfo.Name; }
        }

        public Uri StagingFileName
        {
            get { return new Uri(Path.Combine(StagingPath.LocalPath, FileName)); }
        }

        public int NumberOfChunks
        {
            get { return FileSize % ChunkSize > 0 ? (FileSize/ChunkSize) + 1 : FileSize/ChunkSize ; }
        }

        public FileTransferStatus Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public FileTransferPriority Priority
        {
            get { return _priority; }
            private set
            {
                _priority = value;
                OnPropertyChanged();
            }
        }

        public int LastChunk
        {
            get { return _lastChunk; }
            set
            {
                _lastChunk = value;
                OnPropertyChanged();
                PercentComplete = (LastChunk/Chunks.Count)*100;
            }
        }

        public int PercentComplete
        {
            get { return _percentComplete; }
            set
            {
                _percentComplete = value;
                OnPropertyChanged();
            }
        }

        public Direction Direction { get; set; }

        #endregion


        #region Methods

        public void SetStatus(FileTransferStatus status)
        {
            if (Status == status)
            {
                return;
            }

            var previousStatus = Status;
            Status = status;
            if (Status == FileTransferStatus.UploadComplete)
            {
                CanPause = false;
            }
            OnStatusChanged(new StatusChangedEventArgs
            {
                DocumentContainerId = DocumentContainerId,
                DocumentVersionId = DocumentVersionId,
                PreviousStatus = previousStatus,
                Status = Status
            });
        }

        public void SetPriority(FileTransferPriority priority)
        {
            if (Priority == priority)
            {
                return;
            }

            var previousPriority = Priority;
            Priority = priority;
            OnPriorityChanged(new PriorityChangedEventArgs
            {
                DocumentContainerId = DocumentContainerId,
                DocumentVersionId = DocumentVersionId,
                PreviousPriority = previousPriority,
                Priority = Priority
            });
        }

        #endregion


        #region Events

        private void OnPriorityChanged(PriorityChangedEventArgs args)
        {
            if (PriorityChanged != null)
            {
                PriorityChanged(this, args);
            }
        }


        protected virtual void OnStatusChanged(StatusChangedEventArgs args)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, args);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion


    }
}

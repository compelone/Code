namespace Diligent.Teams.FileTransfer.Core
{
    public enum FileTransferStatus
    {
        Pending,
        WaitingToUpload,
        PreparingToUpload,
        ReadingFile,
        CalculatingCrc,
        SendingUploadRequest,
        CreatingChunks,
        UploadingChunks,
        UploadComplete,
        ConversionComplete,
        DownloadComplete,
        UploadPaused,
        UploadRestarted,
        CancellingUpload,
        UploadFailed,
        DownloadFailed,
        ConversionFailed
    }
}
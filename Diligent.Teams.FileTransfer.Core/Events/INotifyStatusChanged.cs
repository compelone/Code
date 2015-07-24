namespace Diligent.Teams.FileTransfer.Core.Events
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs agrs);
    public interface INotifyStatusChanged
    {
        event StatusChangedEventHandler StatusChanged;
    }
}

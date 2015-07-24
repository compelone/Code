namespace Diligent.Teams.FileTransfer.Core.Events
{
    public delegate void PriorityChangedEventHandler(object sender, PriorityChangedEventArgs agrs);

    public interface INotifyPriorityChanged
    {
        event PriorityChangedEventHandler PriorityChanged;
    }
}

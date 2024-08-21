namespace AutomaticDownloader.Interfaces
{
    internal interface IMessageConsumer
    {
        void StartListening();
        void StopListening();
    }
}

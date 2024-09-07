namespace MediaDownloaderKafkaRelayer.Interfaces;

internal interface IMessageConsumer
{
    void StartListening();
    void StopListening();
}
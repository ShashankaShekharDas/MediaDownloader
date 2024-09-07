using AutomaticDownloader.VadapavFileGetter;

namespace AutomaticDownloader.Interfaces;

public interface IGetMediaLink
{
    public MediaFileInfo GetMediaInfo();
}
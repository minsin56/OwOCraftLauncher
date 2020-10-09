using System;
using System.ComponentModel;
using System.Net;

namespace Launcher.Data
{
    public static class ModDownloader
    {
        public const string FileLink = "https://drive.google.com/uc?id=1-7bqT2n0XupUYutcgbyYuxoOFF6SMYIX&export=download";


        public static async void DownloadFile(FileDownloader.DownloadProgressChangedEventHandler ChangeHandler, AsyncCompletedEventHandler OnComplete)
        {
            using(FileDownloader Downloader = new FileDownloader())
            {
                Downloader.DownloadProgressChanged += ChangeHandler;
                Downloader.DownloadFileCompleted += OnComplete;

                Downloader.DownloadFileAsync(FileLink, "Mods.zip");
            }
        }
    }
}
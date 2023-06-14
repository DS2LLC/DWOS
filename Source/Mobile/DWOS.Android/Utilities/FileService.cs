using System;
using DWOS.Services.Messages;
using DWOS.Utilities;
using DWOS.ViewModels;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Ionic.Zlib;

namespace DWOS.Android
{
    /// <summary>
    /// Implementation of <see cref="IFileService"/>.
    /// </summary>
    public class FileService : IFileService
    {
        private const string PERMISSION_ERROR_MESSAGE = "Please allow this app to access external media to open media.";

        public async Task<ResponseBase> Download(DocumentInfo selectedDocument)
        {
            if (selectedDocument == null || !GetPermission())
            {
                return ResponseBase.Error(PERMISSION_ERROR_MESSAGE);
            }

            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();

            var docService = ServiceContainer.Resolve<IDocumentService>();
            docService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var document = await docService.GetDocumentAsync(new DocumentRequest()
            {
                UserId = loginViewModel.UserProfile.UserId,
                RevisionId = selectedDocument.CurrentRevisionId
            });

            if (document.Success)
            {
                var path = GetPath(selectedDocument);

                using (var docStream = GetStream(document.Document))
                {
                    using (var fileStream = System.IO.File.OpenWrite(path))
                    {
                        docStream.CopyTo(fileStream);
                    }
                }
            }

            return document;
        }

        public async Task<ResponseBase> Download(MediaSummary selectedMedia)
        {
            if (selectedMedia == null || !GetPermission())
            {
                return ResponseBase.Error(PERMISSION_ERROR_MESSAGE);
            }

            var loginViewModel = ServiceContainer.Resolve<LogInViewModel>();

            var mediaService = ServiceContainer.Resolve<IMediaService>();
            mediaService.RootUrl = loginViewModel.ServerUrlWellFormed;

            var media = await mediaService.GetMediaAsync(new MediaDetailRequest()
            {
                UserId = loginViewModel.UserProfile.UserId,
                MediaId = selectedMedia.MediaId
            });

            if (media.Success)
            {
                var path = GetPath(selectedMedia);
                System.IO.File.WriteAllBytes(path, media.Media.Media);
            }

            return media;
        }

        public string GetPath(DocumentInfo document)
        {
            if (!GetPermission())
            {
                return null;
            }

            var directory = System.IO.Path.Combine(
                global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                "DWOS",
                "Documents");
            try
            {
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                return System.IO.Path.Combine(directory,
                    $"{document.CurrentRevisionId}.{document.MediaType}");
            }
            catch (UnauthorizedAccessException)
            {
                var dialogService = ServiceContainer.Resolve<IDialogService>();
                dialogService.ShowToast($"Unable to open media directory {directory}.");
                return null;
            }
        }

        public string GetPath(MediaSummary media)
        {
            if (!GetPermission())
            {
                return null;
            }

            var directory = System.IO.Path.Combine(
                global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                "DWOS",
                "Media");

            try
            {
                if (!System.IO.Directory.Exists(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }

                return System.IO.Path.Combine(directory,
                    $"{media.MediaId}.{media.FileExtension}");
            }
            catch (UnauthorizedAccessException)
            {
                var dialogService = ServiceContainer.Resolve<IDialogService>();
                dialogService.ShowToast($"Unable to open media directory {directory}.");
                return null;
            }
        }

        private static bool GetPermission()
        {
            var dialogService = ServiceContainer.Resolve<IDialogService>();
            var context = global::Android.App.Application.Context;
            var hasPermission = ContextCompat.CheckSelfPermission(context, Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                ContextCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) == Permission.Granted;

            if (hasPermission)
            {
                return true;
            }

            // Ask for permission
            var permission = new[]
            {
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.WriteExternalStorage
            };

            dialogService.Show("DWOS", PERMISSION_ERROR_MESSAGE, "OK", () =>
            {
                if (DWOSApplication.Current.CurrentActivity.TryGetTarget(out var activity))
                {
                    ActivityCompat.RequestPermissions(activity, permission, RequestCodes.ReadWriteMedia);
                }
            });

            return false;
        }

        private System.IO.Stream GetStream(DocumentDetail documentDetail)
        {
            if (documentDetail == null || documentDetail.DocumentData == null)
            {
                return new System.IO.MemoryStream();
            }

            if (documentDetail.IsCompressed)
            {
                return new ZlibStream(
                    new System.IO.MemoryStream(documentDetail.DocumentData),
                    CompressionMode.Decompress);
            }

            return new System.IO.MemoryStream(documentDetail.DocumentData);
        }
    }
}
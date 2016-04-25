namespace Bars.Gkh.Ris.Integration.FileService.Impl
{
    using System;
    using System.Net;
    using System.Text;
    using B4.DataAccess;
    using B4.Modules.FileStorage;
    using B4.Utils;
    
    using Bars.Gkh.Ris.ConfigSections;
    using Bars.Gkh.Ris.Extensions;

    using Castle.Windsor;
    using Config;
    using CryptoPro.Sharpei;
    using Entities;
    using Enums;

    /// <summary>
    /// Сервис для сохранения вложений файлов на рест-сервис
    /// </summary>
    public class FileUploadService : IFileUploadService
    {
        /// <summary>
        /// Container
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Отправить файл на рест-сервис и сохранить полученный гуид в БД
        /// </summary>
        /// <param name="file"></param>
        /// <param name="description"></param>
        /// <param name="fileStorageName"></param>
        /// <param name="senderId"></param>
        /// <returns></returns>
        public Attachment SaveAttachment(FileInfo file, string description, FileStorageName fileStorageName, string senderId)
        {
            var attachmentDomain = Container.ResolveDomain<Attachment>();

            try
            {
                var attachment =  new Attachment
                {
                    FileInfo = file,
                    Guid = this.UploadSmallFile(file, senderId, fileStorageName),
                    Hash = this.GetGostHash(file),
                    Name = file.Name,
                    Description = description
                };

                attachmentDomain.Save(attachment);

                return attachment;
            }
            finally
            {
                Container.Release(attachmentDomain);
            }
        }

        /// <summary>
        /// Отправить файл на рест-сервис
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="senderId"></param>
        /// <param name="fileStorageName"></param>
        /// <returns></returns>
        public string UploadSmallFile(FileInfo fileInfo, string senderId, FileStorageName fileStorageName)
        {
            var configProvider = this.Container.Resolve<IGkhConfigProvider>();
            var fileManager = Container.Resolve<IFileManager>();

            try
            {
                var gisIntegrationConfig = configProvider.Get<GisIntegrationConfig>();

                var serviceAddress = string.Format(
                    "{0}/ext-bus-file-store-service/rest/{1}",
                    gisIntegrationConfig.GetServiceAddress(IntegrationService.File, false, "http://127.0.0.1:8080"),
                    fileStorageName.GetEnumMeta().Display);

                var webRequest = (HttpWebRequest)WebRequest.Create(serviceAddress);
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                webRequest.Headers.Add("Content-MD5", GetMD5Hash(fileInfo));
                webRequest.Headers.Add("X-Upload-Dataprovider", senderId);
                webRequest.Headers.Add("X-Upload-Filename", fileInfo.FullName);
                webRequest.Method = "PUT";

                // test
                // SDldfls4lz5@!82d
                if (gisIntegrationConfig.UseLoginCredentials)
                {
                    webRequest.Credentials = new NetworkCredential(gisIntegrationConfig.Login, gisIntegrationConfig.Password);
                }

                webRequest.Date = DateTime.Now;

                var reqStream = webRequest.GetRequestStream();
                using (var fileStream = fileManager.GetFile(fileInfo))
                {
                    fileStream.CopyTo(reqStream);
                }

                reqStream.Close();

                var resp = webRequest.GetResponse();
                return resp.Headers["X-Upload-UploadID"];
            }
            finally
            {
                Container.Release(configProvider);
                Container.Release(fileManager);
            }
        }

        /// <summary>
        /// Получить хэш файла по алгоритму МД5
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private string GetMD5Hash(FileInfo fileInfo)
        {
            var fileManager = Container.Resolve<IFileManager>();

            try
            {
                byte[] hash;

                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    using (var stream = fileManager.GetFile(fileInfo))
                    {
                        hash = md5.ComputeHash(stream);
                    }
                }

                return Convert.ToBase64String(hash);
            }
            finally
            {
                Container.Release(fileManager);
            }
        }

        /// <summary>
        /// Получить хэш файла по алгоритму ГОСТ
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private string GetGostHash(FileInfo fileInfo)
        {
            var fileManager = Container.Resolve<IFileManager>();

            try
            {
                byte[] hash;

                using (var gost = Gost3411.Create())
                {
                    using (var stream = fileManager.GetFile(fileInfo))
                    {
                        hash = gost.ComputeHash(stream);
                    }
                }

                var hex = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    hex.AppendFormat("{0:x2}", b);
                }
                    
                return hex.ToString();
            }
            finally
            {
                Container.Release(fileManager);
            }
        }
    }
}
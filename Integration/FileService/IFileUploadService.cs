namespace Bars.Gkh.Ris.Integration.FileService
{
    using B4.Modules.FileStorage;
    using Entities;
    using Enums;

    /// <summary>
    /// Сервис для сохранения вложений файлов на рест-сервис
    /// </summary>
    public interface IFileUploadService
    {
        /// <summary>
        /// Отправить файл на рест-сервис и сохранить полученный гуид в БД
        /// </summary>
        /// <param name="file">Файл</param>
        /// <param name="description">Описание</param>
        /// <param name="fileStorageName">Наименование хранилища</param>
        /// <param name="senderId">SenderId поставщика</param>
        /// <returns>Файл вложения</returns>
        Attachment SaveAttachment(FileInfo file, string description, FileStorageName fileStorageName, string senderId);

        /// <summary>
        /// Отправить файл на рест-сервис
        /// </summary>
        /// <param name="fileInfo">Файл</param>
        /// <param name="senderId">ID отправителя</param>
        /// <param name="fileStorageName">Наименование хранилища</param>
        /// <returns></returns>
        string UploadSmallFile(FileInfo fileInfo, string senderId, FileStorageName fileStorageName);
    }
}
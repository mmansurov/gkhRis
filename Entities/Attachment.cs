namespace Bars.Gkh.Ris.Entities
{
    using B4.DataAccess;
    using B4.Modules.FileStorage;

    /// <summary>
    /// Файл-вложение
    /// </summary>
    public class Attachment : BaseEntity
    {
        /// <summary>
        /// Ссылка на b4_file_info
        /// </summary>
        public virtual FileInfo FileInfo { get; set; }

        /// <summary>
        /// Идентификатор файла в ГИС для передачи в методах
        /// </summary>
        public virtual string Guid { get; set; }

        /// <summary>
        /// Хэш файла по алгоритму Gost
        /// </summary>
        public virtual string Hash { get; set; }

        /// <summary>
        /// Наименование вложения
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Описание вложения
        /// </summary>
        public virtual string Description { get; set; }
    }
}
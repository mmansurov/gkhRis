namespace Bars.Gkh.Ris.Tasks
{
    using System.Collections.Generic;
    using System.Linq;

    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Результат обработки пакета данных
    /// </summary>
    public class PackageProcessingResult
    {
        /// <summary>
        /// Статус
        /// </summary>
        public PackageProcessingState State { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Результаты обработки объектов
        /// </summary>
        public List<ObjectProcessingResult> Objects { get; set; }

        /// <summary>
        /// Общее количество объектов
        /// </summary>
        public int ObjectsCount {
            get
            {
                if (this.State == PackageProcessingState.Success)
                {
                    return this.Objects.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Количество успешно обработанных объектов
        /// </summary>
        public int SuccessProcessedObjectsCount {
            get
            {
                if (this.State == PackageProcessingState.Success)
                {
                    return this.Objects.Count(x => x.State == ObjectProcessingState.Success);
                }

                return 0;
            }
        }
    }
}

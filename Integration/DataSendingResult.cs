namespace Bars.Gkh.Ris.Integration
{
    using System.Collections.Generic;
    using System.Linq;

    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Результат отправки данных на обработку
    /// </summary>
    public class DataSendingResult
    {
        /// <summary>
        /// Запланированная задача обработки данных
        /// </summary>
        public RisTask Task { get; set; }

        /// <summary>
        /// Результаты отправки пакетов
        /// </summary>
        public List<PackageSendingResult> PackageSendingResults { get; set; }

        /// <summary>
        /// Статус отправки данных на обработку
        /// </summary>
        public DataSendingState State {
            get
            {
                var errorsCount = this.PackageSendingResults.Count(x => x.Success == false);
                var totalCount = this.PackageSendingResults.Count;

                if (this.Task != null && errorsCount == 0)
                {
                    return DataSendingState.Success;
                }

                if (this.Task == null && errorsCount == totalCount)
                {
                    return DataSendingState.Error;
                }

                return DataSendingState.WithErrors;
            }
        }
    }
}

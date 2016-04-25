using System;
namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Статус управляемого объекта в ГИС ЖКХ
    /// </summary>
    public enum RisContractObjectStatus
    {
        /// <summary>
        /// Проект
        /// </summary>
        [Display("Проект")]
        Project = 10,

        /// <summary>
        /// На утверждении
        /// </summary>
        [Display("На утверждении")]
        ApprovalProcess = 20,

        /// <summary>
        /// Проект
        /// </summary>
        [Display("Отклонен")]
        Rejected = 30,

        /// <summary>
        /// Утвержден
        /// </summary>
        [Display("Утвержден")]
        Approved = 40,

        /// <summary>
        /// Заблокирован
        /// </summary>
        [Display("Заблокирован")]
        Locked = 50
    }
}

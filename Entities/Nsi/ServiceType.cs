namespace Bars.Gkh.Ris.Entities.Nsi
{
    using GisIntegration.Ref;

    /// <summary>
    /// Запись справочника «Вид работ»
    /// </summary>
    public class ServiceType : BaseRisEntity
    {
        /// <summary>
        /// Запись справочника
        /// </summary>
        public virtual GisDictRef GisDictRef { get; set; }
    }
}

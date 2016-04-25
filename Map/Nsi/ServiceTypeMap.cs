namespace Bars.Gkh.Ris.Map.Nsi
{
    using Entities.Nsi;
    using Bars.B4.Modules.Mapping.Mappers;

    /// <summary>
    /// Маппинг сущности ServiceType
    /// </summary>
    public class ServiceTypeMap : BaseEntityMap<ServiceType>
    {
        public ServiceTypeMap()
            : base("Bars.Gkh.Ris.Entities.Nsi.ServiceType", "RIS_SERVICE_TYPE")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.GisDictRef, "GisDictRef").Column("GISDICTREF_ID").Fetch();
        }
    }
}

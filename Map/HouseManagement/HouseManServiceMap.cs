namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.HouseManService"
    /// </summary>
    public class HouseManServiceMap : BaseRisEntityMap<HouseManService>
    {
        public HouseManServiceMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.HouseServiceMap", "HOUSE_SERVICE")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.ContractObject, "ContractObject").Column("CONTRACTOBJECT_ID").NotNull().Fetch();
            this.Property(x => x.ServiceTypeCode, "ServiceTypeCode").Column("SERVICETYPE_CODE").Length(50);
            this.Property(x => x.ServiceTypeGuid, "ServiceTypeGuid").Column("SERVICETYPE_GUID").Length(50);
            this.Property(x => x.StartDate, "StartDate").Column("STARTDATE");
            this.Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            this.Property(x => x.BaseServiceCurrentDoc, "BaseServiceCurrentDoc").Column("BASESERVICE_CURRENTDOC");
            this.Reference(x => x.BaseServiceAgreement, "BaseServiceAgreement").Column("BASESERVICE_AGREEMENT_ID").Fetch();
        }
    }
}

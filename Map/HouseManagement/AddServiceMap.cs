namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.AddService"
    /// </summary>
    public class AddServiceMap : BaseRisEntityMap<AddService>
    {
        public AddServiceMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.AddService", "RIS_ADDSERVICE")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.ContractObject, "ContractObject").Column("CONTRACTOBJECT_ID").NotNull().Fetch();
            this.Property(x => x.ServiceTypeCode, "ServiceTypeCode").Column("SERVICETYPE_CODE").Length(50);
            this.Property(x => x.ServiceTypeGuid, "ServiceTypeGuid").Column("SERVICETYPE_GUID").Length(50);
            this.Property(x => x.StartDate, "StartDate").Column("STARTDATE");
            this.Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            this.Property(x => x.BaseServiceCurrentDoc, "BaseServiseCharterCurrentCharter").Column("BASESERVISECHARTER_CURRENTCHARTER");
            this.Reference(x => x.BaseServiceCharterProtocolMeetingOwner, "BaseServiseCharterProtocolMeetingOwner")
                .Column("BASESERVICECHARTER_PROTOCOLMEETINGOWNER_ID").Fetch();
            this.Reference(x => x.BaseServiceAgreement, "BaseServiseAgreement").Column("BASESERVISE_AGREEMENT_ID").Fetch();
        }
    }
}
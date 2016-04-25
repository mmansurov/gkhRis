namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.ContractObject"
    /// </summary>
    public class ContractObjectMap : BaseRisEntityMap<ContractObject>
    {
        public ContractObjectMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.ContractObject", "RIS_CONTRACTOBJECT")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.Charter, "Charter").Column("CHARTER_ID").Fetch();
            this.Reference(x => x.House, "House").Column("HOUSE_ID").Fetch();
            this.Property(x => x.StartDate, "StartDate").Column("STARTDATE");
            this.Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            this.Property(x => x.BaseMServiseCurrentCharter, "BaseMServiseCurrentCharter").Column("BASEMSERVISE_CURRENTCHARTER");
            this.Reference(x => x.BaseMServiseProtocolMeetingOwner, "BaseMServiseProtocolMeetingOwner")
                .Column("BASEMSERVISE_PROTOCOLMEETINGOWNER_ID").Fetch();
            this.Property(x => x.DateExclusion, "DateExclusion").Column("DATEEXCLUSION");
            this.Property(x => x.BaseExclusionCurrentCharter, "BaseExclusionCurrentCharter").Column("BASEEXCLUSION_CURRENTCHARTER");
            this.Reference(x => x.BaseExclusionProtocolMeetingOwner, "BaseExclusionProtocolMeetingOwner")
                .Column("BASEEXCLUSION_PROTOCOLMEETINGOWNER_ID").Fetch();
            this.Reference(x => x.Contract, "Contract").Column("CONTRACT_ID").Fetch();
            this.Property(x => x.StatusObject, "StatusObject").Column("STATUSOBJECT");
            this.Reference(x => x.BaseExclusionAgreement, "BaseExclusionAgreement").Column("BASEEXCLUSION_AGREEMENT_ID").Fetch();
            this.Reference(x => x.BaseMServiseAgreement, "BaseMServiseAgreement").Column("BASEMSERVISE_AGREEMENT_ID").Fetch();
            this.Property(x => x.PeriodMeteringStartDate, "PeriodMeteringStartDate").Column("PERIODMETERING_STARTDATE");
            this.Property(x => x.PeriodMeteringEndDate, "PeriodMeteringEndDate").Column("PERIODMETERING_ENDDATE");
            this.Property(x => x.PeriodMeteringLastDay, "PeriodMeteringLastDay").Column("PERIODMETERING_LASTDAY");
            this.Property(x => x.PaymentDateStartDate, "PaymentDateStartDate").Column("PAYMENTDATE_STARTDATE");
            this.Property(x => x.PaymentDateLastDay, "PaymentDateLastDay").Column("PAYMENTDATE_LASTDAY");
            this.Property(x => x.PaymentDateCurrentMounth, "PaymentDateCurrentMounth").Column("PAYMENTDATE_CURRENTMOUNTH");
            this.Property(x => x.PaymentDateNextMounth, "PaymentDateNextMounth").Column("PAYMENTDATE_NEXTMOUNTH");
            this.Property(x => x.RealityObjectId, "RealityObjectId").Column("REALITYOBJECT_ID");
            this.Property(x => x.FiasHouseGuid, "FiasHouseGuid").Column("FIASHOUSE_GUID");
        }
    }
}
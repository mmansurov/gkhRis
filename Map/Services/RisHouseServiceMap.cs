namespace Bars.Gkh.Ris.Map.Services
{
    using Entities.Services;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Services.RisHouseService"
    /// </summary>
    public class RisHouseServiceMap : BaseRisEntityMap<RisHouseService>
    {
        public RisHouseServiceMap() :
            base("Bars.Gkh.Ris.Entities.Services.RisHouseService", "RIS_HOUSESERVICE")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.MsTypeCode, "MsTypeCode").Column("SERVICETYPE_CODE").Length(200);
            this.Property(x => x.MsTypeGuid, "MsTypeGuid").Column("SERVICETYPE_GUID").Length(200);
            this.Property(x => x.StartDate, "StartDate").Column("STARTDATE");
            this.Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            this.Property(x => x.AccountingTypeCode, "AccountingTypeCode").Column("ACCOUNTINGTYPE_CODE").Length(200);
            this.Property(x => x.AccountingTypeGuid, "AccountingTypeGuid").Column("ACCOUNTINGTYPE_GUID").Length(200);

            this.Reference(x => x.House, "House").Column("HOUSE_ID").Fetch();
        }
    }
}
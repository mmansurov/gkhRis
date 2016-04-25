namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisEcnbr"
    /// </summary>
    public class RisEcnbrMap : BaseRisEntityMap<RisEcnbr>
    {
        public RisEcnbrMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisEcnbr", "RIS_ECNBR")
        {
        }

        protected override void Map()
        {
            Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            Property(x => x.KindCode, "KindCode").Column("KINDCODE").Length(50);
            Property(x => x.KindGuid, "KindGuid").Column("KINDGUID").Length(50);
            Reference(x => x.RisEcnbrContragent, "Contragent").Column("CONTRAGENT_ID").Fetch();
            Reference(x => x.Share, "Share").Column("SHARE_ID").Fetch();
        }
    }
}

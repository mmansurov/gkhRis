namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using GisIntegration;
    using Entities.HouseManagement;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisProtocolOk"
    /// </summary>
    public class RisProtocolOkMap : BaseRisEntityMap<RisProtocolOk>
    {
        public RisProtocolOkMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisProtocolOk", "RIS_PROTOCOLOK")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").Fetch();
            this.Reference(x => x.Contract, "Contract").Column("CONTRACT_ID").Fetch();
        }
    }
}

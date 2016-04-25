namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using B4.Modules.Mapping.Mappers;
    using Entities.HouseManagement;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisAgreement"
    /// </summary>
    public class RisAgreementMap : BaseEntityMap<RisAgreement>
    {
        public RisAgreementMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisAgreement", "RIS_AGREEMENT")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.AgreementNumber, "AgreementNumber").Column("AGREEMENTNUMBER").Length(200);
            this.Property(x => x.AgreementDate, "AgreementDate").Column("AGREEMENTDATE");
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").Fetch();
            this.Reference(x => x.Contract, "Contract").Column("CONTRACT_ID").Fetch();
        }
    }
}

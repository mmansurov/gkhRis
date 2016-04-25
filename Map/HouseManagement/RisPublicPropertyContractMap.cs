namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisPublicPropertyContract"
    /// </summary>
    public class RisPublicPropertyContractMap : BaseRisEntityMap<RisPublicPropertyContract>
    {
        public RisPublicPropertyContractMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisPublicPropertyContract", "RIS_PUBLICPROPERTYCONTRACT")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.ProtocolNumber, "ProtocolNumber").Column("PROTOCOLNUMBER").Length(200);
            this.Property(x => x.ProtocolDate, "ProtocolDate").Column("PROTOCOLDATE");
            this.Property(x => x.StartDate, "StartDate").Column("STARTDATE");
            this.Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            this.Property(x => x.ContractNumber, "ContractNumber").Column("CONTRACTNUMBER").Length(50);
            this.Property(x => x.ContractObject, "ContractObject").Column("CONTRACTOBJECT").Length(200);
            this.Property(x => x.Comments, "Comments").Column("COMMENTS").Length(200);
            this.Property(x => x.DateSignature, "DateSignature").Column("DATESIGNATURE");
            this.Property(x => x.IsSignatured, "IsSignatured").Column("ISSIGNATURED");
            this.Reference(x => x.House, "House").Column("HOUSE_ID").Fetch();
            this.Reference(x => x.Entrepreneur, "Entrepreneur").Column("ENTREPRENEUR_ID").Fetch();
            this.Reference(x => x.Organization, "Organization").Column("ORGANIZATION_ID").Fetch();
        }
    }
}

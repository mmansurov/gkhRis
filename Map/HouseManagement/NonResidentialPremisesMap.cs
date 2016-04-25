namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.NonResidentialPremises"
    /// </summary>
    public class NonResidentialPremisesMap : BaseRisEntityMap<NonResidentialPremises>
    {
        public NonResidentialPremisesMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.NonResidentialPremises", "RIS_NONRESIDENTIALPREMISES")
        {
        }

        protected override void Map()
        {
            Reference(x => x.ApartmentHouse, "ApartmentHouse").Column("HOUSE_ID").Fetch();
            Property(x => x.PremisesNum, "PremisesNum").Column("PREMISESNUM").Length(50);
            Property(x => x.CadastralNumber, "CadastralNumber").Column("CADASTRALNUMBER").Length(50);
            Property(x => x.Floor, "Floor").Column("FLOOR").Length(50);
            Property(x => x.PrevStateRegNumberCadastralNumber, "PrevStateRegNumberCadastralNumber")
                .Column("PREVSTATEREGNUMBER_CADASTRALNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberInventoryNumber, "PrevStateRegNumberInventoryNumber")
                .Column("PREVSTATEREGNUMBER_INVENTORYNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberConditionalNumber, "PrevStateRegNumberConditionalNumber")
                .Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER").Length(50);
            Property(x => x.TerminationDate, "TerminationDate").Column("TERMINATIONDATE");
            Property(x => x.PurposeCode, "PurposeCode").Column("PURPOSE_CODE").Length(50);
            Property(x => x.PurposeGuid, "PurposeGuid").Column("PURPOSE_GUID").Length(50);
            Property(x => x.PositionCode, "PositionCode").Column("POSITION_CODE").Length(50);
            Property(x => x.PositionGuid, "PositionGuid").Column("POSITION_GUID").Length(50);
            Property(x => x.GrossArea, "GrossArea").Column("GROSSAREA");
            Property(x => x.TotalArea, "TotalArea").Column("TOTALAREA");
            Property(x => x.IsCommonProperty, "IsCommonProperty").Column("ISCOMMONPROPERTY");
        }
    }
}
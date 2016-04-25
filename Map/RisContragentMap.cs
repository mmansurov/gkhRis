namespace Bars.Gkh.Ris.Map
{
    using Bars.B4.Modules.Mapping.Mappers;
    using Bars.Gkh.Ris.Entities;
    
    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.RisContragent"
    /// </summary>
    public class RisContragentMap : BaseEntityMap<RisContragent>
    {
        public RisContragentMap() : 
                base("Bars.Gkh.Ris.Entities.RisContragent", "RIS_CONTRAGENT")
        {
        }
        
        protected override void Map()
        {
            Property(x => x.GkhId, "GkhId").Column("GKHID");
            Property(x => x.FullName, "FullName").Column("FULLNAME").Length(1000);
            Property(x => x.Ogrn, "Ogrn").Column("OGRN").Length(50);
            Property(x => x.OrgRootEntityGuid, "OrgRootEntityGuid").Column("ORGROOTENTITYGUID").Length(50);
            Property(x => x.OrgVersionGuid, "OrgVersionGuid").Column("ORGVERSIONGUID").Length(50);
            Property(x => x.SenderId, "SenderId").Column("SENDERID").Length(50);
            Property(x => x.FactAddress, "FactAddress").Column("FACT_ADDRESS").Length(500);
            Property(x => x.JuridicalAddress, "JuridicalAddress").Column("JUR_ADDRESS").Length(500);
            Property(x => x.IsIndividual, "IsIndividual").Column("IS_INDIVIDUAL");
        }
    }
}
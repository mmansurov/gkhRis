namespace Bars.Gkh.Ris.Map
{
    using B4.Modules.Mapping.Mappers;
    using Entities;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.RisContainer"
    /// </summary>
    public class RisContainerMap : PersistentObjectMap<RisContainer>
    {
        public RisContainerMap() : 
                base("Bars.Gkh.Ris.Entities.RisContainer", "RIS_CONTAINER")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.RisContragent, "RisContragent").Column("CONTRAGENT_ID").Fetch();
            this.Property(x => x.Date, "Date").Column("CONTAINER_DATE").NotNull();
            this.Property(x => x.UploadDate, "UploadDate").Column("UPLOAD_DATE");
            this.Property(x => x.MethodCode, "MethodCode").Column("METHOD_CODE");
        }
    }
}
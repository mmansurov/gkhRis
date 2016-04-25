namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using Entities.Infrastructure;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisNetPieces"
    /// </summary>
    public class RisNetPiecesMap : BaseRisEntityMap<RisNetPieces>
    {
        public RisNetPiecesMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisNetPieces", "RIS_NET_PIECES")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.RkiItem, "RkiItem").Column("RKIITEM_ID").Fetch();
            this.Property(x => x.Name, "Name").Column("NAME").Length(200);
            this.Property(x => x.Diameter, "Diameter").Column("DIAMETER");
            this.Property(x => x.Length, "Length").Column("LENGTH");
            this.Property(x => x.NeedReplaced, "NeedReplaced").Column("NEEDREPLACED");
            this.Property(x => x.Wearout, "Wearout").Column("WEAROUT");
            this.Property(x => x.PressureCode, "PressureCode").Column("PRESSURECODE").Length(200);
            this.Property(x => x.PressureGuid, "PressureGuid").Column("PRESSUREGUID").Length(200);
            this.Property(x => x.PressureName, "PressureName").Column("PRESSURENAME").Length(200);
            this.Property(x => x.VoltageCode, "VoltageCode").Column("VOLTAGECODE").Length(200);
            this.Property(x => x.VoltageGuid, "VoltageGuid").Column("VOLTAGEGUID").Length(200);
            this.Property(x => x.VoltageName, "VoltageName").Column("VOLTAGENAME").Length(200);
        }
    }
}

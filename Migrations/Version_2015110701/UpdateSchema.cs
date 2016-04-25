namespace Bars.Gkh.Ris.Migrations.Version_2015110701
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015110701")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015110700.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddRisEntityTable("RIS_SHARE",
                new Column("ISPRIVATIZED", DbType.Boolean),
                new Column("TERMDATE", DbType.DateTime),
                new RefColumn("CONTRAGENT_ID", "RIS_SHARE_CONTRAGENT", "RIS_CONTRAGENT", "ID"),
                new RefColumn("ACCOUNT_ID", "RIS_SHARE_ACCOUNT", "RIS_ACCOUNT", "ID"));

            Database.AddRisEntityTable("RIS_ECNBR",
                new Column("ENDDATE", DbType.DateTime),
                new Column("KINDCODE", DbType.String, 50),
                new Column("KINDGUID", DbType.String, 50),
                new RefColumn("CONTRAGENT_ID", "RIS_ECNBR_CONTRAGENT", "RIS_CONTRAGENT", "ID"),
                new RefColumn("SHARE_ID", "RIS_ECNBR_SHARE", "RIS_SHARE", "ID"));

            Database.AddRisEntityTable("RIS_ECNBRIND",
                new RefColumn("ECNBR_ID", "RIS_ECNBRIND_ECNBR", "RIS_ECNBR", "ID"),
                new RefColumn("IND_ID", "RIS_ECNBRIND_IND", "RIS_IND", "ID"));

            Database.AddRisEntityTable("RIS_SHAREIND",
                new RefColumn("SHARE_ID", "RIS_SHAREIND_SHARE", "RIS_SHARE", "ID"),
                new RefColumn("IND_ID", "RIS_SHAREIND_IND", "RIS_IND", "ID"));

            Database.AddRisEntityTable("RIS_SHAREECNBRNONRESPREM",
                new Column("INTPART", DbType.String, 50),
                new Column("FRACPART", DbType.String, 50),
                new RefColumn("ECNBR_ID", "RIS_SHARECNBRNRESPR_ECNBR", "RIS_ECNBR", "ID"),
                new RefColumn("SHARE_ID", "RIS_SHARECNBRNRESPR_SHARE", "RIS_SHARE", "ID"),
                new RefColumn("NONRESIDENTIALPREMISES_ID", "RIS_SHARECNBRNRESPR_PREM", "RIS_NONRESIDENTIALPREMISES", "ID"));

            Database.AddRisEntityTable("RIS_SHAREECNBRLIVINGROOM",
                new Column("INTPART", DbType.String, 50),
                new Column("FRACPART", DbType.String, 50),
                new RefColumn("ECNBR_ID", "RIS_SHARECNBRLR_ECNBR", "RIS_ECNBR", "ID"),
                new RefColumn("SHARE_ID", "RIS_SHARECNBRLR_SHARE", "RIS_SHARE", "ID"),
                new RefColumn("LIVINGROOM_ID", "RIS_SHARECNBRLR_LR", "RIS_LIVINGROOM", "ID"));

            Database.AddRisEntityTable("RIS_SHAREECNBRRESIDENTPREM",
                new Column("INTPART", DbType.String, 50),
                new Column("FRACPART", DbType.String, 50),
                new RefColumn("ECNBR_ID", "RIS_SHARECNBRRESPR_ECNBR", "RIS_ECNBR", "ID"),
                new RefColumn("SHARE_ID", "RIS_SHARECNBRRESPR_SHARE", "RIS_SHARE", "ID"),
                new RefColumn("RESIDENTPREM_ID", "RIS_SHARECNBRRESPR_RP", "RIS_RESIDENTIALPREMISES", "ID"));

            Database.AddRisEntityTable("RIS_SHAREECNBRLIVINGHOUSE",
                new Column("INTPART", DbType.String, 50),
                new Column("FRACPART", DbType.String, 50),
                new RefColumn("ECNBR_ID", "RIS_SHARECNBRLH_ECNBR", "RIS_ECNBR", "ID"),
                new RefColumn("SHARE_ID", "RIS_SHARECNBRLH_SHARE", "RIS_SHARE", "ID"),
                new  RefColumn("HOUSE_ID", "RIS_SHARECNBRLH_HOUSE", "RIS_HOUSE", "ID"));

            if (!Database.ColumnExists("RIS_IND", "ISREGISTERED"))
            {
                Database.AddColumn("RIS_IND", new Column("ISREGISTERED", DbType.Boolean));
            }
            if (!Database.ColumnExists("RIS_IND", "ISRESIDES"))
            {
                Database.AddColumn("RIS_IND", new Column("ISRESIDES", DbType.Boolean));
            }

        }

        public override void Down()
        {
            Database.RemoveColumn("RIS_IND", "ISRESIDES");
            Database.RemoveColumn("RIS_IND", "ISREGISTERED");
            
            Database.RemoveTable("RIS_SHAREECNBRLIVINGHOUSE");
            Database.RemoveTable("RIS_SHAREECNBRRESIDENTPREM");
            Database.RemoveTable("RIS_SHAREECNBRLIVINGROOM");
            Database.RemoveTable("RIS_SHAREECNBRNONRESPREM");
            Database.RemoveTable("RIS_SHAREIND");
            Database.RemoveTable("RIS_ECNBRIND");
            Database.RemoveTable("RIS_ECNBR");
            Database.RemoveTable("RIS_SHARE");
        }
    }
}
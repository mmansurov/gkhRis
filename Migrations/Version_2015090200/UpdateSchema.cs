namespace Bars.Gkh.Ris.Migrations.Version_2015090200
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015090200")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015083100.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddEntityTable("RIS_CONTRAGENT",
                new Column("GKHID", DbType.Int64),
                new Column("FULLNAME", DbType.String, 1000),
                new Column("OGRN", DbType.String, 50),
                new Column("ORGROOTENTITYGUID", DbType.String, 50),
                new Column("ORGVERSIONGUID", DbType.String, 50),
                new Column("SENDERID", DbType.String, 50));
        }

        public override void Down()
        {
            Database.RemoveTable("RIS_CONTRAGENT");
        }
    }
}
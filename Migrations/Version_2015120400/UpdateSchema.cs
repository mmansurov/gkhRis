namespace Bars.Gkh.Ris.Migrations.Version_2015120400
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015120400")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015113000.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddColumn("RIS_HOUSE", "USEDYEAR", DbType.Int16);
            Database.AddColumn("RIS_HOUSE", "FLOORCOUNT", DbType.String, 50);
            Database.AddColumn("RIS_HOUSE", "RESIDENTIALSQUARE", DbType.Decimal);
            Database.AddColumn("RIS_HOUSE", "CULTURALHERITAGE", DbType.Boolean);
            Database.AddColumn("RIS_HOUSE", "UNDERGROUNDFLOORCOUNT", DbType.String, 50);
            Database.AddColumn("RIS_HOUSE", "NONRESIDENTIALSQUARE", DbType.Decimal);

            Database.ChangeColumn("RIS_HOUSE", new Column("CADASTRALNUMBER", DbType.String, 300));
            Database.ChangeColumn("RIS_HOUSE", new Column("PROJECTSERIES", DbType.String, 300));

            Database.AddColumn("RIS_NONRESIDENTIALPREMISES", "FLOOR", DbType.String, 50);
            Database.AddColumn("RIS_NONRESIDENTIALPREMISES", "TOTALAREA", DbType.Decimal);
            Database.AddColumn("RIS_NONRESIDENTIALPREMISES", "ISCOMMONPROPERTY", DbType.Boolean);

            Database.AddColumn("RIS_RESIDENTIALPREMISES", "FLOOR", DbType.String, 50);
            Database.AddColumn("RIS_RESIDENTIALPREMISES", "TOTALAREA", DbType.Decimal);

            Database.AddColumn("RIS_LIVINGROOM", "FLOOR", DbType.String, 50);

            Database.AddRisEntityTable(
                "RIS_HOUSE_INNER_WALL_MATERIAL",
                new RefColumn("HOUSE_ID", "RIS_HOUSE_INNER_WALL_MATERIAL_HOUSE", "RIS_HOUSE", "ID"),
                new Column("INNERWALLMATERIALCODE", DbType.String, 50, ColumnProperty.NotNull),
                new Column("INNERWALLMATERIALGUID", DbType.String, 50, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveColumn("RIS_HOUSE", "USEDYEAR");
            Database.RemoveColumn("RIS_HOUSE", "FLOORCOUNT");
            Database.RemoveColumn("RIS_HOUSE", "RESIDENTIALSQUARE");
            Database.RemoveColumn("RIS_HOUSE", "CULTURALHERITAGE");
            Database.RemoveColumn("RIS_HOUSE", "UNDERGROUNDFLOORCOUNT");
            Database.RemoveColumn("RIS_HOUSE", "NONRESIDENTIALSQUARE");

            Database.RemoveColumn("RIS_NONRESIDENTIALPREMISES", "FLOOR");
            Database.RemoveColumn("RIS_NONRESIDENTIALPREMISES", "TOTALAREA");
            Database.RemoveColumn("RIS_NONRESIDENTIALPREMISES", "ISCOMMONPROPERTY");

            Database.RemoveColumn("RIS_RESIDENTIALPREMISES", "FLOOR");
            Database.RemoveColumn("RIS_RESIDENTIALPREMISES", "TOTALAREA");

            Database.RemoveColumn("RIS_LIVINGROOM", "FLOOR");

            Database.RemoveTable("RIS_HOUSE_INNER_WALL_MATERIAL");
        }
    }
}


namespace Bars.Gkh.Ris.Migrations.Version_1
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("1")]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddEntityTable("RIS_INTEGR_METHOD",
                new Column("NAME", DbType.String, 1000, ColumnProperty.NotNull),
                new Column("DESCRIPTION", DbType.String, 1000),
                new Column("DATE_INTEG", DbType.DateTime),
                new Column("METHODCODE", DbType.String, 1000, ColumnProperty.NotNull),
                new Column("SERVICE_ADDRESS", DbType.String, 1000, ColumnProperty.NotNull));

            Database.AddEntityTable("RIS_INTEGR_DICT",
                new Column("NAME", DbType.String, 500),
                new Column("ACTION_CODE", DbType.String, 500),
                new Column("REGISTRY_NUMBER", DbType.String, 50),
                new Column("DATE_INTEG", DbType.DateTime));

            Database.AddEntityTable("RIS_INTEGR_LOG",
                new RefColumn("FILELOG_ID", "RIS_FILE_LOG", "B4_FILE_INFO", "ID"),
                new Column("LINK", DbType.String, 2000),
                new Column("METHOD_NAME", DbType.String, 300),
                new Column("USER_NAME", DbType.String, 300),
                new Column("COUNT_OBJECTS", DbType.Int32, ColumnProperty.NotNull, 0),
                new Column("DATE_START", DbType.DateTime),
                new Column("DATE_END", DbType.DateTime),
                new Column("PROCESSED_OBJECTS", DbType.Int32),
                new Column("PROCESSED_PERCENT", DbType.Decimal));

            Database.AddEntityTable("RIS_INTEGR_REF_DICT",
                new RefColumn("DICT_ID", "RIS_INTEGR_REF_DICT", "RIS_INTEGR_DICT", "ID"),
                new Column("CLASS_NAME", DbType.String, 1000, ColumnProperty.NotNull),
                new Column("GIS_REC_ID", DbType.String, 10),
                new Column("GIS_REC_NAME", DbType.String, 1000),
                new Column("GKH_REC_ID", DbType.Int64),
                new Column("GKH_REC_NAME", DbType.String, 1000));
        }

        public override void Down()
        {
            Database.RemoveTable("RIS_INTEGR_REF_DICT");

            Database.RemoveTable("RIS_INTEGR_LOG");

            Database.RemoveTable("RIS_INTEGR_DICT");

            Database.RemoveTable("RIS_INTEGR_METHOD");
        }
    }
}
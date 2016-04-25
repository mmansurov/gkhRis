namespace Bars.Gkh.Ris.Migrations.Version_2016021000
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;

    [Migration("2016021000")]
    [MigrationDependsOn(typeof(Version_2016020100.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Накатить миграцию
        /// </summary>
        public override void Up()
        {
            if (!this.Database.ColumnExists("RIS_CONTRACT", "LICENSE_REQUEST"))
            {
                this.Database.AddColumn("RIS_CONTRACT", new Column("LICENSE_REQUEST", DbType.Boolean, ColumnProperty.NotNull, false));
            }
        }

        /// <summary>
        /// Откатить миграцию
        /// </summary>
        public override void Down()
        {
            if (this.Database.ColumnExists("RIS_CONTRACT", "LICENSE_REQUEST"))
            {
                this.Database.RemoveColumn("RIS_CONTRACT", "LICENSE_REQUEST");
            }
        }
    }
}

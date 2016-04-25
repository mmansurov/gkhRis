namespace Bars.Gkh.Ris.Migrations.Version_2016021700
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016021700")]
    [MigrationDependsOn(typeof(Version_2016021600.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            this.Database.AddColumn("RIS_PACKAGE", "NAME", DbType.String, 250);           
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            this.Database.RemoveColumn("RIS_PACKAGE", "NAME");
        }
    }
}

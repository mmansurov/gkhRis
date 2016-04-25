namespace Bars.Gkh.Ris.Migrations.Version_2016022400
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;

    using Bars.B4.Modules.NH.Migrations.DatabaseExtensions;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016022400")]
    [MigrationDependsOn(typeof(Version_2016021001.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            this.Database.AddRefColumn("RIS_PACKAGE", new RefColumn("CONTRAGENT_ID", "RIS_PACK_CONTR", "RIS_CONTRAGENT", "ID"));
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            this.Database.RemoveColumn("RIS_PACKAGE", "CONTRAGENT_ID");
        }
    }
}
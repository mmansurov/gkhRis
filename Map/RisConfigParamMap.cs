namespace Bars.Gkh.Ris.Map
{
    using Bars.B4.DataAccess.UserTypes;
    using Bars.B4.Modules.Mapping.Mappers;
    using Entities;
    using NHibernate.Mapping.ByCode.Conformist;

    /// <summary>Маппинг для "Параметр конфигурации"</summary>
    public class RisConfigParamMap : BaseEntityMap<RisConfigParam>
    {

        public RisConfigParamMap() :
                base("Параметр конфигурации", "RIS_CONFIG_PARAMETER")
        {
        }

        protected override void Map()
        {
            Property(x => x.Key, "Имя параметра").Column("KEY").Length(500).NotNull();
            Property(x => x.Value, "Значение").Column("VALUE");
        }
    }

    public class RisConfigParamNHibernateMapping : ClassMapping<RisConfigParam>
    {
        public RisConfigParamNHibernateMapping()
        {
            Property(
                x => x.Value,
                m =>
                {
                    m.Type<BinaryStringType>();
                });
        }
    }
}
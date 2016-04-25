namespace Bars.Gkh.Ris.Interceptors.GisIntegration
{
    using System.Linq;
    using B4;
    using B4.DataAccess;
    using B4.Utils;
    using DomainService.GisIntegration;
    using Entities.GisIntegration;
    using Entities.GisIntegration.Ref;
    using Integration.Nsi;

    public class GisDictInterceptor : EmptyDomainInterceptor<GisDict>
    {
        public override IDataResult AfterCreateAction(IDomainService<GisDict> service, GisDict entity)
        {
            var actions = Container.ResolveAll<IGisIntegrDictAction>();
            var gisIntegrService = Container.Resolve<IGisIntegrationService>();

            try
            {
                var action = actions.FirstOrDefault(x => x.Code == entity.ActionCode);

                if (action == null)
                {
                    return Failure("Не удалось получить справочник ЖКХ");
                }

                action.Dict = entity;
                action.SoapClient = gisIntegrService.CreateNsiClient();

                action.Update();

                return Success();
            }
            finally
            {
                Container.Release(actions);
                Container.Release(gisIntegrService);
            }
        }

        public override IDataResult AfterUpdateAction(IDomainService<GisDict> service, GisDict entity)
        {
            var actions = Container.ResolveAll<IGisIntegrDictAction>();
            var gisIntegrService = Container.Resolve<IGisIntegrationService>();

            try
            {
                var action = actions.FirstOrDefault(x => x.Code == entity.ActionCode);

                if (action == null)
                {
                    return Failure("Не удалось получить справочник ЖКХ");
                }

                action.Dict = entity;
                action.SoapClient = gisIntegrService.CreateNsiClient();

                action.Update();

                return Success();
            }
            finally
            {
                Container.Release(actions);
                Container.Release(gisIntegrService);
            }
        }

        public override IDataResult BeforeDeleteAction(IDomainService<GisDict> service, GisDict entity)
        {
            var gisDictRefDomain = Container.ResolveDomain<GisDictRef>();

            try
            {
                gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.Id == entity.Id)
                    .Select(x => x.Id)
                    .ForEach(x => gisDictRefDomain.Delete(x));

                return Success();
            }
            finally
            {
                Container.Release(gisDictRefDomain);
            }
        }
    }
}
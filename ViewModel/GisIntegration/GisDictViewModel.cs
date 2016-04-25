namespace Bars.Gkh.Ris.ViewModel.GisIntegration
{
    using System.Linq;
    using B4;
    using B4.DataAccess;
    using B4.Utils;
    using Entities.GisIntegration;
    using Entities.GisIntegration.Ref;
    using Gkh.Domain;

    public class GisDictViewModel : BaseViewModel<GisDict>
    {
        public override IDataResult List(IDomainService<GisDict> domain, BaseParams baseParams)
        {
            var loadParams = GetLoadParam(baseParams);

            var gisDictRefDomain = Container.ResolveDomain<GisDictRef>();

            var gisDictRefDict = gisDictRefDomain.GetAll()
                .Where(x => x.Dict != null)
                .ToArray()
                .GroupBy(x => x.Dict.Id)
                .ToDictionary(x => x.Key, x => new
                {
                    CountRef = x.Count(),
                    CountNotRef = x.Count(y => y.GisId.IsEmpty() || y.GisName.IsEmpty())
                });

            try
            {
                var data = domain.GetAll()
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.ActionCode,
                        x.NsiRegistryNumber,
                        x.DateIntegration
                    })
                    .ToList()
                    .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.ActionCode,
                        x.NsiRegistryNumber,
                        x.DateIntegration,
                        CountRefRecords = gisDictRefDict.ContainsKey(x.Id)
                            ? gisDictRefDict[x.Id].CountRef
                            : 0,
                        CountNotRefRecords = gisDictRefDict.ContainsKey(x.Id)
                            ? gisDictRefDict[x.Id].CountNotRef
                            : 0
                    })
                    .AsQueryable()
                    .Filter(loadParams, Container);

                return new ListDataResult(data.Order(loadParams).Paging(loadParams), data.Count());
            }
            finally
            {
                Container.Release(gisDictRefDomain);
            }
        }

        public override IDataResult Get(IDomainService<GisDict> domainService, BaseParams baseParams)
        {
            var id = baseParams.Params.GetAsId("Id");

            var rec = domainService.Get(id);

            return rec == null
                ? new BaseDataResult(null)
                : new BaseDataResult(new
                {
                    rec.Id,
                    rec.Name,
                    rec.ActionCode,
                    rec.NsiRegistryNumber,
                    rec.DateIntegration,
                    DictProxy = new
                    {
                        Id = rec.NsiRegistryNumber,
                        rec.Name
                    }
                });

        }
    }
}
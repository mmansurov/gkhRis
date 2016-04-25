namespace Bars.Gkh.Ris.ViewModel.GisIntegration
{
    using System.Linq;
    using B4;
    using B4.Utils;
    using Entities.GisIntegration.Ref;

    public class GisDictRefViewModel : BaseViewModel<GisDictRef>
    {
        public override IDataResult List(IDomainService<GisDictRef> domain, BaseParams baseParams)
        {
            var loadParams = GetLoadParam(baseParams);
            var dictId = loadParams.Filter.GetAs<long>("dictId");

            var data = domain.GetAll()
                .Where(x => x.Dict.Id == dictId)
                .OrderIf(loadParams.Order.Length == 0, true, x => x.ClassName)
                .Filter(loadParams, Container);

            return new ListDataResult(data.Order(loadParams).Paging(loadParams).ToList(), data.Count());
        }
    }
}
namespace Bars.Gkh.Ris.ViewModel.GisIntegration
{
    using System;
    using System.Linq;
    using B4;
    using B4.Utils;
    using Entities.GisIntegration;

    public class GisLogViewModel : BaseViewModel<GisLog>
    {
        public override IDataResult List(IDomainService<GisLog> domain, BaseParams baseParams)
        {
            var loadParams = GetLoadParam(baseParams);

            var data = domain.GetAll()
                .Select(x => new
                {
                    x.Id,
                    x.ServiceLink,
                    x.UserName,
                    x.MethodName,
                    x.DateStart,
                    x.DateEnd,
                    x.CountObjects,
                    x.ProcessedObjects,
                    x.ProcessedPercent,
                    x.FileLog
                })
                .Filter(loadParams, Container);

            var cnt = data.Count();

            var result = data.ToArray()
                .Select(x => new
                {
                    x.Id,
                    x.ServiceLink,
                    x.UserName,
                    x.MethodName,
                    DateStart = x.DateEnd ?? DateTime.MinValue,
                    TimeStart = (x.DateStart ?? DateTime.MinValue).ToString("HH:mm:ss"),
                    TimeWork = ((x.DateEnd ?? DateTime.MinValue) - (x.DateEnd ?? DateTime.MinValue)).ToDateTime().ToString("HH:mm:ss"),
                    DateEnd = x.DateEnd ?? DateTime.MinValue,
                    x.CountObjects,
                    x.ProcessedObjects,
                    x.ProcessedPercent,
                    x.FileLog
                })
                .AsQueryable()
                .Order(loadParams)
                .OrderIf(loadParams.Order.Length == 0, false, x => x.DateStart)
                .OrderThenIf(loadParams.Order.Length == 0, false, x => x.TimeStart);

            return new ListDataResult(result.Paging(loadParams).ToList(), cnt);
        }
    }
}
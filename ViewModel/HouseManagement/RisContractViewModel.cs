namespace Bars.Gkh.Ris.ViewModel.HouseManagement
{
    using B4;
    using Entities.HouseManagement;
    using System.Linq;
    using B4.DataAccess;
    using B4.Utils;

    using Bars.Gkh.Entities;

    public class RisContractViewModel : BaseViewModel<RisContract>
    {
        public override IDataResult List(IDomainService<RisContract> domain, BaseParams baseParams)
        {
            var loadParams = this.GetLoadParam(baseParams);
            var contractObjectDomain = this.Container.ResolveDomain<ContractObject>();
            var roDomain = this.Container.ResolveDomain<RealityObject>();

            try
            {
                var addressesByFiasGuid = roDomain.GetAll()
                    .Where(x => x.HouseGuid != "")
                    .Select(
                        x => new
                        {
                            x.HouseGuid,
                            x.Address
                        })
                    .ToDictionary(x => x.HouseGuid);

                var addressesByContractId = contractObjectDomain.GetAll()
                    .Where(x => x.Contract != null && x.FiasHouseGuid != "")
                    .Select(
                        x => new
                        {
                            ContractId = x.Contract.Id,
                            x.FiasHouseGuid
                        })
                    .ToList()
                    .Select(
                        x => new
                        {
                            x.ContractId,
                            Address = addressesByFiasGuid.Get(x.FiasHouseGuid).ReturnSafe(y => y.Address)
                        })
                    .GroupBy(x => x.ContractId)
                    .ToDictionary(x => x.Key, x => x.First().ReturnSafe(y => y.Address));

                var data = domain.GetAll()
                    .Select(
                        x => new
                        {
                            x.Id,
                            x.DocNum,
                            x.SigningDate,
                            OwnersType = x.OwnersType != null ? x.OwnersType.ToString() : string.Empty,
                            Organization = x.Org != null ? x.Org.FullName : string.Empty
                        })
                    .ToList();

                //todo: ускорить, если долго будет работать (сначала отбирать по фильтру адреса, если он есть)
                var result = data
                    .Select(
                        x => new
                        {
                            x.Id,
                            x.DocNum,
                            x.SigningDate,
                            x.Organization,
                            x.OwnersType,
                            Address = addressesByContractId.Get(x.Id) ?? ""
                        })
                    .AsQueryable()
                    .Filter(loadParams, this.Container);

                return new ListDataResult(result.Order(loadParams).Paging(loadParams), result.Count());
            }
            finally
            {
                this.Container.Release(contractObjectDomain);
                this.Container.Release(roDomain);
            }
        }
    }
}
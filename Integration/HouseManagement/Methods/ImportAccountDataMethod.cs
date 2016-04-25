namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.HouseManagement;
    using Enums;
    using Enums.HouseManagement;
    using Ris.HouseManagement;
    
    /// <summary>
    /// Метод передачи данных по домам
    /// </summary>
    public class ImportAccountDataMethod : GisIntegrationHouseManagementMethod<RisAccount, importAccountRequest>
    {
        private Dictionary<long, List<RisShare>> sharesToAccountDict = new Dictionary<long, List<RisShare>>();
        private List<string> shareRequestList = new List<string>();
        private readonly Dictionary<string, RisAccount> accountByTransportGuidDict = new Dictionary<string, RisAccount>();
        private readonly List<RisAccount> accountsToSave = new List<RisAccount>();

        public override string Code
        {
            get { return "importAccountData"; }
        }

        public override string Name
        {
            get { return "Импорт счетов"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 38;
            }
        }

        protected override int Portion
        {
            get { return 100; }
        }

        protected override int ProcessedObjects
        {
            get { return this.accountsToSave.Count; }
        }

        protected override IList<RisAccount> MainList { get; set; }

        protected override void Prepare()
        {
            var accDomain = this.Container.ResolveDomain<RisAccount>();
            var shareDomain = this.Container.ResolveDomain<RisShare>();

            try
            {
                this.MainList = accDomain.GetAll().ToList();

                this.sharesToAccountDict = shareDomain.GetAll()
                    .GroupBy(x => x.Account.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
            finally
            {
                this.Container.Release(accDomain);
                this.Container.Release(shareDomain);
            }
        }

        protected override importAccountRequest GetRequestObject(IEnumerable<RisAccount> listForImport)
        {
            var importRequestList = new List<importAccountRequestAccount>();

            foreach (var item in listForImport)
            {
                if (!this.CheckItem(item))
                {
                    continue;
                }

                var prepareResult = this.PrepareRequest(item);
                importRequestList.Add(prepareResult);
            }

            this.CountObjects += importRequestList.Count;

            return new importAccountRequest
            {
                Account = importRequestList.ToArray()
            };
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.accountByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var plan = this.accountByTransportGuidDict[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("План проверок", plan.Id, "Не загружен", errorNotation);
                return;
            }

            plan.Guid = responseItem.GUID;
            this.accountsToSave.Add(plan);

            this.AddLineToLog("План проверок", plan.Id, "Загружен", responseItem.GUID);
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.accountsToSave, 1000, true, true);
        }

        protected override ImportResult1 GetRequestResult1(importAccountRequest request)
        {
            ImportResult1 result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importAccountData(this.RequestHeader, request, out result);
            }

            return result;
        }

        private bool CheckItem(RisAccount acc)
        {
            bool result = true;

            var accNotation = new StringBuilder();

            if (acc.OwnerInd == null && acc.OwnerOrg == null && acc.RenterInd == null && acc.RenterOrg == null)
            {
                accNotation.Append("OWNERIND_ID/OWNERORG_ID/RENTERIND_ID/RENTERORG_ID ");
            }

            var risInd = acc.OwnerInd ?? acc.RenterInd;
            var risContragent = acc.OwnerOrg ?? acc.RenterOrg;

            var shares = this.sharesToAccountDict.ContainsKey(acc.Id)
                ? this.sharesToAccountDict[acc.Id]
                : null;

            this.shareRequestList = new List<string>();

            if (shares != null)
            {
                foreach (var share in shares)
                {
                    var shareNotation = new StringBuilder();

                    if (share.Guid.IsEmpty())
                    {
                        shareNotation.Append("SHARE_GUID ");
                    }

                    if (shareNotation.Length > 0)
                    {
                        this.AddLineToLog("Доля", share.Id, "Отсутствует гуид", shareNotation);
                        continue;
                    }

                    this.shareRequestList.Add(share.Guid);
                }
            }

            if (!this.shareRequestList.Any())
            {
                accNotation.Append("SHARE ");
            }

            if (risInd != null)
            {
                if (risInd.Surname.IsEmpty())
                {
                    accNotation.Append("SURNAME ");
                }
                if (risInd.FirstName.IsEmpty())
                {
                    accNotation.Append("FIRSTNAME ");
                }
                if (!risInd.DateOfBirth.HasValue)
                {
                    accNotation.Append("DATEOFBIRTH ");
                }
                if (risInd.Snils.IsEmpty())
                {
                    accNotation.Append("SNILS ");
                }
            }
            else if (risContragent != null)
            {
                if (risContragent.OrgVersionGuid.IsEmpty())
                {
                    accNotation.Append("ORGVERSIONGUID ");
                }
            }

            if (!acc.TotalSquare.HasValue)
            {
                accNotation.Append("TOTALSQUARE ");
            }

            if (acc.AccountNumber.IsEmpty())
            {
                accNotation.Append("ACCOUNTNUMBER ");
            }

            if (accNotation.Length > 0)
            {
                this.AddLineToLog("Счет", acc.Id, "Не загружен", accNotation);
                result = false;
            }

            return result;
        }

        private importAccountRequestAccount PrepareRequest(RisAccount acc)
        {
            var accRequest = new importAccountRequestAccount
            {
                TransportGUID = Guid.NewGuid().ToString(),
                Item = true,
                ItemElementName = acc.RisAccountType == RisAccountType.Uo
                                ? ItemChoiceType6.isUOAccount
                                : ItemChoiceType6.isRSOAccount,
                Item2 = acc.AccountNumber,
                Item2ElementName = Item2ChoiceType.AccountNumber,
                TotalSquare = acc.TotalSquare ?? 0m,
                ShareGUID = this.shareRequestList.ToArray()
            };

            if (acc.OwnerInd != null)
            {
                var indType = new IndType
                {
                    Surname = acc.OwnerInd.Surname,
                    FirstName = acc.OwnerInd.FirstName,
                    Sex = acc.OwnerInd.Sex == RisGender.F
                        ? IndTypeSex.F
                        : IndTypeSex.M,
                    DateOfBirth = acc.OwnerInd.DateOfBirth ?? DateTime.MinValue,
                    Item = acc.OwnerInd.Snils
                };
                if (acc.OwnerInd.Patronymic.IsNotEmpty())
                {
                    indType.Patronymic = acc.OwnerInd.Patronymic;
                }
                accRequest.Item1 = indType;
                accRequest.Item1ElementName = Item1ChoiceType1.OwnerInd;
            }
            else if (acc.OwnerOrg != null)
            {
                accRequest.Item1 = new RegOrgVersionType
                {
                    orgVersionGUID = acc.OwnerOrg.OrgVersionGuid
                };
                accRequest.Item1ElementName = Item1ChoiceType1.OwnerOrg;
            }
            else if (acc.RenterInd != null)
            {
                var indType = new IndType
                {
                    Surname = acc.RenterInd.Surname,
                    FirstName = acc.RenterInd.FirstName,
                    Sex = acc.RenterInd.Sex == RisGender.F
                        ? IndTypeSex.F
                        : IndTypeSex.M,
                    DateOfBirth = acc.RenterInd.DateOfBirth ?? DateTime.MinValue,
                    Item = acc.RenterInd.Snils
                };
                if (acc.RenterInd.Patronymic.IsNotEmpty())
                {
                    indType.Patronymic = acc.RenterInd.Patronymic;
                }
                accRequest.Item1 = indType;
                accRequest.Item1ElementName = Item1ChoiceType1.RenterInd;
            }
            else if (acc.RenterOrg != null)
            {
                accRequest.Item1 = new RegOrgVersionType
                {
                    orgVersionGUID = acc.RenterOrg.OrgVersionGuid
                };
                accRequest.Item1ElementName = Item1ChoiceType1.RenterOrg;
            }

            if (acc.LivingPersonsNumber.HasValue)
            {
                accRequest.LivingPersonsNumber = (sbyte)acc.LivingPersonsNumber.Value;
            }

            if (acc.ResidentialSquare.HasValue)
            {
                accRequest.ResidentialSquare = acc.ResidentialSquare.Value;
            }

            if (acc.HeatedArea.HasValue)
            {
                accRequest.HeatedArea = acc.HeatedArea.Value;
            }

            this.accountByTransportGuidDict.Add(accRequest.TransportGUID, acc);
            
            return accRequest;
        }
    }
}
//namespace Bars.Gkh.Ris.Integration.OrgRegistry
//{
//    using System;
//    using B4.Utils;
//    using Entities;
//    using Ris.OrgRegistry;

//    /// <summary>
//    /// Базовый метод импорта для сервиса OrgRegistry
//    /// </summary>
//    /// <typeparam name="T">Тип импортируемой сущности</typeparam>
//    /// <typeparam name="K">Тип запроса - класс, сгенерированный на основе wsdl</typeparam>
//    public abstract class GisIntegrationOrgRegistryMethod<T, K> : GisIntegrationMethodBase<T, K, RegOrgPortsTypeClient>
//        where T : BaseRisEntity
//    {
//        /// <summary>
//        /// Заголовок запроса
//        /// </summary>
//        protected ISRequestHeader RequestHeader
//        {
//            get
//            {
//                return new ISRequestHeader
//                {
//                    Date = DateTime.Now,
//                    MessageGUID = Guid.NewGuid().ToStr()
//                };
//            }
//        }

//        /// <summary>
//        /// Получить ответ от сервиса.
//        /// </summary>
//        /// <param name="request">Запрос</param>
//        /// <returns>Результат выполнения запроса</returns>
//        protected abstract ImportResult GetRequestResult(K request);

//        /// <summary>
//        /// Проверить строку response и добавить объекты ГИС на сохранение.
//        /// </summary>
//        /// <param name="responseItem">Элемент списка из response</param>
//        protected abstract void CheckResponseItem(CommonResultType responseItem);

//        /// <summary>
//        /// Выполнить запрос и обработать результат.
//        /// </summary>
//        /// <param name="request">Объект для запроса</param>
//        protected override void HandleRequestResult(K request)
//        {
//            ImportResult result;
//            try
//            {
//                result = this.GetRequestResult(request);
//            }
//            catch (Exception exception)
//            {
//                this.AddLineToLog(typeof(T).ToString(), 0, string.Empty, exception.Message);
//                return;
//            }

//            if (result != null)
//            {
//                foreach (var item in result.Items)
//                {
//                    var errorItem = item as CommonResultTypeError;
//                    var errorMessageTypeItem = item as ErrorMessageType;
//                    var responseItem = item as CommonResultType;

//                    if (errorItem != null)
//                    {
//                        this.AddLineToLog(string.Empty, 0, string.Empty, errorItem.Description);
//                    }
//                    else if (errorMessageTypeItem != null)
//                    {
//                        this.AddLineToLog(string.Empty, 0, string.Empty, errorMessageTypeItem.Description);
//                    }
//                    else if (responseItem != null)
//                    {
//                        this.CheckResponseItem(responseItem);
//                    }
//                }
//            }
//        }
//    }
//}

﻿namespace Bars.Gkh.Ris.Integration.HouseManagement
{
    using B4.Utils;
    using Entities;
    using Ris.HouseManagement;
    using System;

    public abstract class GisIntegrationHouseManagementMethod<T,K> : GisIntegrationMethodBase<T,K,HouseManagementPortsTypeClient> where T : BaseRisEntity
    {
        /// <summary>
        /// Заголовок запроса
        /// </summary>
        protected RequestHeader RequestHeader
        {
            get
            {
                return new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToStr(),
                    SenderID = this.SenderId
                };
            }
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>Результат выполнения запроса</returns>
        protected virtual ImportResult GetRequestResult(K request)
        {
            var result = new ImportResult();
            var result1 = this.GetRequestResult1(request);

            if (result1 != null)
            {
                result.Items = result1.Items;
            }

            return result;
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>Результат выполнения запроса</returns>
        protected virtual ImportResult1 GetRequestResult1(K request)
        {
            return null;
        }

        /// <summary>
        /// Проверить строку response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент списка из response</param>
        protected abstract void CheckResponseItem(CommonResultType responseItem);

        /// <summary>
        /// Выполнить запрос и обработать результат.
        /// </summary>
        /// <param name="request">Объект для запроса</param>
        protected override void HandleRequestResult(K request)
        {
            ImportResult result;
            try
            {
                result = this.GetRequestResult(request);
            }
            catch (Exception exception)
            {
                this.AddLineToLog(typeof(T).ToString(), 0, string.Empty, exception.Message);
                return;
            }

            if (result != null)
            {
                foreach (var item in result.Items)
                {
                    var errorItem = item as CommonResultTypeError;
                    var errorMessageTypeItem = item as ErrorMessageType;
                    var responseItem = item as CommonResultType;

                    if (errorItem != null)
                    {
                        this.AddLineToLog(string.Empty, 0, string.Empty, errorItem.Description);
                    }
                    else if (errorMessageTypeItem != null)
                    {
                        this.AddLineToLog(string.Empty, 0, string.Empty, errorMessageTypeItem.Description);
                    }
                    else if (responseItem != null)
                    {
                        this.CheckResponseItem(responseItem);
                    }
                }
            }
        }
    }
}
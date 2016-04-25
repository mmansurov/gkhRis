namespace Bars.Gkh.Ris.Controllers
{
    using B4;
    using DomainService.GisIntegration;
    using System;
    using System.Collections;
    using System.Web.Mvc;

    public class GisIntegrationController : BaseController
    {
        /// <summary>
        /// Получить список экстракторов
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        //public ActionResult DataExtractorList(BaseParams baseParams)
        //{
        //    var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

        //    try
        //    {
        //        var result = (ListDataResult)gisIntegrService.GetDataExtractorList(baseParams);
        //        return new JsonListResult((IEnumerable)result.Data, result.TotalCount);
        //    }
        //    finally
        //    {
        //        this.Container.Release(gisIntegrService);
        //    }
        //}

        /// <summary>
        /// Выполнение извлечения данных
        /// </summary>
        //public ActionResult ExtractData(BaseParams baseParams)
        //{
        //    var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

        //    try
        //    {
        //        var result = gisIntegrService.ExtractData(baseParams);
        //        return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
        //    }
        //    finally
        //    {
        //        this.Container.Release(gisIntegrService);
        //    }
        //}

        /// <summary>
        /// Получить список методов
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        public ActionResult MethodList(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = (ListDataResult)gisIntegrService.GetMethodList(baseParams);
                return new JsonListResult((IEnumerable)result.Data, result.TotalCount);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Метод проверки необходим ли для выполнения метода сертификат
        /// </summary>
        /// <param name="baseParams">Базовые параметры</param>
        /// <returns>Результат, содержащий true - сертификат нужен, false - в противном случае</returns>
        public ActionResult NeedCertificate(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = gisIntegrService.NeedCertificate(baseParams);
                return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Выполнение метода
        /// </summary>
        public ActionResult ExecuteMethod(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = gisIntegrService.ExecuteMethod(baseParams);
                return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        ///  Метод подготовки данных к экспорту, включая валидацию, формирование пакетов
        /// </summary>
        /// <param name="baseParams">Параметры экспорта</param>
        /// <returns>Результат подготовки данных</returns>
        public ActionResult PrepareDataForExport(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = gisIntegrService.PrepareDataForExport(baseParams);
                return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Получить неподписанные данные форматированной xml строкой
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификатор пакета</param>
        /// <returns>Результат, содержащий неподписанные данные форматированной xml строкой</returns>
        public ActionResult GetNotSignedData(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = gisIntegrService.GetNotSignedData(baseParams);
                return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Сохранить подписанные xml
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        public ActionResult SaveSignedData(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = gisIntegrService.SaveSignedData(baseParams);
                return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Запустить экспорт
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификаторы пакетов</param>
        /// <returns>Результат, содержащий идентификатор запланированной задачи получения результатов экспорта</returns>
        public ActionResult ExecuteExporter(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = gisIntegrService.ExecuteExporter(baseParams);
                return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Удалить временные объекты
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификаторы всех созданных объектов в рамках работы одного мастера</param>
        /// <returns>Результат выполнения операции</returns>
        public ActionResult DeleteTempObjects(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = gisIntegrService.DeleteTempObjects(baseParams);
                return result.Success ? new JsonGetResult(result.Data) : this.JsFailure(result.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Метод получения списка справочников ГИС
        /// </summary>
        public ActionResult GetDictList(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();
            try
            {
                var result = (ListDataResult)gisIntegrService.GetDictList(baseParams);
                return new JsonListResult((IEnumerable)result.Data, result.TotalCount);
            }
            catch (Exception exception)
            {
                return JsonNetResult.Failure(exception.Message);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Метод получения зарегистрированных методов для справочников
        /// </summary>
        public ActionResult GetDictActions(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();
            try
            {
                var result = gisIntegrService.GetDictActions(baseParams);
                var list = (IList)result.Data;
                return new JsonListResult(list);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Метод получения списка записей справочника броги
        /// </summary>
        public ActionResult GetDictRecordList(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();
            try
            {
                var result = gisIntegrService.GetDictRecordList(baseParams);
                var list = (IList)result.Data;
                return new JsonListResult(list, list.Count);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }

        /// <summary>
        /// Получение списка сертификатов для подписи сообщения
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        public ActionResult GetCertificates(BaseParams baseParams)
        {
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var result = (ListDataResult)gisIntegrService.GetCertificates(baseParams);
                return new JsonListResult((IEnumerable)result.Data, result.TotalCount);
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }
    }
}
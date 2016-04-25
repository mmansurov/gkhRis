namespace Bars.Gkh.Ris.Entities
{
    using B4.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Xml;

    using Bars.Gkh.Ris.Extensions;

    /// <summary>
    /// Пакет данных
    /// </summary>
    public class RisPackage : BaseEntity, IUserEntity
    {
        /// <summary>
        /// Не подписанные данные
        /// </summary>
        public virtual byte[] NotSignedData { get; set; }

        /// <summary>
        /// Подписанные данные
        /// </summary>
        public virtual byte[] SignedData { get; set; }

        /// <summary>
        /// Сериализованный объект, содержащий набор данных Тип объекта, Идентификатор объекта, Транспортный идентификатор
        /// </summary>
        public virtual byte[] TransportGuidDictionary { get; set; }

        /// <summary>
        /// Наименование пакета
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public virtual string UserName { get; set; }
        
        /// <summary>
        /// Контрагент
        /// </summary>
        public virtual RisContragent RisContragent { get; set; }

        /// <summary>
        /// Десериализовать неподписанные данные
        /// </summary>
        /// <returns>XML-документ неподписанных данных</returns>
        public virtual XmlDocument GetNotSignedXml()
        {
            var str = Encoding.UTF8.GetString(this.NotSignedData);
            var document = new XmlDocument();
            document.LoadXml(str);

            return document;
        }

        public virtual string GetNotSignedXmlString()
        {
            var str = Encoding.UTF8.GetString(this.NotSignedData);

            return str;
        }

        /// <summary>
        /// Получание неподписанных данных отформатированной XML-строки
        /// </summary>
        /// <returns>Форматированная XML-строка</returns>
        public virtual string GetNotSignedDataXmlString()
        {
            return XmlExtensions.GetFormattedXmlString(this.NotSignedData);
        }

        /// <summary>
        /// Сериализовать XML-документ неподписанных данных
        /// </summary>
        /// <param name="document">XML-документ неподписанных данных</param>
        public virtual void SetNotSignedXml(XmlDocument document)
        {
            this.NotSignedData = Encoding.UTF8.GetBytes(document.OuterXml);
        }

        /// <summary>
        /// Десериализовать подписанные данные
        /// </summary>
        /// <returns>XML-документ подписанных данных</returns>
        public virtual XmlDocument GetSignedXml()
        {
            var str = Encoding.UTF8.GetString(this.SignedData);
            var document = new XmlDocument();
            document.LoadXml(str);

            return document;
        }

        public virtual string GetSignedXmlString()
        {
            return Encoding.UTF8.GetString(this.SignedData);
        }

        /// <summary>
        /// Сериализовать XML-документ подписанных данных
        /// </summary>
        /// <param name="document">XML-документ подписанных данных</param>
        public virtual void SetSignedXml(XmlDocument document)
        {
            this.SignedData = Encoding.UTF8.GetBytes(document.OuterXml);
        }

        public virtual void SetSignedXml(string document)
        {
            this.SignedData = Encoding.UTF8.GetBytes(document);
        }

        /// <summary>
        /// Десериализовать словать сущностей, содержащихся в данных
        /// </summary>
        /// <returns>Словать сущностей, содержащихся в данных</returns>
        public virtual Dictionary<Type, Dictionary<string, long>> GetTransportGuidDictionary()
        {
            var result = new Dictionary<Type, Dictionary<string, long>>();
            BinaryFormatter formatter = new BinaryFormatter();

            if (this.TransportGuidDictionary != null)
            {
                using (var memoryStream = new MemoryStream(this.TransportGuidDictionary))
                {
                    result = (Dictionary<Type, Dictionary<string, long>>)formatter.Deserialize(memoryStream);
                }
            }

            return result;
        }

        /// <summary>
        /// Сериализовать словарь сущностей данных
        /// </summary>
        /// <param name="dictionary">Словарь сущностей данных</param>
        public virtual void SetTransportGuidDictionary(Dictionary<Type, Dictionary<string, long>> dictionary)
        {
            using (var memoryStream = new MemoryStream())
            {
                if (dictionary != null)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, dictionary);
                }

                this.TransportGuidDictionary = memoryStream.ToArray();
            }
        }
    }
}

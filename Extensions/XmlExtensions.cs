namespace Bars.Gkh.Ris.Extensions
{
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Статический класс, содержащий методы работы с xml
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Получание отформатированной XML-строки из массива байтов
        /// </summary>
        /// <param name="data">Массив байтов</param>
        /// <returns>Отформатированная XML-строка</returns>
        public static string GetFormattedXmlString(byte[] data)
        {
            string result;
            var mStream = new MemoryStream();
            var writer = new XmlTextWriter(mStream, Encoding.Unicode);
            var document = new XmlDocument();

            using (mStream)
            {
                using (writer)
                {
                    var str = Encoding.UTF8.GetString(data);
                    document.LoadXml(str);

                    writer.Formatting = Formatting.Indented;

                    document.WriteContentTo(writer);
                    writer.Flush();
                    mStream.Flush();

                    mStream.Position = 0;

                    var sReader = new StreamReader(mStream);

                    result = sReader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
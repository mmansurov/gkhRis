namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип документа по результатам проверки
    /// </summary>
    public enum DocumentTypeByResult
    {
        [Display("Акт проверки")]
        ActCheck = 20,

        [Display("Протокол")]
        Protocol = 60
    }
}
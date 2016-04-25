namespace Bars.Gkh.Ris.Entities
{
    using Bars.B4.DataAccess;

    /// <summary>
    /// Интерфейс сущности, содержащей поле Имя пользователя
    /// </summary>
    public interface IUserEntity : IEntity
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        string UserName { get; set; }
    }
}

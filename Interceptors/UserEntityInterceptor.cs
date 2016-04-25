namespace Bars.Gkh.Ris.Interceptors
{
    using Bars.B4;
    using Bars.Gkh.Ris.Entities;

    /// <summary>
    /// Interceptor для сущностей, содержащих имя пользователя
    /// </summary>
    /// <typeparam name="T">Сущность, содержащая имя пользователя</typeparam>
    public class UserEntityInterceptor<T> : EmptyDomainInterceptor<T> where T : class, IUserEntity
    {
        /// <summary>
        /// Проставление значения текущего пользователя перед созданием сущности
        /// </summary>
        /// <param name="service">Domain сервис</param>
        /// <param name="entity">Сущность</param>
        /// <returns>Результат</returns>
        public override IDataResult BeforeCreateAction(IDomainService<T> service, T entity)
        {
            if (string.IsNullOrEmpty(entity.UserName))
            {
                var user = this.Container.Resolve<IUserIdentity>();

                try
                {
                    entity.UserName = user != null ? user.Name : string.Empty;
                }
                finally 
                {
                    this.Container.Release(user);
                }
            }

            return this.Success();
        }
    }
}

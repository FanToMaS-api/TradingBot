namespace DatabaseLibrary.Repositories
{
    /// <summary>
    ///     Общее хранилище
    /// </summary>
    public interface IUnitOfWork
    {
        /// <inheritdoc cref="IHotUnitOfWork"/>
        IHotUnitOfWork HotUnitOfWork { get; }

        /// <inheritdoc cref="IHotUnitOfWork"/>
        IColdUnitOfWork ColdUnitOfWork { get; }
    }
}

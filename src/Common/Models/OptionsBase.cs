namespace Common.Models
{
    /// <summary>
    ///     Базовый класс настроек
    /// </summary>
    public abstract class OptionsBase
    {
        /// <summary>
        ///     Название блока настроек
        /// </summary>
        public abstract string Name { get; }
    }
}

namespace Common.Models
{
    /// <summary>
    ///     Базовый класс настроек бирж
    /// </summary>
    public abstract class OptionsBase
    {
        /// <summary>
        ///     Название блока настроек
        /// </summary>
        public abstract string Name { get; }
    }
}

using System;

namespace Common.Helpers
{
    /// <summary>
    ///     Проверяет утверждения
    /// </summary>
    public static class Assert
    {
        /// <summary>
        ///     Проверяет объект на null
        /// </summary>
        public static void NotNull(object @object, string message = "")
        {
            if (@object is null)
            {
                throw new ArgumentNullException($"Error! {nameof(@object)}. {message}");
            }
        }

        /// <summary>
        ///     Проверяет утверждение на истинность
        /// </summary>
        public static void True(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new ArgumentException($"Error! Condition is false. {message}");
            }
        }

        /// <summary>
        ///     Проверяет утверждение на ложь
        /// </summary>
        public static void False(bool condition, string message = "")
        {
            if (condition)
            {
                throw new ArgumentException($"Error! Condition is true. {message}");
            }
        }
    }
}

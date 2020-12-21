using System;

namespace FactoryApi
{
    /// <summary>
    /// Конфигурация приложения
    /// </summary>
    public class Configuration
    {
        private static Configuration? _instance;

        public static Configuration GetInstance() => _instance ??
                                                     throw new Exception(
                                                         "Необходимо инициализировать конфигурацию, вызвав метод Init");

        /// <summary>
        /// Инициализирует конфигурацию приложения
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных</param>
        /// <param name="enableWriting">Включить этап НАНЕСЕНИЕ</param>
        public static void Init(string connectionString, bool enableWriting) =>
            _instance = new Configuration(connectionString, enableWriting);
        
        /// <summary>
        /// Создает конфигуркцию приложения
        /// </summary>
        /// <param name="connectionString">Строка подключения к базе данных</param>
        /// <param name="enableWriting">Включить этап НАНЕСЕНИЕ</param>
        private Configuration(string connectionString, bool enableWriting)
        {
            ConnectionString = connectionString;
            EnableWriting = enableWriting;
        }

        /// <summary>
        /// Строка подключения к базе данных
        /// </summary>
        public string ConnectionString { get; }

        /// <summary>
        /// Включить этап НАНЕСЕНИЕ
        /// </summary>
        public bool EnableWriting { get; set; }
    }
}
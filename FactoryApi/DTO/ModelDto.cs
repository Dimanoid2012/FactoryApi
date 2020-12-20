using System;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Модель
    /// </summary>
    public class ModelDto
    {
        /// <summary>
        /// Идентификатор модели
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Наименование модели
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Идентификатор цвет модели
        /// </summary>
        public Guid ColorId { get; set; }
    }
}
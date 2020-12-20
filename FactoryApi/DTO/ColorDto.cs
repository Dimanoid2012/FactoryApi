using System;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Цвет
    /// </summary>
    public class ColorDto
    {
        public Guid? Id { get; set; }
        
        /// <summary>
        /// Наименование цвета
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Значение цвета
        /// </summary>
        public string? Value { get; set; }
    }
}
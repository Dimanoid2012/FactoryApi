using System;
using System.ComponentModel.DataAnnotations;

namespace FactoryApi.SwaggerDoc
{
    public class Color
    {
        /// <summary>
        /// Идентификатор цвета
        /// </summary>
        /// <example>fd058e3f-a5e0-47ef-bf15-3d83edc87a61</example>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование цвета
        /// </summary>
        /// <example>Белый</example>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Значение цвета
        /// </summary>
        /// <example>#FFFFFF</example>
        [Required]
        public string Value { get; set; } = "";
    }
}
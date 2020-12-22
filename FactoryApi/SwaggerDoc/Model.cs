using System;
using System.ComponentModel.DataAnnotations;

namespace FactoryApi.SwaggerDoc
{
    public class Model
    {
        /// <summary>
        /// Идентификатор модели
        /// </summary>
        /// <example>fd058e3f-a5e0-47ef-bf15-3d83edc87a61</example>
        [Required]
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование модели
        /// </summary>
        /// <example>Белая рубашка</example>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Цвет модели
        /// </summary>
        [Required]
        public Color Color { get; set; } = new();
    }
}
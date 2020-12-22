using System;
using System.ComponentModel.DataAnnotations;

namespace FactoryApi.SwaggerDoc
{
    public class Size
    {
        /// <summary>
        /// Идентификатор размера
        /// </summary>
        /// <example>fd058e3f-a5e0-47ef-bf15-3d83edc87a61</example>
        [Required]
        public Guid Id { get; set; }
        
        /// <summary>
        /// Наименование размера
        /// </summary>
        /// <example>Большой</example>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Обозначение размера
        /// </summary>
        /// <example>XL</example>
        [Required]
        public string Value { get; set; } = "";
    }
}
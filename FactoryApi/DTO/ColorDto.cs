﻿using System.ComponentModel.DataAnnotations;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Цвет
    /// </summary>
    public class ColorDto
    {
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
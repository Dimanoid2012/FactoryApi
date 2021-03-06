﻿using System.ComponentModel.DataAnnotations;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Размер
    /// </summary>
    public class SizeDto
    {
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
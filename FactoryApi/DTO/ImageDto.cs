﻿using System.ComponentModel.DataAnnotations;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Картинка
    /// </summary>
    public class ImageDto
    {
        /// <summary>
        /// Наименование картинки
        /// </summary>
        /// <example>Груша</example>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Ширина картинки
        /// </summary>
        /// <example>640</example>
        [Required]
        public decimal Width { get; set; }

        /// <summary>
        /// Высота картинки
        /// </summary>
        /// <example>480</example>
        [Required]
        public decimal Height { get; set; }

        /// <summary>
        /// Тип картинки
        /// </summary>
        /// <example>jpg</example>
        [Required]
        public string Type { get; set; } = "";

        /// <summary>
        /// Содержимое картинки, закодированное в base64
        /// </summary>
        /// <example>MjM0d2VvZmljajRydnVgPWMyM2lyeC1pYC0zNT1j</example>
        [Required]
        public string ContentsBase64 { get; set; } = "";
    }
}
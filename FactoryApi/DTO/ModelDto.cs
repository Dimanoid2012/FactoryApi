﻿using System;
using System.ComponentModel.DataAnnotations;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Модель
    /// </summary>
    public class ModelDto
    {
        /// <summary>
        /// Наименование модели
        /// </summary>
        /// <example>Белая рубашка</example>
        [Required]
        public string Name { get; set; } = "";

        /// <summary>
        /// Идентификатор цвет модели
        /// </summary>
        /// <example>fd058e3f-a5e0-47ef-bf15-3d83edc87a61</example>
        [Required]
        public Guid ColorId { get; set; }
    }
}
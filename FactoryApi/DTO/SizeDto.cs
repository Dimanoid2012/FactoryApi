using System;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Размер
    /// </summary>
    public class SizeDto
    {
        public Guid? Id { get; set; }
        
        /// <summary>
        /// Наименование размера
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Обозначение размера: XS, M, XL, и т.д.
        /// </summary>
        public string? Value { get; set; }
    }
}
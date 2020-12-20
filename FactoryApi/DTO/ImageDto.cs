using System;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Картинка
    /// </summary>
    public class ImageDto
    {
        public Guid? Id { get; set; }
        
        /// <summary>
        /// Наименование размера
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Ширина картинки
        /// </summary>
        public decimal? Width { get; set; }

        /// <summary>
        /// Высота картинки
        /// </summary>
        public decimal? Height { get; set; }
        
        /// <summary>
        /// Тип картинки
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// Содержимое картинки, закодированное в base64
        /// </summary>
        public string? ContentsBase64 { get; set; }
    }
}
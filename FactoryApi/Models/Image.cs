using System;
using System.Collections.Generic;

namespace FactoryApi.Models
{
    public class Image
    {
        /// <summary>
        /// Создает новую картинку
        /// </summary>
        /// <param name="name">Наименование картинки</param>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        /// <param name="type">Тип картинки</param>
        /// <param name="contents">Содержимое картинки</param>
        /// <param name="id">Идентификатор картинки</param>
        public Image(string name, decimal width, decimal height, string type, byte[] contents, Guid? id = null) : this(
            id ?? Guid.NewGuid())
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), width,
                    "Ширина картинки должна быть положительной");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), height,
                    "Ширина картинки должна быть положительной");
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentOutOfRangeException(nameof(type), type, 
                    "Тип картинки должен быть указан");
            if (contents.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(contents), 
                    "Содержимое картинки должно быть указано");

            Name = string.IsNullOrWhiteSpace(name) ? Id.ToString() : name;
            Width = width;
            Height = height;
            Type = type;
            Contents = contents;
        }

        /// <summary>
        /// Создает новую картинку без имени. Имя будет равно идентификатору
        /// </summary>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        /// <param name="type">Тип картинки</param>
        /// <param name="contents">Содержимое картинки</param>
        /// <param name="id">Идентификатор картинки</param>
        public Image(decimal width, decimal height, string type, byte[] contents, Guid? id = null) : this("", width,
            height, type, contents, id)
        {
            Name = Id.ToString();
        }

        /// <summary>
        /// Используется для создания ссылки на существующую в базе картинку
        /// </summary>
        /// <param name="id">Идентификатор картинки</param>
        public Image(Guid id)
        {
            Id = id;
            Name = "";
            Type = "";
            Contents = Array.Empty<byte>();
            Orders = new List<Order>();
        }


        /// <summary>
        /// Идентификатор картинки
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Наименование картинки
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Ширина картинки
        /// </summary>
        public decimal Width { get; private set; }

        /// <summary>
        /// Высота картинки
        /// </summary>
        public decimal Height { get; private set; }
        
        /// <summary>
        /// Тип картинки: jpg/png/gif и т.д.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Содержимое картинки
        /// </summary>
        public byte[] Contents { get; private set; }
        
        /// <summary>
        /// Список заказов с данной картинкой
        /// </summary>
        public IReadOnlyCollection<Order> Orders { get; private set; }
    }
}
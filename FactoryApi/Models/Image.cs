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
        /// <param name="contents">Содержимое картинки</param>
        /// <param name="id">Идентификатор картинки</param>
        public Image(string name, decimal width, decimal height, byte[] contents, Guid? id = null) : this(
            id ?? Guid.NewGuid())
        {
            Name = name;
            Width = width;
            Height = height;
            Contents = contents;
        }

        /// <summary>
        /// Создает новую картинку без имени. Имя будет равно идентификатору
        /// </summary>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        /// <param name="contents">Содержимое картинки</param>
        /// <param name="id">Идентификатор картинки</param>
        public Image(decimal width, decimal height, byte[] contents, Guid? id = null) : this("", width, height,
            contents, id)
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
            Contents = Array.Empty<byte>();
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
        /// Содержимое картинки
        /// </summary>
        public byte[] Contents { get; private set; }
        
        /// <summary>
        /// Список заказов с данной картинкой
        /// </summary>
        public IReadOnlyCollection<Order> Orders { get; private set; }
    }
}
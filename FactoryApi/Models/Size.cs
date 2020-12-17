using System;

namespace FactoryApi.Models
{
    /// <summary>
    /// Размер
    /// </summary>
    public class Size
    {
        /// <summary>
        /// Создает новый размер
        /// </summary>
        /// <param name="id">Идентификатор размера</param>
        /// <param name="name">Наименование размера</param>
        /// <param name="value">Обозначение размера: XS, M, XL, и т.д.</param>
        public Size(string name, string value, Guid? id = null) : this(id ?? Guid.NewGuid())
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Используется для создания ссылки на существующий в базе размер
        /// </summary>
        /// <param name="id">Идентификатор размера</param>
        public Size(Guid id)
        {
            Id = id;
            Name = "";
            Value = "";
        }

        /// <summary>
        /// Идентификатор размера
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование размера
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Обозначение размера: XS, M, XL, и т.д.
        /// </summary>
        public string Value { get; set; }
    }
}
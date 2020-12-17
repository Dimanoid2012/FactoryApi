using System;
using System.Collections.Generic;

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
        public Guid Id { get; private set; }

        /// <summary>
        /// Наименование размера
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Обозначение размера: XS, M, XL, и т.д.
        /// </summary>
        public string Value { get; private set; }
        
        /// <summary>
        /// Список заказов данного размера
        /// </summary>
        public IReadOnlyCollection<Order> Orders { get; private set; }
    }
}
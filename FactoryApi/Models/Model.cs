using System;
using System.Collections.Generic;

namespace FactoryApi.Models
{
    /// <summary>
    /// Модель
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Создает новую модель
        /// </summary>
        /// <param name="name">Наименование модели</param>
        /// <param name="color">Цвет модели</param>
        /// <param name="id">Идентификатор модели</param>
        public Model(string name, Color color, Guid? id = null) : this(id ?? Guid.NewGuid())
        {
            Name = name;
            Color = color;
        }

        /// <summary>
        /// Используется для создания ссылки на существующую в базе модель
        /// </summary>
        /// <param name="id">Идентификатор модели</param>
        public Model(Guid id)
        {
            Id = id;
            Name = "";
            Color = new Color(Guid.Empty);
            Orders = Array.Empty<Order>();
        }

        /// <summary>
        /// Идентификатор модели
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Наименование модели
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Цвет модели
        /// </summary>
        public Color Color { get; private set; }
        
        /// <summary>
        /// Список заказов данной модели
        /// </summary>
        public IReadOnlyCollection<Order> Orders { get; private set; }
    }
}
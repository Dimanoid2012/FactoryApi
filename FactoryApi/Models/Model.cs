using System;

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
        }

        /// <summary>
        /// Идентификатор модели
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование модели
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Цвет модели
        /// </summary>
        public Color Color { get; set; }
    }
}
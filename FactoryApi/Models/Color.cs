using System;
using System.Collections.Generic;

namespace FactoryApi.Models
{
    /// <summary>
    /// Цвет
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Создает новый цвет
        /// </summary>
        /// <param name="name">Наименование цвета</param>
        /// <param name="rgb">Составляющие цвета</param>
        public Color(string name, RGB rgb) : this(Guid.NewGuid())
        {
            Name = name;
            RGB = rgb;
        }

        /// <summary>
        /// Используется для создания ссылки на существующий в базе цвет
        /// </summary>
        /// <param name="id">Идентификатор цвета</param>
        public Color(Guid id)
        {
            Id = id;
            Name = "";
            RGB = new RGB(0, 0, 0);
            Models = new List<Model>();
        }

        /// <summary>
        /// Идентификатор цвета
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Наименование цвета
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Составляющие цвета
        /// </summary>
        public RGB RGB { get; private set; }

        /// <summary>
        /// Список моделей с данным цветом
        /// </summary>
        public IReadOnlyCollection<Model> Models { get; private set; }
    }
}
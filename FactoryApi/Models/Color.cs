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
        /// <param name="id">Идентификатор цвета</param>
        /// <param name="name">Наименование цвета</param>
        /// <param name="r">Красная составляющая</param>
        /// <param name="g">Зеленая составляющая</param>
        /// <param name="b">Синяя составляющая</param>
        public Color(string name, byte r, byte g, byte b, Guid? id = null) : this(id ?? Guid.NewGuid())
        {
            Name = name;
            R = r;
            G = g;
            B = b;
        }

        /// <summary>
        /// Используется для создания ссылки на существующий в базе цвет
        /// </summary>
        /// <param name="id">Идентификатор цвета</param>
        public Color(Guid id)
        {
            Id = id;
            Name = "";
            Models = Array.Empty<Model>();
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
        /// Красная составляющая
        /// </summary>
        public byte R { get; private set; }

        /// <summary>
        /// Зеленая составляющая
        /// </summary>
        public byte G { get; private set; }

        /// <summary>
        /// Синяя составляющая
        /// </summary>
        public byte B { get; private set; }

        /// <summary>
        /// Список моделей с данным цветом
        /// </summary>
        public IReadOnlyCollection<Model> Models { get; private set; }
    }
}
using System;

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
        }

        /// <summary>
        /// Идентификатор цвета
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Наименование цвета
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Красная составляющая
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Зеленая составляющая
        /// </summary>
        public byte G { get; set; }

        /// <summary>
        /// Синяя составляющая
        /// </summary>
        public byte B { get; set; }
    }
}
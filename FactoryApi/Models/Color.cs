using System;
using System.Collections.Generic;
using System.Globalization;

namespace FactoryApi.Models
{
    /// <summary>
    /// Цвет
    /// </summary>
    public class Color
    {
        public static Color Parse(string name, string value)
        {
            if (value.Length != 7)
                throw new ArgumentOutOfRangeException(nameof(value), value,
                    "Неправильный формат цвета. Формат должен быть #FFFFFF");

            var rString = value[1..3];
            var gString = value[3..5];
            var bString = value[5..];
            
            var r = byte.Parse(rString, NumberStyles.HexNumber);
            var g = byte.Parse(gString, NumberStyles.HexNumber);
            var b = byte.Parse(bString, NumberStyles.HexNumber);

            return new Color(name, r, g, b);
        }
        
        /// <summary>
        /// Создает новый цвет
        /// </summary>
        /// <param name="id">Идентификатор цвета</param>
        /// <param name="name">Наименование цвета</param>
        /// <param name="r">Красная составляющая</param>
        /// <param name="g">Зеленая составляющая</param>
        /// <param name="b">Синяя составляющая</param>
        public Color(string name, byte r, byte g, byte b) : this(Guid.NewGuid())
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

        /// <summary>
        /// Возвращает HTML-представление цвета, например, #FFFFFF.
        /// </summary>
        /// <returns>Возвращает HTML-представление цвета, например, #FFFFFF.</returns>
        public override string ToString() => $"#{R:X}{G:X}{B:X}";
    }
}
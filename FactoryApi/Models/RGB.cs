using System;
using System.Globalization;

namespace FactoryApi.Models
{
    /// <summary>
    /// RGB-составляющая цвета
    /// </summary>
    public record RGB(byte R, byte G, byte B)
    {
        /// <summary>
        /// Разбирает строку в формате #RRGGBB на составляющие цвета.
        /// </summary>
        /// <param name="value">Строка в формате #RRGGBB</param>
        /// <returns>Возвращает объект RGB</returns>
        public static RGB Parse(string value)
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

            return new RGB(r, g, b);
        }
        
        /// <summary>
        /// Возвращает HTML-представление цвета, например, #FFFFFF.
        /// </summary>
        /// <returns>Возвращает HTML-представление цвета, например, #FFFFFF.</returns>
        public override string ToString() => $"#{R:X2}{G:X2}{B:X2}";
    }
}
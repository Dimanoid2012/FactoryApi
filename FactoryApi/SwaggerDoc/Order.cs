using System;
using FactoryApi.Models;

namespace FactoryApi.SwaggerDoc
{
    /// <summary>
    /// Отмененный заказ
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        /// <example>fd058e3f-a5e0-47ef-bf15-3d83edc87a61</example>
        public Guid Id { get; set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        /// <example>0</example>
        public OrderState State { get; set; }

        /// <summary>
        /// Номер заказа
        /// </summary>
        /// <example>12</example>
        public int Number { get; set; }

        /// <summary>
        /// Расположение принта: 0 - спереди, 1 - сзади
        /// </summary>
        /// <example>0</example>
        public ImageSide Side { get; set; }

        /// <summary>
        /// Картинка
        /// </summary>
        public Image Image { get; set; } = new();

        /// <summary>
        /// Модель
        /// </summary>
        public Model Model { get; set; } = new();

        /// <summary>
        /// Размер
        /// </summary>
        public Size Size { get; set; } = new();

        /// <summary>
        /// Имя клиента
        /// </summary>
        /// <example>Вася</example>
        public string? ClientName { get; set; }

        /// <summary>
        /// Номер телефона клиента
        /// </summary>
        /// <example>+79991234567</example>
        public string? ClientPhone { get; set; }

        /// <summary>
        /// Смещение картинки сверху
        /// </summary>
        /// <example>5</example>
        public decimal? Top { get; set; }

        /// <summary>
        /// Смещение картинки слева
        /// </summary>
        /// <example>10</example>
        public decimal? Left { get; set; }
    }
}
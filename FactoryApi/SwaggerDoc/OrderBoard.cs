using System;
using FactoryApi.Models;

namespace FactoryApi.SwaggerDoc
{
    /// <summary>
    /// Заказ для табло
    /// </summary>
    public class OrderBoard
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
    }
}
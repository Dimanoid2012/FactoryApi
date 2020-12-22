using System;
using FactoryApi.Models;

namespace FactoryApi.DTO
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// Номер заказа (генерируется базой данных)
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Идентификатор модели
        /// </summary>
        public Guid ModelId { get; set; }

        /// <summary>
        /// Идентификатор размера
        /// </summary>
        public Guid SizeId { get; set; }

        /// <summary>
        /// Расположение принта
        /// </summary>
        public ImageSide Side { get; set; }

        /// <summary>
        /// Принт
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Смещение принта сверху
        /// </summary>
        public decimal Top { get; set; }

        /// <summary>
        /// Смещение принта слева
        /// </summary>
        public decimal Left { get; set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName { get; set; } = "";

        /// <summary>
        /// Номер телефона клиента
        /// </summary>
        public string ClientPhone { get; set; } = "";
    }
}
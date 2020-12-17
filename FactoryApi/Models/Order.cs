using System;

namespace FactoryApi.Models
{
    /// <summary>
    /// Заказ
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Создает новый заказ
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="size">Размер</param>
        /// <param name="side">Расположение принта: спереди или сзади</param>
        /// <param name="image">Принт</param>
        /// <param name="top">Смещение принта сверху</param>
        /// <param name="left">Смещение принта слева</param>
        /// <param name="clientName">Имя клиента</param>
        /// <param name="clientPhone">Номер телефона клиента</param>
        /// <param name="id">Идентификатор заказа</param>
        public Order(Model model, Size size, ImageSide side, Image image, decimal top, decimal left, string clientName,
            string clientPhone, Guid? id = null) :
            this(id ?? Guid.NewGuid())
        {
            if (top < 0)
                throw new ArgumentOutOfRangeException(nameof(top), top,
                    "Смещение картинки не может быть отрицательным");
            if (left < 0)
                throw new ArgumentOutOfRangeException(nameof(left), left,
                    "Смещение картинки не может быть отрицательным");
            
            State = OrderState.New;
            Model = model;
            Size = size;
            Side = side;
            Image = image;
            Top = top;
            Left = left;
            ClientName = clientName;
            ClientPhone = clientPhone;
        }

        /// <summary>
        /// Используется для создания ссылки на существующий в базе заказ
        /// </summary>
        /// <param name="id">Идентификатор заказа</param>
        public Order(Guid id)
        {
            Id = id;
            State = OrderState.New;
            Model = new Model(Guid.Empty);
            Size = new Size(Guid.Empty);
            Image = new Image(Guid.Empty);
            ClientName = "";
            ClientPhone = "";
        }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// Номер заказа (генерируется базой данных)
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Модель
        /// </summary>
        public Model Model { get; }

        /// <summary>
        /// Размер
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// Расположение принта: спереди или сзади
        /// </summary>
        public ImageSide Side { get; }

        /// <summary>
        /// Принт
        /// </summary>
        public Image Image { get; }

        /// <summary>
        /// Смещение принта сверху
        /// </summary>
        public decimal Top { get; }

        /// <summary>
        /// Смещение принта слева
        /// </summary>
        public decimal Left { get; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Номер телефона клиента
        /// </summary>
        public string ClientPhone { get; set; }
    }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// Новый
        /// </summary>
        New = 0,

        /// <summary>
        /// Подтвержден
        /// </summary>
        Confirmed = 1,

        /// <summary>
        /// На нанесении
        /// </summary>
        Writing = 2,

        /// <summary>
        /// На печати
        /// </summary>
        Printing = 3,

        /// <summary>
        /// Выдача
        /// </summary>
        Issue = 4,

        /// <summary>
        /// Завершено
        /// </summary>
        Done = 100
    }

    /// <summary>
    /// Расположение принта
    /// </summary>
    public enum ImageSide
    {
        /// <summary>
        /// Спереди
        /// </summary>
        Front = 0,

        /// <summary>
        /// Сзади
        /// </summary>
        Back = 1
    }
}
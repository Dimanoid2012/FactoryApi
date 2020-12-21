using System;
using Microsoft.AspNetCore.Identity;

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
        /// <exception cref="ArgumentOutOfRangeException">Неправильно задано расположение принта или его смещение</exception>
        public Order(Model model, Size size, ImageSide side, Image image, decimal top, decimal left, string clientName,
            string clientPhone, Guid? id = null) :
            this(id ?? Guid.NewGuid())
        {
            if (!Enum.IsDefined(typeof(ImageSide), side))
                throw new ArgumentOutOfRangeException(nameof(side), side,
                    "Расположение принта указано неправильно");
            if (top < 0)
                throw new ArgumentOutOfRangeException(nameof(top), top,
                    "Смещение картинки не может быть отрицательным");
            if (left < 0)
                throw new ArgumentOutOfRangeException(nameof(left), left,
                    "Смещение картинки не может быть отрицательным");

            State = OrderState.Confirming;
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
            State = OrderState.Confirming;
            Model = new Model(Guid.Empty);
            Size = new Size(Guid.Empty);
            Image = new Image(Guid.Empty);
            ClientName = "";
            ClientPhone = "";
        }

        /// <summary>
        /// Идентификатор заказа
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Статус заказа
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// Номер заказа (генерируется базой данных)
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Модель
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Размер
        /// </summary>
        public Size Size { get; private set; }

        /// <summary>
        /// Расположение принта: спереди или сзади
        /// </summary>
        public ImageSide Side { get; private set; }

        /// <summary>
        /// Принт
        /// </summary>
        public Image Image { get; private set; }

        /// <summary>
        /// Смещение принта сверху
        /// </summary>
        public decimal Top { get; private set; }

        /// <summary>
        /// Смещение принта слева
        /// </summary>
        public decimal Left { get; private set; }

        /// <summary>
        /// Имя клиента
        /// </summary>
        public string ClientName { get; private set; }

        /// <summary>
        /// Номер телефона клиента
        /// </summary>
        public string ClientPhone { get; private set; }
        
        /// <summary>
        /// Исполнитель стадии НА НАНЕСЕНИИ
        /// </summary>
        public string? WriterName { get; private set; }
        
        /// <summary>
        /// Исполнитель стадии НА ПЕЧАТИ
        /// </summary>
        public string? PrinterName { get; private set; }

        /// <summary>
        /// Отменяет новый заказ
        /// </summary>
        /// <returns>Возвращает true, если заказ был в статусе ПОДТВЕРЖДЕНИЕ, иначе возвращает false</returns>
        public bool Cancel()
        {
            if (State != OrderState.Confirming)
                return false;

            State = OrderState.Canceled;
            return true;

        }

        /// <summary>
        /// Подтверждает новый заказ, переводя его в статус НАНЕСЕНИЕ или ПЕЧАТЬ в зависимости от настроек сессии
        /// </summary>
        /// <returns>Возвращает true, если заказ был в статусе ПОДТВЕРЖДЕНИЕ, иначе возвращает false</returns>
        public bool Confirm()
        {
            if (State != OrderState.Confirming)
                return false;

            State = Configuration.GetInstance().EnableWriting ? OrderState.Writing : OrderState.Printing;
            return true;
        }

        /// <summary>
        /// Задает исполнителя на стадии НА НАНЕСЕНИИ
        /// </summary>
        /// <param name="writerName">Исполнитель стадии НА НАНЕСЕНИИ</param>
        /// <returns>Возвращает true, если заказ в статусе НА НАНЕСЕНИИ и исполнитель не задан, иначе возвращает false</returns>
        public bool SetWriter(string writerName)
        {
            if (State != OrderState.Writing || WriterName != null)
                return false;

            if (string.IsNullOrWhiteSpace(writerName))
                return false;
            
            WriterName = writerName;
            return true;
        }
        
        /// <summary>
        /// Задает исполнителя на стадии НА ПЕЧАТИ
        /// </summary>
        /// <param name="printerName">Исполнитель стадии НА ПЕЧАТИ</param>
        /// <returns>Возвращает true, если заказ в статусе НА ПЕЧАТИ и исполнитель не задан, иначе возвращает false</returns>
        public bool SetPrinter(string printerName)
        {
            if (State != OrderState.Printing || PrinterName != null)
                return false;
            
            if (string.IsNullOrWhiteSpace(printerName))
                return false;

            PrinterName = printerName;
            return true;
        }
        
        /// <summary>
        /// Переводит заказ в статус ВЫДАЧА
        /// </summary>
        /// <returns>Возвращает true, если заказ был в статусе НА НАНЕСЕНИИ или НА ПЕЧАТИ и был указан исполнитель, иначе возвращает false</returns>
        public bool Issue()
        {
            if ((State != OrderState.Writing || WriterName == null) &&
                (State != OrderState.Printing || PrinterName == null)) 
                return false;
            
            State = OrderState.Issue;
            return true;
        }
    }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// Подтверждение
        /// </summary>
        Confirming = 0,

        /// <summary>
        /// На нанесении
        /// </summary>
        Writing = 1,

        /// <summary>
        /// На печати
        /// </summary>
        Printing = 2,

        /// <summary>
        /// Выдача
        /// </summary>
        Issue = 3,

        /// <summary>
        /// Завершено
        /// </summary>
        Done = 100,
        
        /// <summary>
        /// Отменен
        /// </summary>
        Canceled = 200
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
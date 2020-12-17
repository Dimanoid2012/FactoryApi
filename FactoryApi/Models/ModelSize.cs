using System;

namespace FactoryApi.Models
{
    /// <summary>
    /// Доступный размер для модели
    /// </summary>
    public class ModelSize
    {
        /// <summary>
        /// Создает доступный размер для модели
        /// </summary>
        /// <param name="model">Модель</param>
        /// <param name="size">Размер</param>
        public ModelSize(Model model, Size size)
        {
            Model = model;
            Size = size;
        }

        /// <summary>
        /// Для EntityFramework Core
        /// </summary>
        private ModelSize()
        {
            Model = new Model(Guid.Empty);
            Size = new Size(Guid.Empty);
        }

        /// <summary>
        /// Модель
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Размер
        /// </summary>
        public Size Size { get; private set; }
    }
}
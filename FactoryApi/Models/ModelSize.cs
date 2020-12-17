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
        /// Модель
        /// </summary>
        public Model Model { get; set; }

        /// <summary>
        /// Размер
        /// </summary>
        public Size Size { get; set; }
    }
}
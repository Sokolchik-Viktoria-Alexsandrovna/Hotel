using System.ComponentModel.DataAnnotations;

namespace Hotel.Models
{
    public enum RoomType
    {
        Standard,
        Deluxe,
        Suite,
        Family
    }

    public class Room
    {
        public int Id { get; set; }

        [Required, Display(Name = "Номер комнаты")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required, Display(Name = "Тип")]
        public RoomType Type { get; set; }

        [Required, Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required, Display(Name = "Цена за ночь")]
        [DataType(DataType.Currency)]
        public decimal PricePerNight { get; set; }

        [Display(Name = "Вместимость")]
        public int Capacity { get; set; }

        [Display(Name = "Фото")]
        public string ImageUrl { get; set; } = string.Empty;

        [Display(Name = "Доступен")]
        public bool IsAvailable { get; set; } = true;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}

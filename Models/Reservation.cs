using System.ComponentModel.DataAnnotations;

namespace Hotel.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        [Required, Display(Name = "Дата заезда")]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required, Display(Name = "Дата выезда")]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        [Display(Name = "Дата бронирования")]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Display(Name = "Итоговая сумма")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        public int Nights => (CheckOut - CheckIn).Days > 0 ? (CheckOut - CheckIn).Days : 1;
    }
}

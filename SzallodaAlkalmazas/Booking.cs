
using System;

namespace SzallodaAlkalmazas
{
    public class Booking
    {
        public int Id { get; private set; }
        public int GuestId { get; private set; }
        public int RoomNumber { get; private set; }
        public DateTime CheckIn { get; private set; }
        public DateTime CheckOut { get; private set; }
        public int GuestCount { get; private set; }
        public string Status { get; private set; }
        public decimal TotalPrice { get; private set; }

        public Booking(int id, int guestId, int roomNumber,
            DateTime checkIn, DateTime checkOut, int guestCount,
            string status, decimal pricePerNight)
        {
            if (id <= 0 || guestId <= 0 || roomNumber <= 0)
                throw new ArgumentException("Hibás azonosító.");

            if (checkOut.Date <= checkIn.Date)
                throw new ArgumentException(
                    "A távozásnak az érkezés után kell lennie.");

            if (guestCount <= 0)
                throw new ArgumentException("Hibás vendégszám.");

            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("A foglalás állapota kötelező.");

            if (pricePerNight <= 0)
                throw new ArgumentException("Hibás szobaár.");

            Id = id;
            GuestId = guestId;
            RoomNumber = roomNumber;
            CheckIn = checkIn.Date;
            CheckOut = checkOut.Date;
            GuestCount = guestCount;
            Status = status;

            TotalPrice = CalculatePrice(
                CheckIn, CheckOut, pricePerNight);
        }

        public static decimal CalculatePrice(
            DateTime checkIn, DateTime checkOut, decimal pricePerNight)
        {
            if (checkOut.Date <= checkIn.Date || pricePerNight <= 0)
                throw new ArgumentException("Hibás dátum vagy ár.");

            int nights = (checkOut.Date - checkIn.Date).Days;
            return nights * pricePerNight;
        }

        public override string ToString()
        {
            return $"Foglalás #{Id} | Vendég: {GuestId} | " +
                $"Szoba: {RoomNumber} | {CheckIn:yyyy.MM.dd.} - " +
                $"{CheckOut:yyyy.MM.dd.} | {GuestCount} fő | " +
                $"{TotalPrice:N0} Ft | {Status}";
        }
    }
}


using System;

namespace SzallodaAlkalmazas
{
    public class Room
    {
        public int RoomNumber { get; private set; }
        public string RoomType { get; private set; }
        public int Capacity { get; private set; }
        public decimal PricePerNight { get; private set; }
        public string Equipment { get; private set; }
        public bool IsActive { get; private set; }

        public Room(int roomNumber, string roomType, int capacity,
            decimal pricePerNight, string equipment, bool isActive = true)
        {
            if (roomNumber <= 0)
                throw new ArgumentException("Hibás szobaszám.");

            if (string.IsNullOrWhiteSpace(roomType))
                throw new ArgumentException("Add meg a szoba típusát.");

            if (capacity <= 0)
                throw new ArgumentException("A férőhely legyen pozitív.");

            if (pricePerNight <= 0)
                throw new ArgumentException("Az ár legyen pozitív.");

            if (string.IsNullOrWhiteSpace(equipment))
                throw new ArgumentException("Add meg a felszereltséget.");

            RoomNumber = roomNumber;
            RoomType = roomType;
            Capacity = capacity;
            PricePerNight = pricePerNight;
            Equipment = equipment;
            IsActive = isActive;
        }

        public override string ToString()
        {
            return $"Szoba {RoomNumber} - {RoomType}, {Capacity} fő, " +
                   $"{PricePerNight:N0} Ft/éj, Aktív: {IsActive}";
        }
    }
}

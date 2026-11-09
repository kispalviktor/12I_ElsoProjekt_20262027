
using System;
using System.Collections.Generic;
using System.Linq;

namespace SzallodaAlkalmazas
{
    public class Hotel
    {
        public List<Room> Rooms { get; } = new List<Room>();
        public List<Guest> Guests { get; } = new List<Guest>();
        public List<Booking> Bookings { get; } = new List<Booking>();

        public void AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (Rooms.Exists(r => r.RoomNumber == room.RoomNumber))
                throw new InvalidOperationException(
                    "Ez a szobaszám már létezik.");

            Rooms.Add(room);
        }

        public void AddGuest(Guest guest)
        {
            if (guest == null)
                throw new ArgumentNullException(nameof(guest));

            if (Guests.Exists(g => g.Id == guest.Id))
                throw new InvalidOperationException(
                    "Ez a vendég már létezik.");

            Guests.Add(guest);
        }

        public bool IsRoomAvailable(int roomNumber,
            DateTime checkIn, DateTime checkOut)
        {
            if (checkOut.Date <= checkIn.Date)
                return false;

            Room room = Rooms.Find(r => r.RoomNumber == roomNumber);

            if (room == null || !room.IsActive)
                return false;

            foreach (Booking booking in Bookings)
            {
                if (booking.RoomNumber == roomNumber &&
                    booking.Status == "Aktív" &&
                    checkIn.Date < booking.CheckOut.Date &&
                    checkOut.Date > booking.CheckIn.Date)
                {
                    return false;
                }
            }

            return true;
        }

        public void AddBooking(Booking booking)
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            if (Bookings.Exists(b => b.Id == booking.Id))
                throw new InvalidOperationException(
                    "Ez a foglalásazonosító már létezik.");

            if (!Guests.Exists(g => g.Id == booking.GuestId))
                throw new InvalidOperationException(
                    "A vendég nem található.");

            Room room = Rooms.Find(
                r => r.RoomNumber == booking.RoomNumber);

            if (room == null)
                throw new InvalidOperationException(
                    "A szoba nem található.");

            if (booking.GuestCount > room.Capacity)
                throw new InvalidOperationException(
                    "Túl sok vendég a szoba férőhelyéhez képest.");

            if (!IsRoomAvailable(booking.RoomNumber,
                booking.CheckIn, booking.CheckOut))
                throw new InvalidOperationException(
                    "A szoba ebben az időszakban már foglalt.");

            Bookings.Add(booking);
        }

        public List<Room> FindAvailableRooms(
            DateTime checkIn, DateTime checkOut, int guestCount)
        {
            if (checkOut.Date <= checkIn.Date || guestCount <= 0)
                throw new ArgumentException("Hibás dátum vagy vendégszám.");

            return Rooms.FindAll(r =>
                r.IsActive &&
                r.Capacity >= guestCount &&
                IsRoomAvailable(r.RoomNumber, checkIn, checkOut));
        }
    }
}

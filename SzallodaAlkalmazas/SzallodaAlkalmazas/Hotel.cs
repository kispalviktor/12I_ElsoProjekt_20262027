using System;
using System.Collections.Generic;

namespace SzallodaAlkalmazas
{
    public class Hotel
    {
        public List<Room> Rooms { get; } = new List<Room>();
        public List<Guest> Guests { get; } = new List<Guest>();
        public List<Booking> Bookings { get; } = new List<Booking>();

        // ===================== KERESŐ METÓDUSOK =====================

        public Room FindRoom(int roomNumber)
        {
            foreach (Room room in Rooms)
            {
                if (room.RoomNumber == roomNumber)
                    return room;
            }
            return null;
        }

        public Guest FindGuest(int id)
        {
            foreach (Guest guest in Guests)
            {
                if (guest.Id == id)
                    return guest;
            }
            return null;
        }

        public Booking FindBooking(int id)
        {
            foreach (Booking booking in Bookings)
            {
                if (booking.Id == id)
                    return booking;
            }
            return null;
        }

        // ===================== SZOBÁK =====================

        public void AddRoom(Room room)
        {
            if (room == null)
                throw new ArgumentNullException(nameof(room));

            if (FindRoom(room.RoomNumber) != null)
                throw new InvalidOperationException(
                    "Ez a szobaszám már létezik.");

            Rooms.Add(room);
        }

        // A régi szoba helyére az új adatokkal létrehozott szoba kerül.
        public void UpdateRoom(int oldRoomNumber, Room newRoom)
        {
            if (newRoom == null)
                throw new ArgumentNullException(nameof(newRoom));

            Room oldRoom = FindRoom(oldRoomNumber);
            if (oldRoom == null)
                throw new InvalidOperationException("A szoba nem található.");

            if (newRoom.RoomNumber != oldRoomNumber)
            {
                if (FindRoom(newRoom.RoomNumber) != null)
                    throw new InvalidOperationException(
                        "Ez a szobaszám már létezik.");

                if (RoomHasBookings(oldRoomNumber))
                    throw new InvalidOperationException(
                        "Foglalással rendelkező szoba száma nem módosítható.");
            }

            int index = Rooms.IndexOf(oldRoom);
            Rooms[index] = newRoom;
        }

        public void DeleteRoom(int roomNumber)
        {
            Room room = FindRoom(roomNumber);
            if (room == null)
                throw new InvalidOperationException("A szoba nem található.");

            if (RoomHasBookings(roomNumber))
                throw new InvalidOperationException(
                    "A szobához foglalás tartozik, ezért nem törölhető. " +
                    "Állítsd inkább inaktívra!");

            Rooms.Remove(room);
        }

        public bool RoomHasBookings(int roomNumber)
        {
            foreach (Booking booking in Bookings)
            {
                if (booking.RoomNumber == roomNumber)
                    return true;
            }
            return false;
        }

        // ===================== VENDÉGEK =====================

        public void AddGuest(Guest guest)
        {
            if (guest == null)
                throw new ArgumentNullException(nameof(guest));

            if (FindGuest(guest.Id) != null)
                throw new InvalidOperationException(
                    "Ez a vendég már létezik.");

            Guests.Add(guest);
        }

        // A vendég azonosítója nem változik, csak a többi adata.
        public void UpdateGuest(Guest newGuest)
        {
            if (newGuest == null)
                throw new ArgumentNullException(nameof(newGuest));

            Guest oldGuest = FindGuest(newGuest.Id);
            if (oldGuest == null)
                throw new InvalidOperationException("A vendég nem található.");

            int index = Guests.IndexOf(oldGuest);
            Guests[index] = newGuest;
        }

        public void DeleteGuest(int id)
        {
            Guest guest = FindGuest(id);
            if (guest == null)
                throw new InvalidOperationException("A vendég nem található.");

            foreach (Booking booking in Bookings)
            {
                if (booking.GuestId == id)
                    throw new InvalidOperationException(
                        "A vendéghez foglalás tartozik, ezért nem törölhető.");
            }

            Guests.Remove(guest);
        }

        // A következő szabad vendégazonosító (a legnagyobb + 1)
        public int GetNextGuestId()
        {
            int max = 0;
            foreach (Guest guest in Guests)
            {
                if (guest.Id > max)
                    max = guest.Id;
            }
            return max + 1;
        }

        // ===================== FOGLALÁSOK =====================

        public int GetNextBookingId()
        {
            int max = 0;
            foreach (Booking booking in Bookings)
            {
                if (booking.Id > max)
                    max = booking.Id;
            }
            return max + 1;
        }

        public bool IsRoomAvailable(int roomNumber,
            DateTime checkIn, DateTime checkOut)
        {
            return IsRoomAvailable(roomNumber, checkIn, checkOut, 0);
        }

        // Ütközésvizsgálat. Az ignoredBookingId azonosítójú foglalást nem nézzük
        // (módosításnál a foglalás saját magával ne ütközzön).
        // A lemondott foglalások nem foglalják a szobát.
        public bool IsRoomAvailable(int roomNumber,
            DateTime checkIn, DateTime checkOut, int ignoredBookingId)
        {
            if (checkOut.Date <= checkIn.Date)
                return false;

            Room room = FindRoom(roomNumber);

            if (room == null || !room.IsActive)
                return false;

            foreach (Booking booking in Bookings)
            {
                if (booking.Id == ignoredBookingId)
                    continue;

                if (booking.RoomNumber == roomNumber &&
                    booking.Status != "Lemondva" &&
                    booking.OverlapsWith(checkIn, checkOut))
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

            if (FindBooking(booking.Id) != null)
                throw new InvalidOperationException(
                    "Ez a foglalásazonosító már létezik.");

            CheckBooking(booking, 0);

            Bookings.Add(booking);
        }

        public void UpdateBooking(Booking newBooking)
        {
            if (newBooking == null)
                throw new ArgumentNullException(nameof(newBooking));

            Booking oldBooking = FindBooking(newBooking.Id);
            if (oldBooking == null)
                throw new InvalidOperationException(
                    "A foglalás nem található.");

            CheckBooking(newBooking, newBooking.Id);

            int index = Bookings.IndexOf(oldBooking);
            Bookings[index] = newBooking;
        }

        public void DeleteBooking(int id)
        {
            Booking booking = FindBooking(id);
            if (booking == null)
                throw new InvalidOperationException(
                    "A foglalás nem található.");

            Bookings.Remove(booking);
        }

        // A foglalás ellenőrzése: létezik-e a vendég és a szoba,
        // elfér-e a vendégszám, és szabad-e a szoba az adott időszakban.
        private void CheckBooking(Booking booking, int ignoredBookingId)
        {
            if (FindGuest(booking.GuestId) == null)
                throw new InvalidOperationException(
                    "A vendég nem található.");

            Room room = FindRoom(booking.RoomNumber);

            if (room == null)
                throw new InvalidOperationException(
                    "A szoba nem található.");

            if (booking.GuestCount > room.Capacity)
                throw new InvalidOperationException(
                    "Túl sok vendég a szoba férőhelyéhez képest.");

            // Lemondott foglalásnál nem kell ütközést vizsgálni
            if (booking.Status == "Lemondva")
                return;

            // Inaktív szobára új (aktív) foglalás nem vehető fel
            if (booking.Status == "Aktív" && !room.IsActive)
                throw new InvalidOperationException(
                    "A szoba inaktív, nem foglalható.");

            // Ütközésvizsgálat: van-e másik foglalás, ami átfedi ezt az időszakot
            foreach (Booking other in Bookings)
            {
                if (other.Id == ignoredBookingId)
                    continue;

                if (other.RoomNumber == booking.RoomNumber &&
                    other.Status != "Lemondva" &&
                    other.OverlapsWith(booking.CheckIn, booking.CheckOut))
                {
                    throw new InvalidOperationException(
                        "A szoba ebben az időszakban már foglalt " +
                        "(ütközik a(z) " + other.Id + ". számú foglalással).");
                }
            }
        }

        public List<Room> FindAvailableRooms(
            DateTime checkIn, DateTime checkOut, int guestCount)
        {
            if (checkOut.Date <= checkIn.Date)
                throw new ArgumentException(
                    "A távozásnak az érkezés után kell lennie.");

            if (guestCount <= 0)
                throw new ArgumentException("Hibás vendégszám.");

            List<Room> result = new List<Room>();

            foreach (Room room in Rooms)
            {
                if (room.IsActive &&
                    room.Capacity >= guestCount &&
                    IsRoomAvailable(room.RoomNumber, checkIn, checkOut))
                {
                    result.Add(room);
                }
            }

            return result;
        }
    }
}

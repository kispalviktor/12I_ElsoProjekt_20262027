using System;
using System.Collections.Generic;

namespace SzallodaAlkalmazas
{
    public class RoomStatistics
    {
        private Hotel hotel;

        public RoomStatistics(Hotel hotel)
        {
            this.hotel = hotel;
        }

        public double GetOccupancyRate(DateTime from, DateTime to)
        {
            int napok = (to.Date - from.Date).Days;
            if (napok <= 0)
                throw new ArgumentException("A vége dátumnak a kezdet után kell lennie.");

            int aktivSzobak = 0;
            foreach (Room room in hotel.Rooms)
            {
                if (room.IsActive)
                    aktivSzobak++;
            }

            if (aktivSzobak == 0)
                return 0;

            int foglaltEjszakak = GetBookedNights(from, to);
            int osszesEjszaka = aktivSzobak * napok;

            return (double)foglaltEjszakak / osszesEjszaka * 100;
        }

        public int GetBookedNights(DateTime from, DateTime to)
        {
            int osszesen = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status == "Lemondva")
                    continue;

                DateTime kezdet = booking.CheckIn;
                if (from.Date > kezdet)
                    kezdet = from.Date;

                DateTime veg = booking.CheckOut;
                if (to.Date < veg)
                    veg = to.Date;

                if (veg > kezdet)
                    osszesen += (veg - kezdet).Days;
            }

            return osszesen;
        }

        public int GetOccupiedRoomCount(DateTime day)
        {
            int darab = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status != "Lemondva" &&
                    booking.OverlapsWith(day.Date, day.Date.AddDays(1)))
                {
                    darab++;
                }
            }

            return darab;
        }

        public int CountBookingsOfRoom(int roomNumber)
        {
            int darab = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.RoomNumber == roomNumber && booking.Status != "Lemondva")
                    darab++;
            }

            return darab;
        }

        public int GetMostBookedRoomNumber()
        {
            int legjobbSzoba = 0;
            int legtobbFoglalas = 0;

            foreach (Room room in hotel.Rooms)
            {
                int foglalasok = CountBookingsOfRoom(room.RoomNumber);

                if (foglalasok > legtobbFoglalas)
                {
                    legtobbFoglalas = foglalasok;
                    legjobbSzoba = room.RoomNumber;
                }
            }

            return legjobbSzoba;
        }

        public int CountBookingsOfRoomType(string roomType)
        {
            int darab = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status == "Lemondva")
                    continue;

                Room room = hotel.FindRoom(booking.RoomNumber);
                if (room != null && room.RoomType == roomType)
                    darab++;
            }

            return darab;
        }

        public string GetMostPopularRoomType()
        {
            List<string> tipusok = new List<string>();
            foreach (Room room in hotel.Rooms)
            {
                if (!tipusok.Contains(room.RoomType))
                    tipusok.Add(room.RoomType);
            }

            string legjobbTipus = "";
            int legtobbFoglalas = 0;

            foreach (string tipus in tipusok)
            {
                int foglalasok = CountBookingsOfRoomType(tipus);

                if (foglalasok > legtobbFoglalas)
                {
                    legtobbFoglalas = foglalasok;
                    legjobbTipus = tipus;
                }
            }

            return legjobbTipus;
        }
    }
}

using System;
using System.Collections.Generic;

namespace SzallodaAlkalmazas
{
    // Szobákhoz kapcsolódó statisztikák: foglaltság, legtöbbet foglalt szoba,
    // legnépszerűbb szobatípus.
    // A lemondott foglalásokat egyik számításban sem vesszük figyelembe.
    public class RoomStatistics
    {
        private Hotel hotel;

        public RoomStatistics(Hotel hotel)
        {
            this.hotel = hotel;
        }

        // ===================== FOGLALTSÁG =====================

        // Foglaltság százalékban egy időszakra (a "to" napja már nem számít bele).
        // Foglaltság = foglalt éjszakák száma / (aktív szobák száma × éjszakák száma) × 100
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

        // Hány szoba-éjszaka foglalt egy időszakban.
        // Ha egy foglalás csak részben esik az időszakba, csak azt a részt számoljuk.
        public int GetBookedNights(DateTime from, DateTime to)
        {
            int osszesen = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status == "Lemondva")
                    continue;

                // A foglalás és az időszak közös része
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

        // Hány szoba foglalt egy adott éjszakára (pl. ma este)
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

        // ===================== LEGTÖBBSZÖR FOGLALT SZOBA =====================

        // Hányszor foglalták le az adott szobát
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

        // A legtöbbször lefoglalt szoba száma. Ha nincs foglalás, 0-t ad vissza.
        // Egyenlőség esetén a listában előbb szereplő szoba nyer.
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

        // ===================== LEGNÉPSZERŰBB SZOBATÍPUS =====================

        // Hány foglalás volt az adott típusú szobákra
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

        // A legtöbbet foglalt szobatípus neve. Ha nincs foglalás, üres szöveget ad vissza.
        public string GetMostPopularRoomType()
        {
            // Először összegyűjtjük, milyen szobatípusok vannak (mindegyiket egyszer)
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

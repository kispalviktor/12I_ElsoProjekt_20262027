using System;

namespace SzallodaAlkalmazas
{
    // Foglalásokhoz kapcsolódó statisztikák: foglalások száma, bevétel,
    // egy hónapban érkezett vendégek száma.
    // A lemondott foglalások nem hoznak bevételt és nem számítanak érkezésnek.
    public class BookingStatistics
    {
        private Hotel hotel;

        public BookingStatistics(Hotel hotel)
        {
            this.hotel = hotel;
        }

        // ===================== FOGLALÁSOK SZÁMA =====================

        // Az összes foglalás száma (a lemondottakkal együtt)
        public int GetBookingCount()
        {
            return hotel.Bookings.Count;
        }

        // Adott állapotú foglalások száma ("Aktív", "Teljesítve", "Lemondva")
        public int CountBookingsByStatus(string status)
        {
            int darab = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status == status)
                    darab++;
            }

            return darab;
        }

        // ===================== BEVÉTEL =====================

        // A szálloda teljes bevétele (az aktív és a teljesített foglalások összege)
        public decimal GetTotalRevenue()
        {
            decimal osszeg = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status != "Lemondva")
                    osszeg += booking.TotalPrice;
            }

            return osszeg;
        }

        // Egy adott hónap bevétele. Azt a foglalást számoljuk a hónaphoz,
        // amelyiknek az érkezési napja ebbe a hónapba esik.
        public decimal GetRevenueInMonth(int year, int month)
        {
            decimal osszeg = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status != "Lemondva" &&
                    booking.CheckIn.Year == year &&
                    booking.CheckIn.Month == month)
                {
                    osszeg += booking.TotalPrice;
                }
            }

            return osszeg;
        }

        // ===================== ÉRKEZŐ VENDÉGEK =====================

        // Hány vendég érkezett egy adott hónapban (a foglalások vendégszámainak összege)
        public int GetGuestCountInMonth(int year, int month)
        {
            int vendegek = 0;

            foreach (Booking booking in hotel.Bookings)
            {
                if (booking.Status != "Lemondva" &&
                    booking.CheckIn.Year == year &&
                    booking.CheckIn.Month == month)
                {
                    vendegek += booking.GuestCount;
                }
            }

            return vendegek;
        }
    }
}

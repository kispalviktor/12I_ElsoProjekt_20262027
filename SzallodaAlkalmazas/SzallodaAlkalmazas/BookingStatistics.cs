using System;

namespace SzallodaAlkalmazas
{
    public class BookingStatistics
    {
        private Hotel hotel;

        public BookingStatistics(Hotel hotel)
        {
            this.hotel = hotel;
        }

        public int GetBookingCount()
        {
            return hotel.Bookings.Count;
        }

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

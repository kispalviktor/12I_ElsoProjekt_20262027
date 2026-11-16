using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SzallodaAlkalmazas
{
    public class FileManager
    {
        private const string RoomsHeader =
            "RoomNumber;RoomType;Capacity;PricePerNight;Equipment;IsActive";
        private const string GuestsHeader =
            "Id;Name;Phone;Email";
        private const string BookingsHeader =
            "Id;GuestId;RoomNumber;CheckIn;CheckOut;GuestCount;Status;TotalPrice";

        private const string DateFormat = "yyyy-MM-dd";

        private string roomsFilePath;
        private string guestsFilePath;
        private string bookingsFilePath;

        public FileManager(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            roomsFilePath = Path.Combine(folder, "rooms.csv");
            guestsFilePath = Path.Combine(folder, "guests.csv");
            bookingsFilePath = Path.Combine(folder, "bookings.csv");
        }

        public List<string> LoadAllData(Hotel hotel)
        {
            List<string> messages = new List<string>();

            LoadRooms(hotel, messages);
            LoadGuests(hotel, messages);
            LoadBookings(hotel, messages);

            return messages;
        }

        private void LoadRooms(Hotel hotel, List<string> messages)
        {
            string[] lines = ReadFile(roomsFilePath, messages);
            if (lines == null)
                return;

            int badLines = 0;

            for (int i = FirstDataLine(lines, RoomsHeader); i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                try
                {
                    string[] data = SplitLine(lines[i], 6);

                    int roomNumber = int.Parse(data[0]);
                    string roomType = data[1];
                    int capacity = int.Parse(data[2]);
                    decimal price = decimal.Parse(data[3], CultureInfo.InvariantCulture);
                    string equipment = data[4];
                    bool isActive = bool.Parse(data[5]);

                    hotel.AddRoom(new Room(roomNumber, roomType, capacity,
                        price, equipment, isActive));
                }
                catch (Exception ex)
                {
                    badLines++;
                    messages.Add("rooms.csv " + (i + 1) + ". sor hibás, kihagyva: " + ex.Message);
                }
            }

            if (badLines > 0)
                MakeBackup(roomsFilePath, messages);
        }

        private void LoadGuests(Hotel hotel, List<string> messages)
        {
            string[] lines = ReadFile(guestsFilePath, messages);
            if (lines == null)
                return;

            int badLines = 0;

            for (int i = FirstDataLine(lines, GuestsHeader); i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                try
                {
                    string[] data = SplitLine(lines[i], 4);

                    int id = int.Parse(data[0]);
                    string name = data[1];
                    string phone = data[2];
                    string email = data[3];

                    hotel.AddGuest(new Guest(id, name, phone, email));
                }
                catch (Exception ex)
                {
                    badLines++;
                    messages.Add("guests.csv " + (i + 1) + ". sor hibás, kihagyva: " + ex.Message);
                }
            }

            if (badLines > 0)
                MakeBackup(guestsFilePath, messages);
        }

        private void LoadBookings(Hotel hotel, List<string> messages)
        {
            string[] lines = ReadFile(bookingsFilePath, messages);
            if (lines == null)
                return;

            int badLines = 0;

            for (int i = FirstDataLine(lines, BookingsHeader); i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                try
                {
                    string[] data = SplitLine(lines[i], 8);

                    int id = int.Parse(data[0]);
                    int guestId = int.Parse(data[1]);
                    int roomNumber = int.Parse(data[2]);
                    DateTime checkIn = DateTime.ParseExact(data[3], DateFormat, CultureInfo.InvariantCulture);
                    DateTime checkOut = DateTime.ParseExact(data[4], DateFormat, CultureInfo.InvariantCulture);
                    int guestCount = int.Parse(data[5]);
                    string status = data[6];
                    decimal totalPrice = decimal.Parse(data[7], CultureInfo.InvariantCulture);

                    hotel.AddBooking(new Booking(id, guestId, roomNumber,
                        checkIn, checkOut, guestCount, status, totalPrice));
                }
                catch (Exception ex)
                {
                    badLines++;
                    messages.Add("bookings.csv " + (i + 1) + ". sor hibás, kihagyva: " + ex.Message);
                }
            }

            if (badLines > 0)
                MakeBackup(bookingsFilePath, messages);
        }

        private string[] ReadFile(string path, List<string> messages)
        {
            string fileName = Path.GetFileName(path);

            if (!File.Exists(path))
            {
                messages.Add(fileName + " nem található, üres listával indulunk. " +
                    "A fájl a program bezárásakor létrejön.");
                return null;
            }

            try
            {
                return File.ReadAllLines(path, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                messages.Add(fileName + " nem olvasható: " + ex.Message);
                MakeBackup(path, messages);
                return null;
            }
        }

        private int FirstDataLine(string[] lines, string header)
        {
            if (lines.Length > 0 && lines[0].Trim() == header)
                return 1;

            return 0;
        }

        private string[] SplitLine(string line, int columnCount)
        {
            string[] data = line.Split(';');

            if (data.Length != columnCount)
                throw new FormatException(columnCount + " adatnak kellene lennie, de " +
                    data.Length + " van.");

            for (int i = 0; i < data.Length; i++)
                data[i] = data[i].Trim();

            return data;
        }

        private void MakeBackup(string path, List<string> messages)
        {
            string backupPath = path + ".hibas.bak";

            try
            {
                File.Copy(path, backupPath, true);
                messages.Add("Az eredeti fájlról másolat készült: " + Path.GetFileName(backupPath));
            }
            catch (Exception ex)
            {
                messages.Add("Nem sikerült biztonsági másolatot készíteni: " + ex.Message);
            }
        }

        public void SaveAllData(Hotel hotel)
        {
            SaveRooms(hotel);
            SaveGuests(hotel);
            SaveBookings(hotel);
        }

        private void SaveRooms(Hotel hotel)
        {
            List<string> lines = new List<string>();
            lines.Add(RoomsHeader);

            foreach (Room room in hotel.Rooms)
            {
                lines.Add(room.RoomNumber + ";" +
                    Clean(room.RoomType) + ";" +
                    room.Capacity + ";" +
                    room.PricePerNight.ToString(CultureInfo.InvariantCulture) + ";" +
                    Clean(room.Equipment) + ";" +
                    room.IsActive);
            }

            WriteFile(roomsFilePath, lines);
        }

        private void SaveGuests(Hotel hotel)
        {
            List<string> lines = new List<string>();
            lines.Add(GuestsHeader);

            foreach (Guest guest in hotel.Guests)
            {
                lines.Add(guest.Id + ";" +
                    Clean(guest.Name) + ";" +
                    Clean(guest.Phone) + ";" +
                    Clean(guest.Email));
            }

            WriteFile(guestsFilePath, lines);
        }

        private void SaveBookings(Hotel hotel)
        {
            List<string> lines = new List<string>();
            lines.Add(BookingsHeader);

            foreach (Booking booking in hotel.Bookings)
            {
                lines.Add(booking.Id + ";" +
                    booking.GuestId + ";" +
                    booking.RoomNumber + ";" +
                    booking.CheckIn.ToString(DateFormat, CultureInfo.InvariantCulture) + ";" +
                    booking.CheckOut.ToString(DateFormat, CultureInfo.InvariantCulture) + ";" +
                    booking.GuestCount + ";" +
                    Clean(booking.Status) + ";" +
                    booking.TotalPrice.ToString(CultureInfo.InvariantCulture));
            }

            WriteFile(bookingsFilePath, lines);
        }

        private void WriteFile(string path, List<string> lines)
        {
            string tempPath = path + ".tmp";

            File.WriteAllLines(tempPath, lines, Encoding.UTF8);
            File.Copy(tempPath, path, true);
            File.Delete(tempPath);
        }

        private string Clean(string text)
        {
            if (text == null)
                return "";

            return text.Replace(";", ",").Replace("\r", " ").Replace("\n", " ");
        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SzallodaAlkalmazas
{
    // Statisztikák, foglaltság, bevétel és árkalkuláció megjelenítése
    public class StatisztikakForm : Form
    {
        private Hotel hotel;
        private RoomStatistics szobaStatisztika;
        private BookingStatistics foglalasStatisztika;

        // Összesített statisztikák
        private Label foglalasokLabel = new Label();
        private Label legtobbSzobaLabel = new Label();
        private Label bevetelLabel = new Label();
        private Label tipusLabel = new Label();
        private Label maFoglaltLabel = new Label();

        // Havi statisztikák
        private DateTimePicker honapPicker = new DateTimePicker();
        private Label vendegekHonapLabel = new Label();
        private Label bevetelHonapLabel = new Label();
        private Label foglaltsagHonapLabel = new Label();
        private Label ejszakakHonapLabel = new Label();

        // Árkalkuláció
        private ComboBox szobaComboBox = new ComboBox();
        private DateTimePicker erkezesPicker = new DateTimePicker();
        private DateTimePicker tavozasPicker = new DateTimePicker();
        private Label arEredmenyLabel = new Label();

        public StatisztikakForm(Hotel hotel)
        {
            this.hotel = hotel;
            szobaStatisztika = new RoomStatistics(hotel);
            foglalasStatisztika = new BookingStatistics(hotel);

            Text = "Statisztikák";
            ClientSize = new Size(900, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ---- Összesített statisztikák ----
            GroupBox osszesitettGroup = new GroupBox();
            osszesitettGroup.Text = "Összesített statisztikák";
            osszesitettGroup.Location = new Point(12, 12);
            osszesitettGroup.Size = new Size(430, 240);

            EredmenyCimke(foglalasokLabel, 25);
            foglalasokLabel.Height = 40;   // ez a címke két soros
            EredmenyCimke(legtobbSzobaLabel, 75);
            EredmenyCimke(bevetelLabel, 110);
            EredmenyCimke(tipusLabel, 145);
            EredmenyCimke(maFoglaltLabel, 180);

            osszesitettGroup.Controls.Add(foglalasokLabel);
            osszesitettGroup.Controls.Add(legtobbSzobaLabel);
            osszesitettGroup.Controls.Add(bevetelLabel);
            osszesitettGroup.Controls.Add(tipusLabel);
            osszesitettGroup.Controls.Add(maFoglaltLabel);

            Controls.Add(osszesitettGroup);

            // ---- Havi statisztikák ----
            GroupBox haviGroup = new GroupBox();
            haviGroup.Text = "Havi statisztikák";
            haviGroup.Location = new Point(458, 12);
            haviGroup.Size = new Size(430, 240);

            Label honapCimke = new Label();
            honapCimke.Text = "Hónap:";
            honapCimke.Location = new Point(15, 33);
            honapCimke.Size = new Size(60, 20);
            haviGroup.Controls.Add(honapCimke);

            honapPicker.Location = new Point(80, 30);
            honapPicker.Size = new Size(160, 22);
            honapPicker.Format = DateTimePickerFormat.Custom;
            honapPicker.CustomFormat = "yyyy. MMMM";
            honapPicker.ShowUpDown = true;
            honapPicker.Value = DateTime.Today;
            haviGroup.Controls.Add(honapPicker);

            Button haviGomb = new Button();
            haviGomb.Text = "Számítás";
            haviGomb.Location = new Point(260, 27);
            haviGomb.Size = new Size(120, 30);
            haviGomb.Click += HaviGomb_Click;
            haviGroup.Controls.Add(haviGomb);

            EredmenyCimke(vendegekHonapLabel, 80);
            EredmenyCimke(bevetelHonapLabel, 115);
            EredmenyCimke(foglaltsagHonapLabel, 150);
            EredmenyCimke(ejszakakHonapLabel, 185);

            haviGroup.Controls.Add(vendegekHonapLabel);
            haviGroup.Controls.Add(bevetelHonapLabel);
            haviGroup.Controls.Add(foglaltsagHonapLabel);
            haviGroup.Controls.Add(ejszakakHonapLabel);

            Controls.Add(haviGroup);

            // ---- Árkalkuláció ----
            GroupBox arGroup = new GroupBox();
            arGroup.Text = "Árkalkuláció";
            arGroup.Location = new Point(12, 265);
            arGroup.Size = new Size(876, 180);

            Label szobaCimke = new Label();
            szobaCimke.Text = "Szoba:";
            szobaCimke.Location = new Point(15, 33);
            szobaCimke.Size = new Size(70, 20);
            arGroup.Controls.Add(szobaCimke);

            szobaComboBox.Location = new Point(90, 30);
            szobaComboBox.Size = new Size(380, 22);
            szobaComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            szobaComboBox.DropDownWidth = 450;
            foreach (Room szoba in hotel.Rooms)
            {
                if (szoba.IsActive)
                    szobaComboBox.Items.Add(szoba);
            }
            arGroup.Controls.Add(szobaComboBox);

            Label erkezesCimke = new Label();
            erkezesCimke.Text = "Érkezés:";
            erkezesCimke.Location = new Point(15, 68);
            erkezesCimke.Size = new Size(70, 20);
            arGroup.Controls.Add(erkezesCimke);

            erkezesPicker.Location = new Point(90, 65);
            erkezesPicker.Size = new Size(140, 22);
            erkezesPicker.Format = DateTimePickerFormat.Custom;
            erkezesPicker.CustomFormat = "yyyy.MM.dd.";
            erkezesPicker.Value = DateTime.Today;
            arGroup.Controls.Add(erkezesPicker);

            Label tavozasCimke = new Label();
            tavozasCimke.Text = "Távozás:";
            tavozasCimke.Location = new Point(250, 68);
            tavozasCimke.Size = new Size(70, 20);
            arGroup.Controls.Add(tavozasCimke);

            tavozasPicker.Location = new Point(330, 65);
            tavozasPicker.Size = new Size(140, 22);
            tavozasPicker.Format = DateTimePickerFormat.Custom;
            tavozasPicker.CustomFormat = "yyyy.MM.dd.";
            tavozasPicker.Value = DateTime.Today.AddDays(1);
            arGroup.Controls.Add(tavozasPicker);

            Button arGomb = new Button();
            arGomb.Text = "Ár kiszámítása";
            arGomb.Location = new Point(500, 28);
            arGomb.Size = new Size(150, 30);
            arGomb.Click += ArGomb_Click;
            arGroup.Controls.Add(arGomb);

            arEredmenyLabel.Location = new Point(15, 115);
            arEredmenyLabel.Size = new Size(840, 40);
            arEredmenyLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            arGroup.Controls.Add(arEredmenyLabel);

            Controls.Add(arGroup);

            OsszesitettFrissitese();
            HaviFrissitese();
        }

        private void EredmenyCimke(Label cimke, int y)
        {
            cimke.Location = new Point(15, y);
            cimke.Size = new Size(400, 30);
            cimke.Font = new Font("Arial", 10);
        }

        // ===================== SZÁMÍTÁSOK =====================

        private void OsszesitettFrissitese()
        {
            // 1. Hány foglalás történt?
            foglalasokLabel.Text = "Foglalások száma: " + foglalasStatisztika.GetBookingCount() +
                "\n(aktív: " + foglalasStatisztika.CountBookingsByStatus("Aktív") +
                ", teljesített: " + foglalasStatisztika.CountBookingsByStatus("Teljesítve") +
                ", lemondott: " + foglalasStatisztika.CountBookingsByStatus("Lemondva") + ")";

            // 2. Melyik szobát foglalták le a legtöbbször?
            int legtobbSzoba = szobaStatisztika.GetMostBookedRoomNumber();
            if (legtobbSzoba == 0)
            {
                legtobbSzobaLabel.Text = "Legtöbbször foglalt szoba: nincs adat";
            }
            else
            {
                legtobbSzobaLabel.Text = "Legtöbbször foglalt szoba: " + legtobbSzoba +
                    " (" + szobaStatisztika.CountBookingsOfRoom(legtobbSzoba) + " alkalommal)";
            }

            // 3. Mennyi volt a szálloda bevétele?
            bevetelLabel.Text = "Teljes bevétel: " +
                foglalasStatisztika.GetTotalRevenue().ToString("N0") + " Ft";

            // 5. Melyik szobatípus volt a legnépszerűbb?
            string tipus = szobaStatisztika.GetMostPopularRoomType();
            if (tipus == "")
            {
                tipusLabel.Text = "Legnépszerűbb szobatípus: nincs adat";
            }
            else
            {
                tipusLabel.Text = "Legnépszerűbb szobatípus: " + tipus +
                    " (" + szobaStatisztika.CountBookingsOfRoomType(tipus) + " foglalás)";
            }

            // Foglaltság ma
            int aktivSzobak = 0;
            foreach (Room szoba in hotel.Rooms)
            {
                if (szoba.IsActive)
                    aktivSzobak++;
            }

            maFoglaltLabel.Text = "Ma foglalt szobák: " +
                szobaStatisztika.GetOccupiedRoomCount(DateTime.Today) + " / " + aktivSzobak;
        }

        private void HaviFrissitese()
        {
            int ev = honapPicker.Value.Year;
            int honap = honapPicker.Value.Month;

            DateTime elsoNap = new DateTime(ev, honap, 1);
            DateTime kovetkezoHonap = elsoNap.AddMonths(1);

            // 4. Hány vendég érkezett egy adott hónapban?
            vendegekHonapLabel.Text = "Érkezett vendégek: " +
                foglalasStatisztika.GetGuestCountInMonth(ev, honap) + " fő";

            bevetelHonapLabel.Text = "Bevétel a hónapban: " +
                foglalasStatisztika.GetRevenueInMonth(ev, honap).ToString("N0") + " Ft";

            double foglaltsag = szobaStatisztika.GetOccupancyRate(elsoNap, kovetkezoHonap);
            foglaltsagHonapLabel.Text = "Foglaltság: " + foglaltsag.ToString("0.0") + " %";

            ejszakakHonapLabel.Text = "Foglalt szoba-éjszakák: " +
                szobaStatisztika.GetBookedNights(elsoNap, kovetkezoHonap);
        }

        // ===================== ESEMÉNYEK =====================

        private void HaviGomb_Click(object sender, EventArgs e)
        {
            HaviFrissitese();
        }

        private void ArGomb_Click(object sender, EventArgs e)
        {
            Room szoba = (Room)szobaComboBox.SelectedItem;

            if (szoba == null)
            {
                MessageBox.Show("Válassz szobát!");
                return;
            }

            DateTime erkezes = erkezesPicker.Value.Date;
            DateTime tavozas = tavozasPicker.Value.Date;

            if (tavozas <= erkezes)
            {
                MessageBox.Show("A távozásnak az érkezés után kell lennie.", "Hiba",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int ejszakak = (tavozas - erkezes).Days;
            decimal ar = Booking.CalculatePrice(erkezes, tavozas, szoba.PricePerNight);

            string szabad = "szabad";
            if (!hotel.IsRoomAvailable(szoba.RoomNumber, erkezes, tavozas))
                szabad = "ebben az időszakban FOGLALT";

            arEredmenyLabel.Text = ejszakak + " éjszaka × " +
                szoba.PricePerNight.ToString("N0") + " Ft = " + ar.ToString("N0") + " Ft" +
                "   (a szoba " + szabad + ")";
        }
    }
}

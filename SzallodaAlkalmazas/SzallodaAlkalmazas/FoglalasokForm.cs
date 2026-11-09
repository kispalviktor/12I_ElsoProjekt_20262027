using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SzallodaAlkalmazas
{
    // Foglalások kezelése: létrehozás, módosítás, törlés, listázás, keresés azonosító alapján
    public class FoglalasokForm : Form
    {
        private Hotel hotel;

        // A táblázatban kiválasztott foglalás azonosítója (0 = nincs kiválasztva)
        private int kivalasztottId = 0;

        private DataGridView foglalasokGrid = new DataGridView();
        private NumericUpDown keresesNumeric = new NumericUpDown();

        private Label azonositoErtekLabel = new Label();
        private ComboBox vendegComboBox = new ComboBox();
        private ComboBox szobaComboBox = new ComboBox();
        private DateTimePicker erkezesPicker = new DateTimePicker();
        private DateTimePicker tavozasPicker = new DateTimePicker();
        private NumericUpDown vendegszamNumeric = new NumericUpDown();
        private ComboBox allapotComboBox = new ComboBox();
        private Label ejszakakErtekLabel = new Label();
        private Label osszegErtekLabel = new Label();

        private Label darabLabel = new Label();

        public FoglalasokForm(Hotel hotel)
        {
            this.hotel = hotel;

            Text = "Foglalások kezelése";
            ClientSize = new Size(1000, 640);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ---- Táblázat ----
            foglalasokGrid.Location = new Point(12, 12);
            foglalasokGrid.Size = new Size(976, 300);
            foglalasokGrid.ReadOnly = true;
            foglalasokGrid.AllowUserToAddRows = false;
            foglalasokGrid.AllowUserToDeleteRows = false;
            foglalasokGrid.MultiSelect = false;
            foglalasokGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            foglalasokGrid.RowHeadersVisible = false;
            foglalasokGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foglalasokGrid.Columns.Add("id", "Azonosító");
            foglalasokGrid.Columns.Add("vendeg", "Vendég");
            foglalasokGrid.Columns.Add("szoba", "Szoba");
            foglalasokGrid.Columns.Add("erkezes", "Érkezés");
            foglalasokGrid.Columns.Add("tavozas", "Távozás");
            foglalasokGrid.Columns.Add("ejszakak", "Éjszakák");
            foglalasokGrid.Columns.Add("fo", "Vendégek");
            foglalasokGrid.Columns.Add("allapot", "Állapot");
            foglalasokGrid.Columns.Add("ar", "Teljes ár");
            foglalasokGrid.Columns["vendeg"].FillWeight = 200;

            foglalasokGrid.CellClick += FoglalasokGrid_CellClick;
            Controls.Add(foglalasokGrid);

            // ---- Keresés ----
            GroupBox keresesGroup = new GroupBox();
            keresesGroup.Text = "Keresés azonosító alapján";
            keresesGroup.Location = new Point(12, 322);
            keresesGroup.Size = new Size(300, 120);

            keresesNumeric.Location = new Point(15, 30);
            keresesNumeric.Size = new Size(120, 22);
            keresesNumeric.Minimum = 1;
            keresesNumeric.Maximum = 1000000;
            keresesGroup.Controls.Add(keresesNumeric);

            Button keresesGomb = new Button();
            keresesGomb.Text = "Keresés";
            keresesGomb.Location = new Point(15, 70);
            keresesGomb.Size = new Size(130, 30);
            keresesGomb.Click += KeresesGomb_Click;
            keresesGroup.Controls.Add(keresesGomb);

            Button osszesGomb = new Button();
            osszesGomb.Text = "Összes foglalás";
            osszesGomb.Location = new Point(155, 70);
            osszesGomb.Size = new Size(130, 30);
            osszesGomb.Click += OsszesGomb_Click;
            keresesGroup.Controls.Add(osszesGomb);

            Controls.Add(keresesGroup);

            // ---- Foglalás adatai ----
            GroupBox adatokGroup = new GroupBox();
            adatokGroup.Text = "Foglalás adatai";
            adatokGroup.Location = new Point(324, 322);
            adatokGroup.Size = new Size(664, 290);

            adatokGroup.Controls.Add(UjCimke("Azonosító:", 15, 28));
            azonositoErtekLabel.Location = new Point(110, 28);
            azonositoErtekLabel.Size = new Size(200, 20);
            adatokGroup.Controls.Add(azonositoErtekLabel);

            adatokGroup.Controls.Add(UjCimke("Vendég:", 15, 58));
            vendegComboBox.Location = new Point(110, 55);
            vendegComboBox.Size = new Size(340, 22);
            vendegComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            vendegComboBox.DropDownWidth = 450;
            foreach (Guest vendeg in hotel.Guests)
                vendegComboBox.Items.Add(vendeg);
            adatokGroup.Controls.Add(vendegComboBox);

            adatokGroup.Controls.Add(UjCimke("Szoba:", 15, 88));
            szobaComboBox.Location = new Point(110, 85);
            szobaComboBox.Size = new Size(340, 22);
            szobaComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            szobaComboBox.DropDownWidth = 450;
            foreach (Room szoba in hotel.Rooms)
                szobaComboBox.Items.Add(szoba);
            szobaComboBox.SelectedIndexChanged += AdatValtozott;
            adatokGroup.Controls.Add(szobaComboBox);

            adatokGroup.Controls.Add(UjCimke("Érkezés:", 15, 118));
            erkezesPicker.Location = new Point(110, 115);
            erkezesPicker.Size = new Size(140, 22);
            erkezesPicker.Format = DateTimePickerFormat.Custom;
            erkezesPicker.CustomFormat = "yyyy.MM.dd.";
            erkezesPicker.ValueChanged += AdatValtozott;
            adatokGroup.Controls.Add(erkezesPicker);

            adatokGroup.Controls.Add(UjCimke("Távozás:", 15, 148));
            tavozasPicker.Location = new Point(110, 145);
            tavozasPicker.Size = new Size(140, 22);
            tavozasPicker.Format = DateTimePickerFormat.Custom;
            tavozasPicker.CustomFormat = "yyyy.MM.dd.";
            tavozasPicker.ValueChanged += AdatValtozott;
            adatokGroup.Controls.Add(tavozasPicker);

            adatokGroup.Controls.Add(UjCimke("Vendégek:", 15, 178));
            vendegszamNumeric.Location = new Point(110, 175);
            vendegszamNumeric.Size = new Size(80, 22);
            vendegszamNumeric.Minimum = 1;
            vendegszamNumeric.Maximum = 50;
            adatokGroup.Controls.Add(vendegszamNumeric);

            adatokGroup.Controls.Add(UjCimke("Állapot:", 15, 208));
            allapotComboBox.Location = new Point(110, 205);
            allapotComboBox.Size = new Size(140, 22);
            allapotComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            allapotComboBox.Items.Add("Aktív");
            allapotComboBox.Items.Add("Teljesítve");
            allapotComboBox.Items.Add("Lemondva");
            adatokGroup.Controls.Add(allapotComboBox);

            adatokGroup.Controls.Add(UjCimke("Éjszakák:", 270, 118));
            ejszakakErtekLabel.Location = new Point(350, 118);
            ejszakakErtekLabel.Size = new Size(100, 20);
            adatokGroup.Controls.Add(ejszakakErtekLabel);

            adatokGroup.Controls.Add(UjCimke("Összeg:", 270, 148));
            osszegErtekLabel.Location = new Point(350, 148);
            osszegErtekLabel.Size = new Size(120, 20);
            osszegErtekLabel.Font = new Font(Font, FontStyle.Bold);
            adatokGroup.Controls.Add(osszegErtekLabel);

            Button ujGomb = UjGomb("Új foglalás mentése", 500, 25);
            Button modositGomb = UjGomb("Módosítás mentése", 500, 62);
            Button torolGomb = UjGomb("Törlés", 500, 99);
            Button uritGomb = UjGomb("Mezők ürítése", 500, 136);

            ujGomb.Click += UjGomb_Click;
            modositGomb.Click += ModositGomb_Click;
            torolGomb.Click += TorolGomb_Click;
            uritGomb.Click += UritGomb_Click;

            adatokGroup.Controls.Add(ujGomb);
            adatokGroup.Controls.Add(modositGomb);
            adatokGroup.Controls.Add(torolGomb);
            adatokGroup.Controls.Add(uritGomb);

            Controls.Add(adatokGroup);

            darabLabel.Location = new Point(12, 600);
            darabLabel.Size = new Size(300, 22);
            Controls.Add(darabLabel);

            TablazatFeltoltese(hotel.Bookings);
            MezokUritese();
        }

        private Label UjCimke(string szoveg, int x, int y)
        {
            Label cimke = new Label();
            cimke.Text = szoveg;
            cimke.Location = new Point(x, y);
            cimke.Size = new Size(80, 20);
            return cimke;
        }

        private Button UjGomb(string szoveg, int x, int y)
        {
            Button gomb = new Button();
            gomb.Text = szoveg;
            gomb.Location = new Point(x, y);
            gomb.Size = new Size(150, 30);
            return gomb;
        }

        private void TablazatFeltoltese(List<Booking> foglalasok)
        {
            foglalasokGrid.Rows.Clear();

            foreach (Booking foglalas in foglalasok)
            {
                // A vendég nevét is kiírjuk, ne csak az azonosítóját
                string vendegNev = foglalas.GuestId.ToString();
                Guest vendeg = hotel.FindGuest(foglalas.GuestId);
                if (vendeg != null)
                    vendegNev = vendeg.Id + " - " + vendeg.Name;

                foglalasokGrid.Rows.Add(foglalas.Id, vendegNev, foglalas.RoomNumber,
                    foglalas.CheckIn.ToString("yyyy.MM.dd."),
                    foglalas.CheckOut.ToString("yyyy.MM.dd."),
                    foglalas.GetNights(), foglalas.GuestCount, foglalas.Status,
                    foglalas.TotalPrice.ToString("N0") + " Ft");
            }

            darabLabel.Text = "Megjelenített foglalások: " + foglalasok.Count + " / " + hotel.Bookings.Count;
        }

        private void MezokUritese()
        {
            kivalasztottId = 0;
            azonositoErtekLabel.Text = "(új foglalás: " + hotel.GetNextBookingId() + ")";
            vendegComboBox.SelectedIndex = -1;
            szobaComboBox.SelectedIndex = -1;
            erkezesPicker.Value = DateTime.Today;
            tavozasPicker.Value = DateTime.Today.AddDays(1);
            vendegszamNumeric.Value = 1;
            allapotComboBox.SelectedIndex = 0;
            foglalasokGrid.ClearSelection();
            ArFrissitese();
        }

        // Az éjszakák számát és az összeget mindig újraszámoljuk, ha a szoba vagy a dátum változik
        private void ArFrissitese()
        {
            Room szoba = (Room)szobaComboBox.SelectedItem;
            DateTime erkezes = erkezesPicker.Value.Date;
            DateTime tavozas = tavozasPicker.Value.Date;

            if (tavozas <= erkezes)
            {
                ejszakakErtekLabel.Text = "hibás dátum";
                osszegErtekLabel.Text = "-";
                return;
            }

            ejszakakErtekLabel.Text = (tavozas - erkezes).Days.ToString();

            if (szoba == null)
            {
                osszegErtekLabel.Text = "-";
                return;
            }

            decimal ar = Booking.CalculatePrice(erkezes, tavozas, szoba.PricePerNight);
            osszegErtekLabel.Text = ar.ToString("N0") + " Ft";
        }

        // A mezők tartalmából új Booking objektum
        private Booking FoglalasAMezokbol(int id)
        {
            Guest vendeg = (Guest)vendegComboBox.SelectedItem;
            Room szoba = (Room)szobaComboBox.SelectedItem;

            if (vendeg == null)
                throw new ArgumentException("Válassz vendéget!");

            if (szoba == null)
                throw new ArgumentException("Válassz szobát!");

            DateTime erkezes = erkezesPicker.Value.Date;
            DateTime tavozas = tavozasPicker.Value.Date;

            if (tavozas <= erkezes)
                throw new ArgumentException("A távozásnak az érkezés után kell lennie.");

            decimal ar = Booking.CalculatePrice(erkezes, tavozas, szoba.PricePerNight);

            return new Booking(id, vendeg.Id, szoba.RoomNumber, erkezes, tavozas,
                (int)vendegszamNumeric.Value, allapotComboBox.Text, ar);
        }

        // ===================== ESEMÉNYEK =====================

        private void AdatValtozott(object sender, EventArgs e)
        {
            ArFrissitese();
        }

        private void FoglalasokGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int id = (int)foglalasokGrid.Rows[e.RowIndex].Cells["id"].Value;
            Booking foglalas = hotel.FindBooking(id);

            if (foglalas == null)
                return;

            kivalasztottId = foglalas.Id;
            azonositoErtekLabel.Text = foglalas.Id.ToString();
            vendegComboBox.SelectedItem = hotel.FindGuest(foglalas.GuestId);
            szobaComboBox.SelectedItem = hotel.FindRoom(foglalas.RoomNumber);
            erkezesPicker.Value = foglalas.CheckIn;
            tavozasPicker.Value = foglalas.CheckOut;
            vendegszamNumeric.Value = foglalas.GuestCount;
            allapotComboBox.Text = foglalas.Status;
            ArFrissitese();
        }

        private void KeresesGomb_Click(object sender, EventArgs e)
        {
            List<Booking> talalatok = new List<Booking>();

            Booking foglalas = hotel.FindBooking((int)keresesNumeric.Value);
            if (foglalas != null)
                talalatok.Add(foglalas);

            TablazatFeltoltese(talalatok);

            if (talalatok.Count == 0)
                MessageBox.Show("Nincs ilyen azonosítójú foglalás.");
        }

        private void OsszesGomb_Click(object sender, EventArgs e)
        {
            TablazatFeltoltese(hotel.Bookings);
        }

        private void UjGomb_Click(object sender, EventArgs e)
        {
            try
            {
                Booking foglalas = FoglalasAMezokbol(hotel.GetNextBookingId());
                hotel.AddBooking(foglalas);

                TablazatFeltoltese(hotel.Bookings);
                MezokUritese();
                MessageBox.Show("A foglalás mentése sikerült.\nÖsszeg: " +
                    foglalas.TotalPrice.ToString("N0") + " Ft");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModositGomb_Click(object sender, EventArgs e)
        {
            if (kivalasztottId == 0)
            {
                MessageBox.Show("Előbb válassz ki egy foglalást a táblázatban!");
                return;
            }

            try
            {
                Booking foglalas = FoglalasAMezokbol(kivalasztottId);
                hotel.UpdateBooking(foglalas);

                TablazatFeltoltese(hotel.Bookings);
                MezokUritese();
                MessageBox.Show("A módosítás sikerült.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TorolGomb_Click(object sender, EventArgs e)
        {
            if (kivalasztottId == 0)
            {
                MessageBox.Show("Előbb válassz ki egy foglalást a táblázatban!");
                return;
            }

            DialogResult valasz = MessageBox.Show(
                "Biztosan törlöd a(z) " + kivalasztottId + ". foglalást?",
                "Törlés", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (valasz != DialogResult.Yes)
                return;

            try
            {
                hotel.DeleteBooking(kivalasztottId);

                TablazatFeltoltese(hotel.Bookings);
                MezokUritese();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UritGomb_Click(object sender, EventArgs e)
        {
            MezokUritese();
        }
    }
}

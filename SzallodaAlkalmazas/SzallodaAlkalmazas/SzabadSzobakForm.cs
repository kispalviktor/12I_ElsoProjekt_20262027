using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SzallodaAlkalmazas
{
    // Szabad szobák keresése adott időszakra és vendégszámra
    public class SzabadSzobakForm : Form
    {
        private Hotel hotel;

        private DateTimePicker erkezesPicker = new DateTimePicker();
        private DateTimePicker tavozasPicker = new DateTimePicker();
        private NumericUpDown vendegszamNumeric = new NumericUpDown();
        private DataGridView talalatokGrid = new DataGridView();
        private Label eredmenyLabel = new Label();

        public SzabadSzobakForm(Hotel hotel)
        {
            this.hotel = hotel;

            Text = "Szabad szobák keresése";
            ClientSize = new Size(900, 560);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            GroupBox feltetelekGroup = new GroupBox();
            feltetelekGroup.Text = "Feltételek";
            feltetelekGroup.Location = new Point(12, 12);
            feltetelekGroup.Size = new Size(876, 80);

            feltetelekGroup.Controls.Add(UjCimke("Érkezés:", 15, 33));
            erkezesPicker.Location = new Point(85, 30);
            erkezesPicker.Size = new Size(130, 22);
            erkezesPicker.Format = DateTimePickerFormat.Custom;
            erkezesPicker.CustomFormat = "yyyy.MM.dd.";
            erkezesPicker.Value = DateTime.Today;
            feltetelekGroup.Controls.Add(erkezesPicker);

            feltetelekGroup.Controls.Add(UjCimke("Távozás:", 240, 33));
            tavozasPicker.Location = new Point(310, 30);
            tavozasPicker.Size = new Size(130, 22);
            tavozasPicker.Format = DateTimePickerFormat.Custom;
            tavozasPicker.CustomFormat = "yyyy.MM.dd.";
            tavozasPicker.Value = DateTime.Today.AddDays(1);
            feltetelekGroup.Controls.Add(tavozasPicker);

            feltetelekGroup.Controls.Add(UjCimke("Vendégek:", 465, 33));
            vendegszamNumeric.Location = new Point(540, 30);
            vendegszamNumeric.Size = new Size(70, 22);
            vendegszamNumeric.Minimum = 1;
            vendegszamNumeric.Maximum = 50;
            feltetelekGroup.Controls.Add(vendegszamNumeric);

            Button keresesGomb = new Button();
            keresesGomb.Text = "Keresés";
            keresesGomb.Location = new Point(640, 26);
            keresesGomb.Size = new Size(130, 30);
            keresesGomb.Click += KeresesGomb_Click;
            feltetelekGroup.Controls.Add(keresesGomb);

            Controls.Add(feltetelekGroup);

            talalatokGrid.Location = new Point(12, 105);
            talalatokGrid.Size = new Size(876, 410);
            talalatokGrid.ReadOnly = true;
            talalatokGrid.AllowUserToAddRows = false;
            talalatokGrid.AllowUserToDeleteRows = false;
            talalatokGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            talalatokGrid.RowHeadersVisible = false;
            talalatokGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            talalatokGrid.Columns.Add("szam", "Szobaszám");
            talalatokGrid.Columns.Add("tipus", "Típus");
            talalatokGrid.Columns.Add("ferohely", "Férőhely");
            talalatokGrid.Columns.Add("ar", "Ár/éj");
            talalatokGrid.Columns.Add("felszereltseg", "Felszereltség");
            talalatokGrid.Columns.Add("osszeg", "Ár a teljes időszakra");
            talalatokGrid.Columns["felszereltseg"].FillWeight = 250;
            Controls.Add(talalatokGrid);

            eredmenyLabel.Location = new Point(12, 525);
            eredmenyLabel.Size = new Size(600, 22);
            Controls.Add(eredmenyLabel);
        }

        private Label UjCimke(string szoveg, int x, int y)
        {
            Label cimke = new Label();
            cimke.Text = szoveg;
            cimke.Location = new Point(x, y);
            cimke.Size = new Size(70, 20);
            return cimke;
        }

        private void KeresesGomb_Click(object sender, EventArgs e)
        {
            DateTime erkezes = erkezesPicker.Value.Date;
            DateTime tavozas = tavozasPicker.Value.Date;
            int vendegszam = (int)vendegszamNumeric.Value;

            talalatokGrid.Rows.Clear();

            List<Room> szabadSzobak;
            try
            {
                szabadSzobak = hotel.FindAvailableRooms(erkezes, tavozas, vendegszam);
            }
            catch (Exception ex)
            {
                eredmenyLabel.Text = "";
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (Room szoba in szabadSzobak)
            {
                decimal osszeg = Booking.CalculatePrice(erkezes, tavozas, szoba.PricePerNight);

                talalatokGrid.Rows.Add(szoba.RoomNumber, szoba.RoomType, szoba.Capacity,
                    szoba.PricePerNight.ToString("N0") + " Ft", szoba.Equipment,
                    osszeg.ToString("N0") + " Ft");
            }

            int ejszakak = (tavozas - erkezes).Days;
            eredmenyLabel.Text = "Szabad szobák száma: " + szabadSzobak.Count +
                "   (" + ejszakak + " éjszaka, " + vendegszam + " fő)";

            if (szabadSzobak.Count == 0)
                MessageBox.Show("Nincs megfelelő szabad szoba ebben az időszakban.");
        }
    }
}

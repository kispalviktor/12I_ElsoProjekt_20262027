using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SzallodaAlkalmazas
{
    // Szobák kezelése: felvétel, listázás, keresés, módosítás, törlés
    public class SzobakForm : Form
    {
        private Hotel hotel;

        // A táblázatban kiválasztott szoba száma (0 = nincs kiválasztva)
        private int kivalasztottSzobaszam = 0;

        private DataGridView szobakGrid = new DataGridView();

        private RadioButton szamSzerintRadio = new RadioButton();
        private RadioButton tipusSzerintRadio = new RadioButton();
        private TextBox keresesTextBox = new TextBox();

        private NumericUpDown szobaszamNumeric = new NumericUpDown();
        private ComboBox tipusComboBox = new ComboBox();
        private NumericUpDown ferohelyNumeric = new NumericUpDown();
        private NumericUpDown arNumeric = new NumericUpDown();
        private TextBox felszereltsegTextBox = new TextBox();
        private CheckBox aktivCheckBox = new CheckBox();

        private Label darabLabel = new Label();

        public SzobakForm(Hotel hotel)
        {
            this.hotel = hotel;

            Text = "Szobák kezelése";
            ClientSize = new Size(960, 600);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ---- Táblázat ----
            szobakGrid.Location = new Point(12, 12);
            szobakGrid.Size = new Size(936, 300);
            szobakGrid.ReadOnly = true;
            szobakGrid.AllowUserToAddRows = false;
            szobakGrid.AllowUserToDeleteRows = false;
            szobakGrid.MultiSelect = false;
            szobakGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            szobakGrid.RowHeadersVisible = false;
            szobakGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            szobakGrid.Columns.Add("szam", "Szobaszám");
            szobakGrid.Columns.Add("tipus", "Típus");
            szobakGrid.Columns.Add("ferohely", "Férőhely");
            szobakGrid.Columns.Add("ar", "Ár/éj");
            szobakGrid.Columns.Add("felszereltseg", "Felszereltség");
            szobakGrid.Columns.Add("aktiv", "Aktív");
            szobakGrid.Columns["felszereltseg"].FillWeight = 250;

            szobakGrid.CellClick += SzobakGrid_CellClick;
            Controls.Add(szobakGrid);

            // ---- Keresés ----
            GroupBox keresesGroup = new GroupBox();
            keresesGroup.Text = "Keresés";
            keresesGroup.Location = new Point(12, 322);
            keresesGroup.Size = new Size(300, 160);

            szamSzerintRadio.Text = "Szobaszám szerint";
            szamSzerintRadio.Location = new Point(15, 25);
            szamSzerintRadio.Size = new Size(200, 22);
            szamSzerintRadio.Checked = true;
            keresesGroup.Controls.Add(szamSzerintRadio);

            tipusSzerintRadio.Text = "Típus szerint";
            tipusSzerintRadio.Location = new Point(15, 50);
            tipusSzerintRadio.Size = new Size(200, 22);
            keresesGroup.Controls.Add(tipusSzerintRadio);

            keresesTextBox.Location = new Point(15, 80);
            keresesTextBox.Size = new Size(270, 22);
            keresesGroup.Controls.Add(keresesTextBox);

            Button keresesGomb = new Button();
            keresesGomb.Text = "Keresés";
            keresesGomb.Location = new Point(15, 115);
            keresesGomb.Size = new Size(130, 30);
            keresesGomb.Click += KeresesGomb_Click;
            keresesGroup.Controls.Add(keresesGomb);

            Button osszesGomb = new Button();
            osszesGomb.Text = "Összes szoba";
            osszesGomb.Location = new Point(155, 115);
            osszesGomb.Size = new Size(130, 30);
            osszesGomb.Click += OsszesGomb_Click;
            keresesGroup.Controls.Add(osszesGomb);

            Controls.Add(keresesGroup);

            // ---- Szoba adatai ----
            GroupBox adatokGroup = new GroupBox();
            adatokGroup.Text = "Szoba adatai";
            adatokGroup.Location = new Point(324, 322);
            adatokGroup.Size = new Size(624, 230);

            adatokGroup.Controls.Add(UjCimke("Szobaszám:", 15, 28));
            szobaszamNumeric.Location = new Point(130, 25);
            szobaszamNumeric.Size = new Size(120, 22);
            szobaszamNumeric.Minimum = 1;
            szobaszamNumeric.Maximum = 9999;
            adatokGroup.Controls.Add(szobaszamNumeric);

            adatokGroup.Controls.Add(UjCimke("Típus:", 15, 58));
            tipusComboBox.Location = new Point(130, 55);
            tipusComboBox.Size = new Size(200, 22);
            tipusComboBox.Items.Add("Egyágyas");
            tipusComboBox.Items.Add("Kétágyas");
            tipusComboBox.Items.Add("Háromágyas");
            tipusComboBox.Items.Add("Családi");
            tipusComboBox.Items.Add("Lakosztály");
            adatokGroup.Controls.Add(tipusComboBox);

            adatokGroup.Controls.Add(UjCimke("Férőhely (fő):", 15, 88));
            ferohelyNumeric.Location = new Point(130, 85);
            ferohelyNumeric.Size = new Size(120, 22);
            ferohelyNumeric.Minimum = 1;
            ferohelyNumeric.Maximum = 50;
            adatokGroup.Controls.Add(ferohelyNumeric);

            adatokGroup.Controls.Add(UjCimke("Ár/éj (Ft):", 15, 118));
            arNumeric.Location = new Point(130, 115);
            arNumeric.Size = new Size(120, 22);
            arNumeric.Minimum = 0;
            arNumeric.Maximum = 10000000;
            arNumeric.Increment = 1000;
            arNumeric.ThousandsSeparator = true;
            adatokGroup.Controls.Add(arNumeric);

            adatokGroup.Controls.Add(UjCimke("Felszereltség:", 15, 148));
            felszereltsegTextBox.Location = new Point(130, 145);
            felszereltsegTextBox.Size = new Size(320, 22);
            adatokGroup.Controls.Add(felszereltsegTextBox);

            aktivCheckBox.Text = "Aktív (foglalható)";
            aktivCheckBox.Location = new Point(130, 178);
            aktivCheckBox.Size = new Size(200, 22);
            aktivCheckBox.Checked = true;
            adatokGroup.Controls.Add(aktivCheckBox);

            Button ujGomb = UjGomb("Új szoba felvétele", 470, 25);
            Button modositGomb = UjGomb("Módosítás mentése", 470, 62);
            Button torolGomb = UjGomb("Törlés", 470, 99);
            Button uritGomb = UjGomb("Mezők ürítése", 470, 136);

            ujGomb.Click += UjGomb_Click;
            modositGomb.Click += ModositGomb_Click;
            torolGomb.Click += TorolGomb_Click;
            uritGomb.Click += UritGomb_Click;

            adatokGroup.Controls.Add(ujGomb);
            adatokGroup.Controls.Add(modositGomb);
            adatokGroup.Controls.Add(torolGomb);
            adatokGroup.Controls.Add(uritGomb);

            Controls.Add(adatokGroup);

            darabLabel.Location = new Point(12, 565);
            darabLabel.Size = new Size(500, 22);
            Controls.Add(darabLabel);

            TablazatFeltoltese(hotel.Rooms);
            MezokUritese();
        }

        private Label UjCimke(string szoveg, int x, int y)
        {
            Label cimke = new Label();
            cimke.Text = szoveg;
            cimke.Location = new Point(x, y);
            cimke.Size = new Size(110, 20);
            return cimke;
        }

        private Button UjGomb(string szoveg, int x, int y)
        {
            Button gomb = new Button();
            gomb.Text = szoveg;
            gomb.Location = new Point(x, y);
            gomb.Size = new Size(140, 30);
            return gomb;
        }

        // A kapott szobák megjelenítése a táblázatban
        private void TablazatFeltoltese(List<Room> szobak)
        {
            szobakGrid.Rows.Clear();

            foreach (Room szoba in szobak)
            {
                string aktiv = "Nem";
                if (szoba.IsActive)
                    aktiv = "Igen";

                szobakGrid.Rows.Add(szoba.RoomNumber, szoba.RoomType, szoba.Capacity,
                    szoba.PricePerNight.ToString("N0") + " Ft", szoba.Equipment, aktiv);
            }

            darabLabel.Text = "Megjelenített szobák: " + szobak.Count + " / " + hotel.Rooms.Count;
        }

        private void MezokUritese()
        {
            kivalasztottSzobaszam = 0;
            szobaszamNumeric.Value = 1;
            tipusComboBox.Text = "";
            ferohelyNumeric.Value = 1;
            arNumeric.Value = 0;
            felszereltsegTextBox.Text = "";
            aktivCheckBox.Checked = true;
            szobakGrid.ClearSelection();
        }

        // A mezők tartalmából új Room objektum. Hibás adat esetén a Room konstruktora kivételt dob.
        private Room SzobaAMezokbol()
        {
            return new Room((int)szobaszamNumeric.Value, tipusComboBox.Text.Trim(),
                (int)ferohelyNumeric.Value, arNumeric.Value,
                felszereltsegTextBox.Text.Trim(), aktivCheckBox.Checked);
        }

        // ===================== ESEMÉNYEK =====================

        private void SzobakGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int szobaszam = (int)szobakGrid.Rows[e.RowIndex].Cells["szam"].Value;
            Room szoba = hotel.FindRoom(szobaszam);

            if (szoba == null)
                return;

            kivalasztottSzobaszam = szoba.RoomNumber;
            szobaszamNumeric.Value = szoba.RoomNumber;
            tipusComboBox.Text = szoba.RoomType;
            ferohelyNumeric.Value = szoba.Capacity;
            arNumeric.Value = szoba.PricePerNight;
            felszereltsegTextBox.Text = szoba.Equipment;
            aktivCheckBox.Checked = szoba.IsActive;
        }

        private void KeresesGomb_Click(object sender, EventArgs e)
        {
            string keresett = keresesTextBox.Text.Trim();
            List<Room> talalatok = new List<Room>();

            if (szamSzerintRadio.Checked)
            {
                int szam;
                if (!int.TryParse(keresett, out szam))
                {
                    MessageBox.Show("Adj meg egy számot!", "Hiba");
                    return;
                }

                Room szoba = hotel.FindRoom(szam);
                if (szoba != null)
                    talalatok.Add(szoba);
            }
            else
            {
                foreach (Room szoba in hotel.Rooms)
                {
                    if (szoba.RoomType.ToLower().Contains(keresett.ToLower()))
                        talalatok.Add(szoba);
                }
            }

            TablazatFeltoltese(talalatok);

            if (talalatok.Count == 0)
                MessageBox.Show("Nincs találat.");
        }

        private void OsszesGomb_Click(object sender, EventArgs e)
        {
            keresesTextBox.Text = "";
            TablazatFeltoltese(hotel.Rooms);
        }

        private void UjGomb_Click(object sender, EventArgs e)
        {
            try
            {
                Room szoba = SzobaAMezokbol();
                hotel.AddRoom(szoba);

                TablazatFeltoltese(hotel.Rooms);
                MezokUritese();
                MessageBox.Show("A szoba felvétele sikerült.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModositGomb_Click(object sender, EventArgs e)
        {
            if (kivalasztottSzobaszam == 0)
            {
                MessageBox.Show("Előbb válassz ki egy szobát a táblázatban!");
                return;
            }

            try
            {
                Room szoba = SzobaAMezokbol();
                hotel.UpdateRoom(kivalasztottSzobaszam, szoba);

                TablazatFeltoltese(hotel.Rooms);
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
            if (kivalasztottSzobaszam == 0)
            {
                MessageBox.Show("Előbb válassz ki egy szobát a táblázatban!");
                return;
            }

            DialogResult valasz = MessageBox.Show(
                "Biztosan törlöd a(z) " + kivalasztottSzobaszam + ". szobát?",
                "Törlés", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (valasz != DialogResult.Yes)
                return;

            try
            {
                hotel.DeleteRoom(kivalasztottSzobaszam);

                TablazatFeltoltese(hotel.Rooms);
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

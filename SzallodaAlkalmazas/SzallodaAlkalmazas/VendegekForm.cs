using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SzallodaAlkalmazas
{
    // Vendégek kezelése: rögzítés, keresés, listázás, módosítás, törlés
    public class VendegekForm : Form
    {
        private Hotel hotel;

        // A táblázatban kiválasztott vendég azonosítója (0 = nincs kiválasztva)
        private int kivalasztottId = 0;

        private DataGridView vendegekGrid = new DataGridView();
        private TextBox keresesTextBox = new TextBox();

        private Label azonositoErtekLabel = new Label();
        private TextBox nevTextBox = new TextBox();
        private TextBox telefonTextBox = new TextBox();
        private TextBox emailTextBox = new TextBox();

        private Label darabLabel = new Label();

        public VendegekForm(Hotel hotel)
        {
            this.hotel = hotel;

            Text = "Vendégek kezelése";
            ClientSize = new Size(900, 580);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ---- Táblázat ----
            vendegekGrid.Location = new Point(12, 12);
            vendegekGrid.Size = new Size(876, 300);
            vendegekGrid.ReadOnly = true;
            vendegekGrid.AllowUserToAddRows = false;
            vendegekGrid.AllowUserToDeleteRows = false;
            vendegekGrid.MultiSelect = false;
            vendegekGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            vendegekGrid.RowHeadersVisible = false;
            vendegekGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            vendegekGrid.Columns.Add("id", "Azonosító");
            vendegekGrid.Columns.Add("nev", "Név");
            vendegekGrid.Columns.Add("telefon", "Telefonszám");
            vendegekGrid.Columns.Add("email", "E-mail");
            vendegekGrid.Columns["id"].FillWeight = 50;

            vendegekGrid.CellClick += VendegekGrid_CellClick;
            Controls.Add(vendegekGrid);

            // ---- Keresés ----
            GroupBox keresesGroup = new GroupBox();
            keresesGroup.Text = "Keresés (név, telefon vagy e-mail)";
            keresesGroup.Location = new Point(12, 322);
            keresesGroup.Size = new Size(300, 120);

            keresesTextBox.Location = new Point(15, 30);
            keresesTextBox.Size = new Size(270, 22);
            keresesGroup.Controls.Add(keresesTextBox);

            Button keresesGomb = new Button();
            keresesGomb.Text = "Keresés";
            keresesGomb.Location = new Point(15, 70);
            keresesGomb.Size = new Size(130, 30);
            keresesGomb.Click += KeresesGomb_Click;
            keresesGroup.Controls.Add(keresesGomb);

            Button osszesGomb = new Button();
            osszesGomb.Text = "Összes vendég";
            osszesGomb.Location = new Point(155, 70);
            osszesGomb.Size = new Size(130, 30);
            osszesGomb.Click += OsszesGomb_Click;
            keresesGroup.Controls.Add(osszesGomb);

            Controls.Add(keresesGroup);

            // ---- Vendég adatai ----
            GroupBox adatokGroup = new GroupBox();
            adatokGroup.Text = "Vendég adatai";
            adatokGroup.Location = new Point(324, 322);
            adatokGroup.Size = new Size(564, 200);

            adatokGroup.Controls.Add(UjCimke("Azonosító:", 15, 28));
            azonositoErtekLabel.Location = new Point(120, 28);
            azonositoErtekLabel.Size = new Size(200, 20);
            adatokGroup.Controls.Add(azonositoErtekLabel);

            adatokGroup.Controls.Add(UjCimke("Név:", 15, 58));
            nevTextBox.Location = new Point(120, 55);
            nevTextBox.Size = new Size(260, 22);
            adatokGroup.Controls.Add(nevTextBox);

            adatokGroup.Controls.Add(UjCimke("Telefonszám:", 15, 88));
            telefonTextBox.Location = new Point(120, 85);
            telefonTextBox.Size = new Size(260, 22);
            adatokGroup.Controls.Add(telefonTextBox);

            adatokGroup.Controls.Add(UjCimke("E-mail:", 15, 118));
            emailTextBox.Location = new Point(120, 115);
            emailTextBox.Size = new Size(260, 22);
            adatokGroup.Controls.Add(emailTextBox);

            Button ujGomb = UjGomb("Új vendég mentése", 400, 25);
            Button modositGomb = UjGomb("Módosítás mentése", 400, 62);
            Button torolGomb = UjGomb("Törlés", 400, 99);
            Button megseGomb = UjGomb("Mégse", 400, 136);

            ujGomb.Click += UjGomb_Click;
            modositGomb.Click += ModositGomb_Click;
            torolGomb.Click += TorolGomb_Click;
            megseGomb.Click += MegseGomb_Click;

            adatokGroup.Controls.Add(ujGomb);
            adatokGroup.Controls.Add(modositGomb);
            adatokGroup.Controls.Add(torolGomb);
            adatokGroup.Controls.Add(megseGomb);

            Controls.Add(adatokGroup);

            darabLabel.Location = new Point(12, 545);
            darabLabel.Size = new Size(500, 22);
            Controls.Add(darabLabel);

            TablazatFeltoltese(hotel.Guests);
            MezokUritese();
        }

        private Label UjCimke(string szoveg, int x, int y)
        {
            Label cimke = new Label();
            cimke.Text = szoveg;
            cimke.Location = new Point(x, y);
            cimke.Size = new Size(100, 20);
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

        private void TablazatFeltoltese(List<Guest> vendegek)
        {
            vendegekGrid.Rows.Clear();

            foreach (Guest vendeg in vendegek)
            {
                vendegekGrid.Rows.Add(vendeg.Id, vendeg.Name, vendeg.Phone, vendeg.Email);
            }

            darabLabel.Text = "Megjelenített vendégek: " + vendegek.Count + " / " + hotel.Guests.Count;
        }

        private void MezokUritese()
        {
            kivalasztottId = 0;
            azonositoErtekLabel.Text = "(új vendég: " + hotel.GetNextGuestId() + ")";
            nevTextBox.Text = "";
            telefonTextBox.Text = "";
            emailTextBox.Text = "";
            vendegekGrid.ClearSelection();
        }

        // ===================== ESEMÉNYEK =====================

        private void VendegekGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            int id = (int)vendegekGrid.Rows[e.RowIndex].Cells["id"].Value;
            Guest vendeg = hotel.FindGuest(id);

            if (vendeg == null)
                return;

            kivalasztottId = vendeg.Id;
            azonositoErtekLabel.Text = vendeg.Id.ToString();
            nevTextBox.Text = vendeg.Name;
            telefonTextBox.Text = vendeg.Phone;
            emailTextBox.Text = vendeg.Email;
        }

        private void KeresesGomb_Click(object sender, EventArgs e)
        {
            string keresett = keresesTextBox.Text.Trim().ToLower();
            List<Guest> talalatok = new List<Guest>();

            foreach (Guest vendeg in hotel.Guests)
            {
                if (vendeg.Name.ToLower().Contains(keresett) ||
                    vendeg.Phone.ToLower().Contains(keresett) ||
                    vendeg.Email.ToLower().Contains(keresett))
                {
                    talalatok.Add(vendeg);
                }
            }

            TablazatFeltoltese(talalatok);

            if (talalatok.Count == 0)
                MessageBox.Show("Nincs találat.");
        }

        private void OsszesGomb_Click(object sender, EventArgs e)
        {
            keresesTextBox.Text = "";
            TablazatFeltoltese(hotel.Guests);
        }

        private void UjGomb_Click(object sender, EventArgs e)
        {
            try
            {
                Guest vendeg = new Guest(hotel.GetNextGuestId(), nevTextBox.Text.Trim(),
                    telefonTextBox.Text.Trim(), emailTextBox.Text.Trim());
                hotel.AddGuest(vendeg);

                TablazatFeltoltese(hotel.Guests);
                MezokUritese();
                MessageBox.Show("A vendég rögzítése sikerült.");
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
                MessageBox.Show("Előbb válassz ki egy vendéget a táblázatban!");
                return;
            }

            try
            {
                Guest vendeg = new Guest(kivalasztottId, nevTextBox.Text.Trim(),
                    telefonTextBox.Text.Trim(), emailTextBox.Text.Trim());
                hotel.UpdateGuest(vendeg);

                TablazatFeltoltese(hotel.Guests);
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
                MessageBox.Show("Előbb válassz ki egy vendéget a táblázatban!");
                return;
            }

            DialogResult valasz = MessageBox.Show(
                "Biztosan törlöd ezt a vendéget: " + nevTextBox.Text + "?",
                "Törlés", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (valasz != DialogResult.Yes)
                return;

            try
            {
                hotel.DeleteGuest(kivalasztottId);

                TablazatFeltoltese(hotel.Guests);
                MezokUritese();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MegseGomb_Click(object sender, EventArgs e)
        {
            MezokUritese();
        }
    }
}

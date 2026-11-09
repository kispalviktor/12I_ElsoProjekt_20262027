using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SzallodaAlkalmazas
{
    // Főablak: innen lehet megnyitni a többi ablakot.
    // Induláskor betölti az adatokat, bezáráskor elmenti őket.
    public partial class Form1 : Form
    {
        private Hotel hotel = new Hotel();
        private FileManager fileManager;

        private Label osszesitesLabel = new Label();
        private TextBox naploTextBox = new TextBox();

        public Form1()
        {
            InitializeComponent();

            Text = "Szállodai foglaláskezelő";
            ClientSize = new Size(820, 520);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ---- Felső rész: cím és menügombok ----
            Panel menuPanel = new Panel();
            menuPanel.Location = new Point(0, 0);
            menuPanel.Size = new Size(820, 170);
            menuPanel.BackColor = Color.LightSteelBlue;

            Label cimLabel = new Label();
            cimLabel.Text = "SZÁLLODAI FOGLALÁSKEZELŐ";
            cimLabel.Font = new Font("Arial", 18, FontStyle.Bold);
            cimLabel.Location = new Point(20, 15);
            cimLabel.Size = new Size(600, 35);
            menuPanel.Controls.Add(cimLabel);

            Button szobakGomb = MenuGomb("Szobák", 20, 65);
            Button vendegekGomb = MenuGomb("Vendégek", 220, 65);
            Button foglalasokGomb = MenuGomb("Foglalások", 420, 65);
            Button szabadSzobakGomb = MenuGomb("Szabad szobák", 20, 115);
            Button mentesGomb = MenuGomb("Adatok mentése", 220, 115);

            szobakGomb.Click += SzobakGomb_Click;
            vendegekGomb.Click += VendegekGomb_Click;
            foglalasokGomb.Click += FoglalasokGomb_Click;
            szabadSzobakGomb.Click += SzabadSzobakGomb_Click;
            mentesGomb.Click += MentesGomb_Click;

            menuPanel.Controls.Add(szobakGomb);
            menuPanel.Controls.Add(vendegekGomb);
            menuPanel.Controls.Add(foglalasokGomb);
            menuPanel.Controls.Add(szabadSzobakGomb);
            menuPanel.Controls.Add(mentesGomb);

            // ---- Alsó rész: aktuális tartalom ----
            Panel tartalomPanel = new Panel();
            tartalomPanel.Location = new Point(0, 170);
            tartalomPanel.Size = new Size(820, 350);

            osszesitesLabel.Location = new Point(20, 15);
            osszesitesLabel.Size = new Size(780, 50);
            osszesitesLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            tartalomPanel.Controls.Add(osszesitesLabel);

            Label naploLabel = new Label();
            naploLabel.Text = "Adatfájlok állapota:";
            naploLabel.Location = new Point(20, 70);
            naploLabel.Size = new Size(300, 20);
            tartalomPanel.Controls.Add(naploLabel);

            naploTextBox.Location = new Point(20, 92);
            naploTextBox.Size = new Size(780, 240);
            naploTextBox.Multiline = true;
            naploTextBox.ReadOnly = true;
            naploTextBox.ScrollBars = ScrollBars.Vertical;
            naploTextBox.Font = new Font("Consolas", 9);
            tartalomPanel.Controls.Add(naploTextBox);

            Controls.Add(menuPanel);
            Controls.Add(tartalomPanel);

            Load += Form1_Load;
            FormClosing += Form1_FormClosing;
        }

        // Egyforma menügombok készítése
        private Button MenuGomb(string szoveg, int x, int y)
        {
            Button gomb = new Button();
            gomb.Text = szoveg;
            gomb.Location = new Point(x, y);
            gomb.Size = new Size(180, 40);
            gomb.Font = new Font("Arial", 10);
            gomb.BackColor = Color.White;
            return gomb;
        }

        // ===================== BETÖLTÉS INDULÁSKOR =====================

        private void Form1_Load(object sender, EventArgs e)
        {
            // Az adatfájlok az exe mellett, az "Adatok" mappában vannak
            string adatMappa = Path.Combine(Application.StartupPath, "Adatok");

            List<string> uzenetek;

            try
            {
                fileManager = new FileManager(adatMappa);
                uzenetek = fileManager.LoadAllData(hotel);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Az adatok betöltése nem sikerült:\n" + ex.Message,
                    "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                naploTextBox.Text = "Hiba a betöltéskor: " + ex.Message;
                OsszesitesFrissitese();
                return;
            }

            naploTextBox.Text = "Adatmappa: " + adatMappa + Environment.NewLine;

            if (uzenetek.Count == 0)
            {
                naploTextBox.Text += "Minden adat hiba nélkül betöltődött.";
            }
            else
            {
                foreach (string uzenet in uzenetek)
                    naploTextBox.Text += uzenet + Environment.NewLine;

                MessageBox.Show("Az adatok betöltésekor " + uzenetek.Count +
                    " probléma történt. A részletek a főablakban láthatók.",
                    "Figyelem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            OsszesitesFrissitese();
        }

        private void OsszesitesFrissitese()
        {
            osszesitesLabel.Text = "Szobák: " + hotel.Rooms.Count +
                "     Vendégek: " + hotel.Guests.Count +
                "     Foglalások: " + hotel.Bookings.Count;
        }

        // ===================== MENTÉS =====================

        private bool AdatokMentese()
        {
            if (fileManager == null)
                return false;

            try
            {
                fileManager.SaveAllData(hotel);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Az adatok mentése nem sikerült:\n" + ex.Message,
                    "Hiba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void MentesGomb_Click(object sender, EventArgs e)
        {
            if (AdatokMentese())
            {
                naploTextBox.Text += Environment.NewLine + DateTime.Now.ToString("HH:mm:ss") +
                    " - Az adatok mentése sikerült.";
                MessageBox.Show("Az adatok mentése sikerült.");
            }
        }

        // Bezáráskor mentünk. Ha nem sikerül, megkérdezzük, hogy biztosan bezárja-e.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AdatokMentese())
            {
                DialogResult valasz = MessageBox.Show(
                    "Az adatok nem lettek elmentve. Biztosan bezárod a programot?",
                    "Bezárás", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (valasz == DialogResult.No)
                    e.Cancel = true;
            }
        }

        // ===================== ABLAKOK MEGNYITÁSA =====================

        private void SzobakGomb_Click(object sender, EventArgs e)
        {
            SzobakForm ablak = new SzobakForm(hotel);
            ablak.ShowDialog();
            OsszesitesFrissitese();
        }

        private void VendegekGomb_Click(object sender, EventArgs e)
        {
            VendegekForm ablak = new VendegekForm(hotel);
            ablak.ShowDialog();
            OsszesitesFrissitese();
        }

        private void FoglalasokGomb_Click(object sender, EventArgs e)
        {
            FoglalasokForm ablak = new FoglalasokForm(hotel);
            ablak.ShowDialog();
            OsszesitesFrissitese();
        }

        private void SzabadSzobakGomb_Click(object sender, EventArgs e)
        {
            SzabadSzobakForm ablak = new SzabadSzobakForm(hotel);
            ablak.ShowDialog();
            OsszesitesFrissitese();
        }
    }
}

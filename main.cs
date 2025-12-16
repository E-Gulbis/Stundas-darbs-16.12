using System;
using System.IO;
using System.Windows.Forms;

namespace PCComponentManager{
    public class PCComponent{
        public string Veids { get; set; }
        public string Modelis { get; set; }
        public decimal Cena { get; set; }

        public override string ToString(){
            return $"{Veids} - {Modelis} ({Cena:0.00} EUR)";
        }

        public string ToFileFormat(){
            return "-Personālā datora sastāvdaļa-\n" +
                   $"Veids: {Veids}\n" +
                   $"Modelis: {Modelis}\n" +
                   $"Cena: {Cena:0.00} EUR\n";
        }
    }

    static class Program{
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form{
        private ListBox listBox;
        private TextBox txtVeids;
        private TextBox txtModelis;
        private TextBox txtCena;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnSave;

        public MainForm(){
            Text = "Personālā datora sastāvdaļas";
            Width = 520;
            Height = 360;

            listBox = new ListBox { Left = 10, Top = 10, Width = 230, Height = 280 };
            listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

            Label lblVeids = new Label { Text = "Veids", Left = 260, Top = 20 };
            Label lblModelis = new Label { Text = "Modelis", Left = 260, Top = 70 };
            Label lblCena = new Label { Text = "Cena (EUR)", Left = 260, Top = 120 };

            txtVeids = new TextBox { Left = 260, Top = 40, Width = 220 };
            txtModelis = new TextBox { Left = 260, Top = 90, Width = 220 };
            txtCena = new TextBox { Left = 260, Top = 140, Width = 220 };

            btnAdd = new Button { Text = "Pievienot", Left = 260, Top = 180, Width = 100 };
            btnUpdate = new Button { Text = "Labot", Left = 380, Top = 180, Width = 100 };
            btnSave = new Button { Text = "Saglabāt failā", Left = 260, Top = 220, Width = 220 };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnSave.Click += BtnSave_Click;

            Controls.AddRange(new Control[]{
                listBox, lblVeids, lblModelis, lblCena,
                txtVeids, txtModelis, txtCena,
                btnAdd, btnUpdate, btnSave
            });

            // Example data from task table
            listBox.Items.Add(new PCComponent { Veids = "RAM", Modelis = "Corsair Vengeance LPX 16GB", Cena = 99.99m });
            listBox.Items.Add(new PCComponent { Veids = "GPU", Modelis = "Gigabyte GeForce GT 710 2GB", Cena = 75.50m });
            listBox.Items.Add(new PCComponent { Veids = "CPU", Modelis = "AMD Ryzen 7 5800X 3,8GHz", Cena = 657.80m });
        }

        private void BtnAdd_Click(object sender, EventArgs e){
            if (!ValidateInput()) return;

            PCComponent component = new PCComponent
            {
                Veids = txtVeids.Text,
                Modelis = txtModelis.Text,
                Cena = decimal.Parse(txtCena.Text)
            };

            listBox.Items.Add(component);
            ClearFields();
        }

        private void BtnUpdate_Click(object sender, EventArgs e){
            if (listBox.SelectedItem == null || !ValidateInput()) return;

            PCComponent component = (PCComponent)listBox.SelectedItem;
            component.Veids = txtVeids.Text;
            component.Modelis = txtModelis.Text;
            component.Cena = decimal.Parse(txtCena.Text);

            int index = listBox.SelectedIndex;
            listBox.Items[index] = component; // refresh
        }

        private void BtnSave_Click(object sender, EventArgs e){
            if (listBox.SelectedItem == null) return;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                PCComponent component = (PCComponent)listBox.SelectedItem;
                File.WriteAllText(dialog.FileName, component.ToFileFormat());
                MessageBox.Show("Dati saglabāti!", "OK");
            }
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e){
            if (listBox.SelectedItem == null) return;

            PCComponent component = (PCComponent)listBox.SelectedItem;
            txtVeids.Text = component.Veids;
            txtModelis.Text = component.Modelis;
            txtCena.Text = component.Cena.ToString("0.00");
        }

        private bool ValidateInput(){
            if (string.IsNullOrWhiteSpace(txtVeids.Text) ||
                string.IsNullOrWhiteSpace(txtModelis.Text) ||
                string.IsNullOrWhiteSpace(txtCena.Text))
            {
                MessageBox.Show("Jāaizpilda visi lauki!");
                return false;
            }

            if (!decimal.TryParse(txtCena.Text, out _))

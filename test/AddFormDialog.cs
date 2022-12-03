using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class AddFormDialog : Form
    {
        DataBase db = new DataBase();
        public string Name_
        {
            get
            {
                return this.NameTextBox.Text.Trim();
            }
        }
        public int Color
        {
            get;set;
        }
        public string Lat
        {
            get
            {
                return this.LatTextBox.Text.Trim();
            }
        }
        public string Lng
        {
            get
            {
                return this.LngTextBox.Text.Trim();
            }
        }
        public AddFormDialog()
        {
            InitializeComponent();
        }
        private bool checkRepeatName()
        {
            var search = $"SELECT * FROM pointDB1 WHERE name_='{Name_}'";
            SqlCommand command = new SqlCommand(search, db.Sql);
            db.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            bool b=false;
            while (reader.Read())
            {
                b = true;
            }
            reader.Close();
            db.CloseConnection();
            return b;
        }
        private void OkButton_Click(object sender, EventArgs e)
        {
            if(checkRepeatName())
            {
                MessageBox.Show("Имя уже существует!");
                return;
            }

            if(!string.IsNullOrEmpty(Name_)&&!string.IsNullOrEmpty(Lat)&&!string.IsNullOrEmpty(Lng))
            {
                try
                {
                    string lat = Lat.Replace(".", ",");
                    string lng = Lng.Replace(".", ",");
                    if(!(Double.Parse(lat)>=-90&& Double.Parse(lat)<=90&& Double.Parse(lng)>=-180&& Double.Parse(lng)<=180))
                    {
                        MessageBox.Show("Значения широты находятся в диапозоне от -90 до 90 градусов, значения долготы в диапозоне от -180 до 180 градусов");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Значения широты или долготы введены неверно!");
                    return;
                }
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Все поля должны быть заполнены!");
                DialogResult = DialogResult.Cancel;
            }
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (greenRadioButton.Checked)
                Color = 0;
            else if (redRadioButton.Checked)
                Color = 1;
            else
                Color = 2;
            
        }
    }
}

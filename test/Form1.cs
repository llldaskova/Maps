using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
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
    
    public partial class Form1 : Form
    {
        DataBase db = new DataBase();
        int selectedRow;
        private void createColumn()
        {
            dataGridView1.Columns.Add("id", "id");
            dataGridView1.Columns["id"].Visible = false;
            dataGridView1.Columns.Add("name_", "Название");
            dataGridView1.Columns.Add("Lat", "Широта");
            dataGridView1.Columns.Add("Lng", "Долгота");
        }
        private void readSingleRow(DataGridView dataGrid, IDataRecord record)
        {
            dataGrid.Rows.Add( record.GetInt32(0),record.GetString(1), record.GetDouble(2),record.GetDouble(3));

        }
      
        private void RefreshDataGrid(DataGridView dataGrid)
        {

            dataGrid.Rows.Clear();
            string queryString = $"select * from pointDB1";
            SqlCommand command = new SqlCommand(queryString, db.Sql);
            db.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                readSingleRow(dataGrid,reader);
                

            }
            reader.Close();
            db.CloseConnection();
        }
        private GMarkerGoogle createSingleMark(string name, double lat, double lng, GMarkerGoogleType color= GMarkerGoogleType.red)
        {

            GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(lat, lng), color);
            marker.ToolTip = new GMap.NET.WindowsForms.ToolTips.GMapRoundedToolTip(marker);

            marker.ToolTipText = name;
            return marker;
        }
        GMapOverlay markersOverlay;
        private void createMark()
        {
            markersOverlay = new GMapOverlay("markers");
            string queryString = $"select * from pointDB1";
            SqlCommand command = new SqlCommand(queryString, db.Sql);
            db.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {

                markersOverlay.Markers.Add(createSingleMark(reader.GetString(1), reader.GetDouble(2), reader.GetDouble(3)));
            }
            reader.Close();
            db.CloseConnection();
            gMapControl1.Overlays.Add(markersOverlay);
        }
        private void addMark(string name,double lat,double lng, GMarkerGoogleType color= GMarkerGoogleType.red)
        {
            markersOverlay.Markers.Add(createSingleMark(name, lat, lng,color));
            gMapControl1.Overlays.Add(markersOverlay);
        }
        private void deleteMark()
        {
            markersOverlay.Markers.RemoveAt(selectedRow);
            gMapControl1.Overlays.Add(markersOverlay);
        }
        public Form1()
        {
            InitializeComponent();
            LatLabel.Text = " ";
            LngLabel.Text = " ";
           
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl1.MinZoom = 2;
            gMapControl1.MaxZoom = 16;
            gMapControl1.Zoom = 4;
            gMapControl1.Position = new GMap.NET.PointLatLng(54.97670659983604, 82.92601440697915);// точка в центре карты при открытии (центр России)
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            gMapControl1.CanDragMap = true;
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.ShowCenter = false;
            gMapControl1.ShowTileGridLines = false;
            createMark();
        }
      

       

       

        private void Form1_Load(object sender, EventArgs e)
        {
            createColumn();
            RefreshDataGrid(dataGridView1);
        }
        private GMarkerGoogleType color(int c)
        {
            switch(c)
            {
                case 0:
                    return GMarkerGoogleType.green;
                case 1:
                    return GMarkerGoogleType.red;
                default:
                    return GMarkerGoogleType.blue;
            }
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            AddFormDialog dlg = new AddFormDialog();
            DialogResult dialogResult = dlg.ShowDialog();
            if(dialogResult==DialogResult.OK)
            {
                string strlat = dlg.Lat.Replace(",", ".");
                string strlng = dlg.Lng.Replace(",", ".");
                //decimal lat = Decimal.Parse(strlat);
                //decimal lng = Decimal.Parse(strlng);
                double dlat = Double.Parse(dlg.Lat.Replace(".", ","));
                double dlng = Double.Parse(dlg.Lng.Replace(".", ","));
                string add = $"INSERT INTO pointDB1 (name_, Lat, Lng)VALUES('{dlg.Name_}', '{strlat}', '{strlng}')";
                var command = new SqlCommand(add, db.Sql);
                db.OpenConnection();
                command.ExecuteNonQuery();
                db.CloseConnection();
                RefreshDataGrid(dataGridView1);
                addMark(dlg.Name_, dlat, dlng, color(dlg.Color));
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;
        }
        private void deleteRow()
        {
            var id = Convert.ToInt32(dataGridView1.Rows[selectedRow].Cells[0].Value);
            string delete = $"delete from pointDB1 where id={id}";
            SqlCommand command = new SqlCommand(delete, db.Sql);
            db.OpenConnection();
            command.ExecuteNonQuery();
            db.CloseConnection();
            RefreshDataGrid(dataGridView1);
            //label1.Text = id.ToString();

        }
        private void searh(string name)
        { 
            var search = $"SELECT * FROM pointDB1 WHERE name_='{name}'";
            SqlCommand command = new SqlCommand(search, db.Sql);
            db.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            string lat="",lng="";
            int id = 0;
            while (reader.Read())
            {
                id = reader.GetInt32(0);
                lat = selectedMarker.Position.Lat.ToString().Replace(",", ".");
                lng = selectedMarker.Position.Lng.ToString().Replace(",", ".");
                

            }
            reader.Close();
            string update = $"update pointDB1 set Lat='{lat}',Lng='{lng}' where id={id}";
            SqlCommand command2 = new SqlCommand(update, db.Sql);
            command2.ExecuteNonQuery();
            db.CloseConnection();
            RefreshDataGrid(dataGridView1);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            deleteRow();
            deleteMark();
        }
        private GMapMarker selectedMarker=null;
        private bool menuFlag = false;
        private void gMapControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (selectedMarker is null||menuFlag)
                    return;

                var nameMarker = selectedMarker.ToolTipText;

                //переводим координаты курсора мыши в долготу и широту на карте
                var latlng = gMapControl1.FromLocalToLatLng(e.X, e.Y);

                //присваиваем новую позицию для маркера
                selectedMarker.Position = latlng;

                searh(nameMarker);

                selectedMarker = null;
            }
        }

        private void gMapControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!(selectedMarker == null))
                    menuFlag = false;
                selectedMarker = gMapControl1.Overlays
            .SelectMany(o => o.Markers)
            .FirstOrDefault(m => m.IsMouseOver == true);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int i = e.RowIndex;
            double lat= (double)dataGridView1[2,i].Value;
            double lng = (double)dataGridView1[3, i].Value;
            gMapControl1.Position = new GMap.NET.PointLatLng(lat, lng);
        }
        private GMapMarker selectedMenuMarker;
        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                menuFlag = true;
                ContextMenuStrip MenuStrip = new ContextMenuStrip();
                selectedMenuMarker = gMapControl1.Overlays
                .SelectMany(o => o.Markers)
            .FirstOrDefault(m => m.IsMouseOver == true);
                /* MenuStrip.ItemClicked += menu_Click;
                 MenuStrip.Items.Add("111");
                 MenuStrip.Items.Add("222");*/
               // ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Копировать");
                MenuStrip.Items.Add(selectedMenuMarker.ToolTipText);
                MenuStrip.Items[0].Enabled = false;
                ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Удалить");
                MenuStrip.Items.Add(deleteMenuItem);
                deleteMenuItem.Click += DeleteMenuItem_Click;
                MenuStrip.Show(gMapControl1, e.Location);
                //MenuStrip.Items.Add("jkjk");
            }
        }
        private void deleteBDFromMenu(GMapMarker selectedMenuMarker)
        {
            var search = $"SELECT * FROM pointDB1 WHERE name_='{selectedMenuMarker.ToolTipText}'";
            SqlCommand command = new SqlCommand(search, db.Sql);
            db.OpenConnection();
            SqlDataReader reader = command.ExecuteReader();
            int id=-1;
            while (reader.Read())
            {
                id = reader.GetInt32(0);
                break;
            }
            reader.Close();
            string delete = $"delete from pointDB1 where id={id}";
            SqlCommand command2 = new SqlCommand(delete, db.Sql);
            command2.ExecuteNonQuery();
            db.CloseConnection();


        }
        private void deleteMarkFromMenu(GMapMarker selectedMenuMarker)
        {

            markersOverlay.Markers.Remove(selectedMenuMarker);
            deleteBDFromMenu(selectedMenuMarker);
            gMapControl1.Overlays.Add(markersOverlay);
        }
        private void DeleteMenuItem_Click(object sender, EventArgs e)
        {
            deleteMarkFromMenu(selectedMenuMarker);
            RefreshDataGrid(dataGridView1);
        }

        private void gMapControl1_MouseMove(object sender, MouseEventArgs e)
        {
            LatLabel.Text = gMapControl1.FromLocalToLatLng(e.X,e.Y).Lat.ToString();
            LngLabel.Text = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng.ToString();
        }
    }
}

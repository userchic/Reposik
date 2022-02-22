using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.SOAP;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PointLib;

namespace FormsApp
{
    public partial class Form1 : Form
    {
        private Point[] points = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();

            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();

            listBox.DataSource = points;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            if (points == null)
                return;

            Array.Sort(points);

            listBox.DataSource = null;
            listBox.DataSource = points;
        }
        private void btnSerialize_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        bf.Serialize(fs, points);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        sf.Serialize(fs, points);
                        break;
                }
            }
        }

        private void btnDeserialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        points = (Point[])bf.Deserialize(fs);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        points = (Point[])sf.Deserialize(fs);
                        break;
                }
            }

            listBox.DataSource = null;
            listBox.DataSource = points;
        }

    }
}

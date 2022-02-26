using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PointLib;
using System.Xml.Serialization;
using Newtonsoft.Json;

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
                    case ".xml":
                        XmlRootAttribute rt = new XmlRootAttribute("Points");
                        XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                        var xf = new XmlSerializer(typeof(Point[]), overrides, new[] { typeof(Point3D) }, rt, "Points");
                        xf.Serialize(fs, points);
                        break;
                    case ".json":
                        var jf = new JsonSerializer();
                        jf.TypeNameHandling = TypeNameHandling.All;
                        using (var w = new StreamWriter(fs))
                            jf.Serialize(w, points);
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
                new FileStream(dlg.FileName, FileMode.OpenOrCreate))
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
                    case ".xml":
                        XmlRootAttribute rt = new XmlRootAttribute("Points");
                        XmlAttributeOverrides overrides = new XmlAttributeOverrides();
                        var xf = new XmlSerializer(typeof(Point[]),overrides, new[] { typeof(Point3D) },rt, "Points");
                        points = xf.Deserialize(fs) as Point[];

                        break;
                    case ".json":
                        var jf = new JsonSerializer();
                        jf.TypeNameHandling = TypeNameHandling.All;
                        using (var r = new StreamReader(fs))
                            points = (Point[])jf.Deserialize(r, typeof(Point[]));
                        break;
                }
            }

            listBox.DataSource = null;
            listBox.DataSource = points;
        }

    }
}

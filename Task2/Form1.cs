using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Task2.Properties;

namespace Task2
{
    public partial class Form1 : Form
    {
        private readonly List<Feeds> _feeds = new List<Feeds>();
        private SettingObject _setting;

        public Form1(string path)
        {
            var file = new System.IO.StreamReader(path);
            var serializer = new XmlSerializer(typeof(SettingObject));
            _setting = (SettingObject)serializer.Deserialize(file);
            file.Close();
            InitializeComponent();
            timer1.Interval = _setting.Frequency;
            timer1.Start();
            PopulateRssFeed();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var feed = _feeds.Find(f => f.ToString() == listBox1.SelectedItem?.ToString());
            textBox1.Text = checkBox1.Checked ? feed?.ReworkedDescription() : feed?.Description;
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var feed = _feeds.Find(f => f.ToString() == listBox1.SelectedItem.ToString());
            System.Diagnostics.Process.Start(feed.Link);
        }

        private void timer1_Elapsed(object sender, EventArgs e)
        {
            _feeds.Clear();
            PopulateRssFeed();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var feed = _feeds.Find(f => f.Description == textBox1.Text || f.ReworkedDescription() == textBox1.Text);
            textBox1.Text = checkBox1.Checked ? feed?.ReworkedDescription() : feed?.Description;
        }

        private void PopulateRssFeed()
        {
            var client = new WebClient()
            {
                
                Proxy = new WebProxy(string.IsNullOrEmpty(_setting.Address)? null : _setting.Address, true)
                {
                    Credentials = new NetworkCredential(string.IsNullOrEmpty(_setting.Login)? null : _setting.Login, string.IsNullOrEmpty(_setting.Password)? null : _setting.Password)
                }
            } ;
            var xDoc = XDocument.Load(
                XmlReader.Create(client.OpenRead(_setting.Link) ?? throw new InvalidOperationException()));
            var items = from x in xDoc.Descendants("item")
                select new
                {
                    title = x.Element("title")?.Value,
                    link = x.Element("link")?.Value,
                    pubDate = x.Element("pubDate")?.Value,
                    description = x.Element("description")?.Value
                };
            _feeds.AddRange(items.Select(i => new Feeds
                { Title = i.title, Link = i.link, PubDate = i.pubDate, Description = i.description }));
            listBox1.Items.Clear();
            listBox1.Items.AddRange(_feeds.ToArray());
        }
    }
}
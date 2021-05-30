using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Globalization;
using System.Media;

namespace CoinMonitor
{
    public partial class Form1 : Form
    {        
        public Form1()
        {
            InitializeComponent();
        }

//==========================================================================================//

        private void Form1_Load(object sender, EventArgs e)
        {
            //check internet connection
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        lblConnection.Text = "Connected";
                    }
                }
            }
            catch
            {
                lblConnection.Text = "Not Connected";
            }

            comboBox1.Text = "USD";

            Run_API();

            lblLastCheck.Text = DateTime.Now.ToShortTimeString();

            timer1.Start();

        }

//==========================================================================================//

        public void NotificationRing()
        {
            SoundPlayer simpleSound = new SoundPlayer(@"c:\Windows\Media\tada.wav");
            simpleSound.Play();
        }

//==========================================================================================//

        public class Time
        {
            public string updated { get; set; }
            public DateTime updatedISO { get; set; }
            public string updateduk { get; set; }
        }
        public class USD
        {
            public string code { get; set; }
            public string rate { get; set; }
            public string description { get; set; }
            public double rate_float { get; set; }
        }
        public class PHP
        {
            public string code { get; set; }
            public string rate { get; set; }
            public string description { get; set; }
            public double rate_float { get; set; }
        }
        public class Bpi
        {
            public USD USD { get; set; }
            public PHP PHP { get; set; }
        }
        public class RootObject
        {
            public Time time { get; set; }
            public string disclaimer { get; set; }
            public Bpi bpi { get; set; }
        }

//==========================================================================================//

        private void timer1_Tick(object sender, EventArgs e)
        {
            //check internet connection
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        lblConnection.Text = "Connected";
                    }
                }
            }
            catch
            {
                lblConnection.Text = "Not Connected";
            }

            Run_API();              

            lblLastCheck.Text = DateTime.Now.ToShortTimeString();
        }

//==========================================================================================//

        public void Run_API()
        {
            if (chkBeep.Checked == true)
            {
                SystemSounds.Beep.Play();
            }            

            string API_string = "https://api.coindesk.com/v1/bpi/currentprice/" + comboBox1.Text + ".json";
                
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(API_string);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(API_string).Result;

            if (response.IsSuccessStatusCode)
            {
                cmbOutput.Items.Clear();
                try
                {
                    decimal deci_Price = 0;
                    var jresult = response.Content.ReadAsStringAsync().Result;
                    
                    String[] substrings = jresult.Split(',');
                    foreach (var substring in substrings)
                    {
                        cmbOutput.Items.Add(substring.ToString().Replace("\"", "").Replace("{", "").Replace("}", ""));                        
                    }
                    if (comboBox1.Text == "USD")
                    {
                        txtBtcPrice.Text = cmbOutput.Items[10].ToString().Replace("rate_float:", "");
                    }
                    if (comboBox1.Text == "PHP")
                    {
                        txtBtcPrice.Text = cmbOutput.Items[15].ToString().Replace("rate_float:", "");                        
                    }
                    deci_Price = Convert.ToDecimal(txtBtcPrice.Text);
                    txtBtcPrice.Text = String.Format("{0:#,###,###}", Convert.ToDecimal(txtBtcPrice.Text));

                    if (chkNotify.Checked == true)
                    {
                        Check_Notif(deci_Price);
                    }

                }
                catch(Exception y)
                {
                    MessageBox.Show(y.Message);
                }
                
            }
            else
            {
                MessageBox.Show("Error Code: " + response.StatusCode + " . Message: " + response.ReasonPhrase);
            }
        }

//==========================================================================================//

        public void Check_Notif(decimal price)
        {
            if(numericMax.Value > 0)
            {
                if(price > numericMax.Value)
                {
                    NotificationRing();
                }
            }

            if(numericMin.Value > 0)
            {
                if(price < numericMin.Value)
                {
                    NotificationRing();
                }
            }
        }

//==========================================================================================//
        
    }//class
}

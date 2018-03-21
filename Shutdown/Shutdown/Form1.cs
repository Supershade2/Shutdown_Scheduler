using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
namespace Shutdown
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (maskedTextBox1.Text != "" || maskedTextBox1.Text != null) 
            {
                DateTime specified_time = new DateTime();
                specified_time = Convert.ToDateTime(maskedTextBox1.Text);
                DateTime c_time = DateTime.Now;
                this.Text = c_time.TimeOfDay.ToString();
                TimeSpan difference = specified_time.TimeOfDay - c_time.TimeOfDay;
                double seconds = difference.TotalSeconds;
                seconds = Math.Ceiling(seconds);
                Process p1 = new Process();
                p1.StartInfo.FileName = @"C:\Windows\System32\shutdown.exe";
                p1.StartInfo.Arguments = "/s /t " + seconds;
                p1.StartInfo.UseShellExecute = true;
                p1.StartInfo.Verb ="runas";
                //p1.StartInfo.RedirectStandardError = true;
                //p1.StartInfo.RedirectStandardOutput = true;
                p1.Start();
                //string output = p1.StandardOutput.ReadToEnd();
                //this.Text = output;
                //Process.Start(@"C:\Windows\System32\shutdown.exe", "-s -t "+seconds);
                this.Close();
            }
        }
        public static DateTime GetNetworkTime()
        {
            const string ntpServer = "time.windows.com";
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            //IPAddress IP = IPAddress.Parse("208.94.243.152");
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            socket.Connect(ipEndPoint);
            socket.Send(ntpData);
            //socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }

        private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            MessageBox.Show("Schedules a system shutdown calculated in minutes","Shutdown Scheduler By Aaron English");
        }
    }
}

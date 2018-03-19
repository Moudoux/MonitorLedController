using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorLedController
{
    public partial class Form1 : Form
    {

        private const String COM_DEVICE = "COM5";

        [DllImport("User32", SetLastError = true, EntryPoint = "RegisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, Int32 Flags);

        [DllImport("User32", EntryPoint = "UnregisterPowerSettingNotification", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnregisterPowerSettingNotification(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public Int32 DataLength;
            public byte Data;
        }

        private Guid GUID_CONSOLE_DISPLAY_STATE = Guid.Parse("6fe69556-704a-47a0-8f24-c28d936fda47");
        private Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        private int WM_POWERBROADCAST = 0x0218;
        private SerialPort com;

        public Form1()
        {
            InitializeComponent();
        }

        private void Print(String text)
        {
            textBox1.Text += (textBox1.Text.Equals("") ? "" : "\r\n") + text;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg.Equals(WM_POWERBROADCAST))
            {
                POWERBROADCAST_SETTING setting = (POWERBROADCAST_SETTING) Marshal.PtrToStructure(m.LParam, typeof(POWERBROADCAST_SETTING));
                com.Write("led " + (setting.Data.Equals(0x0) ? "off" : "on"));
            }
            base.WndProc(ref m);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                com = new SerialPort(COM_DEVICE)
                {
                    BaudRate = 9600,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None,
                    ReadTimeout = 2000,
                    WriteTimeout = 500,
                    DtrEnable = false,
                    RtsEnable = true
                };
                com.Open();
                Print("Connected to serial");
                IntPtr notificationPtr = RegisterPowerSettingNotification(this.Handle, ref GUID_CONSOLE_DISPLAY_STATE,
                    DEVICE_NOTIFY_WINDOW_HANDLE);
            }
            catch (Exception ex)
            {
                Print("Failed to connect to serial");
            }
            textBox2.Select();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Print("Sending command: " + textBox2.Text);
            com.Write(textBox2.Text);
            textBox2.Clear();
        }
    }
}

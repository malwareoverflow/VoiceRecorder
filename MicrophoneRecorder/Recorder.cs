using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MicrophoneRecorder
{
    public partial class Recorder : Form
    {



        private  WaveIn sourceStream;
        private WaveFileWriter waveWriter;
        private readonly string FilePath;
        private readonly string FileName;
        private readonly int InputDeviceIndex;


        public Recorder()
        {
            InitializeComponent();

        }

        private void Browse_Click(object sender, EventArgs e)
        {
            try
            {
                this.folderBrowserDialog1.ShowDialog();
                this.Savedir.Text = this.folderBrowserDialog1.SelectedPath.ToString();
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void Start_Click(object sender, EventArgs e)
        {
            try
            {
                WaveIn in1 = new WaveIn
                {
                    DeviceNumber = this.InputDeviceIndex,
                    WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(this.InputDeviceIndex).Channels)
                };
                this.sourceStream = in1;
                this.sourceStream.DataAvailable += new EventHandler<WaveInEventArgs>(this.SourceStreamDataAvailable);
                this.waveWriter = new WaveFileWriter(this.Savedir.Text + this.Savename.Text + "." + this.ExtensionTextBox.Text, this.sourceStream.WaveFormat);
                this.sourceStream.StartRecording();
                MessageBox.Show("Started");
            }
            catch (Exception)
            {
                throw;
            }

        }

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int record(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
        private void RecordEnd(object sender, EventArgs e)
        {
            if (this.sourceStream != null)
            {
                this.sourceStream.StopRecording();
                this.sourceStream.Dispose();
                this.sourceStream = null;
            }
            if (this.waveWriter != null)
            {
                this.waveWriter.Dispose();
                this.waveWriter = null;
                this.stopbtn.Enabled = false;
                Application.Exit();
                Environment.Exit(0);
            }
        }

        public void SourceStreamDataAvailable(object sender, WaveInEventArgs e)
        {
            if (this.waveWriter != null)
            {
                this.waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                this.waveWriter.Flush();
            }
        }

        private void stopbtn_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Stopped");
                this.sourceStream.StopRecording();
                Process.Start(this.Savedir.Text + this.Savename.Text + "." + this.ExtensionTextBox.Text);
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}

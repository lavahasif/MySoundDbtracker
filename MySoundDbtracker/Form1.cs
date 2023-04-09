using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace MySoundDbtracker
{


    public partial class Form1 : Form
    {
        int MaxValue = 0;
        public Form1()
        {
            InitializeComponent();

            var waveIn = new WaveInEvent();
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();



            //waveIn.StopRecording();

        }
        int bufferSize = 44100; // 1 second of audio data at 44100 Hz
        int samplingRate = 44100;
        int bufferIndex = 0;
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            

            // Initialize variables for storing the audio data and maximum value
            byte[] audioBuffer = new byte[bufferSize * 4];

            // calculate the RMS amplitude
            float rms = 0;
            for (int i = 0; i < e.BytesRecorded; i += 2)
            {
                short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);
                rms += sample * sample;
            }
            rms = (float)Math.Sqrt(rms / (e.BytesRecorded / 2));

            // calculate the dB level
            float dB = 20 * (float)Math.Log10(rms);

            // Add the new data to the audio buffer
            short sampleValue = (short)(dB * short.MaxValue);
            byte[] sampleBytes = BitConverter.GetBytes(sampleValue);
            Array.Copy(sampleBytes, 0, audioBuffer, bufferIndex, sampleBytes.Length);
            bufferIndex += sampleBytes.Length;

            



            int dB1 = (int)dB;
            if (dB1 >= MaxValue && bufferIndex >= 2 * bufferSize * 4)
            {
                saveBuffer(audioBuffer);
                bufferIndex = 0;
            }
                
            progressBar1.Invoke(new Action(() =>
            {
                progressBar1.Value = (int)dB;
                label1.Text = dB1.ToString();
                if (dB1 > MaxValue)
                {

                    MaxValue = dB1;
                    label2.Text = "MaxValue: " + dB1.ToString();
                }
            }

            )); ;


        }

        void saveBuffer(byte[] buffer)
        {
            // Create a new WaveFileWriter object to write the audio data to a file
            WaveFileWriter waveFileWriter = new WaveFileWriter($"outpu2t.wav", new WaveFormat(44100, 16, 2));

            // Write the audio data from the buffer to the WaveFileWriter object
            
            waveFileWriter.Write(buffer, 0, bufferIndex);
            waveFileWriter.Close();
        }
        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}

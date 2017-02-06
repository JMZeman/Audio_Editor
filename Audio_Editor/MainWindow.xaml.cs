using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NAudio;
using NAudio.Wave;
using System.Windows.Forms;
using WPFSoundVisualizationLib;
using System.Threading;


namespace Audio_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<SliderController> controller = new List<SliderController>();
        int number = 0;
        public MainWindow()
        {
            InitializeComponent();



        }

        public void deletesong(int number)
        {

        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            controller.Add(new SliderController(sliderHolder, this.Dispatcher, playbox, pausebox, delete, number, controller));  // add new controller for file
            number++;
        }

        private void pauseAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (SliderController control in controller)//pause all
            {
                control.pauseMusic();
            }

        }

        private void playAll_Click(object sender, RoutedEventArgs e)//void all
        {
            foreach (SliderController control in controller)
            {
                control.playMusic();
            }
        }

        private void Window_Closed(object sender, EventArgs e)//close window
        {
            foreach (SliderController control in controller)
            {
                control.closeThreads();
            }
        }

        private void export_Click(object sender, RoutedEventArgs e)
        {
            List<AudioFileReader> readers = new List<AudioFileReader>();
            List<float[]> buffer = new List<float[]>();
            if (controller.Count != 0)
            {
                foreach (SliderController control in controller) //populate the lists
                {
                    readers.Add(control.AudioFileReader());
                    buffer.Add(new float[control.AudioFileReader().Length / (control.AudioFileReader().WaveFormat.BitsPerSample / 8)]); //add float thing for each
                    readers.Last().Read(buffer.Last(), 0, buffer.Last().Length);

                }
            }
            else
                return;
            List<long> length = new List<long>();
            foreach (float[] f in buffer)
            {
                length.Add(f.Length);
            }
            long max = length.Max();
            var final = new byte[max];
            System.Diagnostics.Debug.WriteLine(max);


            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "wav files (*.wav)|*.wav";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;


            if (!saveFileDialog.ShowDialog().Equals(""))
            {
                WaveFileWriter writer = new WaveFileWriter(saveFileDialog.FileName, readers[0].WaveFormat);

                List<float> longList = new List<float>();
                for (int i = 0; i < max; i++)                           //go though for each byte
                {

                    foreach (float[] a in buffer)
                    {
                        if (i < a.Length)
                        {
                            longList.Add(a[i] / 2);
                        }
                        else
                            longList.Add(0);
                    }
                    System.Diagnostics.Debug.WriteLine(i + "/" + max);
                    writer.WriteSample(longList.Sum());
                    longList.Clear();
                }

            }

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < number; i++)
            {
                controller[0].closeAll();
            }
            number = 0;                                     //set counter back to 0
        }

        private void about_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("author: Jared Zeman\npawprint: jmzty5\nAplication: basic audio mixer\ndiscription: it can take multiple audio files and combine them into a wav file.", "about");
        }
    }
}

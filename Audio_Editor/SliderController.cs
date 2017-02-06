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
using System.Windows.Threading;
namespace Audio_Editor
{
    class SliderController
    {
        private IWavePlayer waveOutDevice = new WaveOut();
        private AudioFileReader audioFileReader;
        Slider slider = new Slider();
        Thread sliderThread;
        Dispatcher disp;
        OpenFileDialog openFileDialog;
        System.Windows.Controls.ListBox listbox;
        System.Windows.Controls.ListBox playbox;
        System.Windows.Controls.ListBox pausebox;
        System.Windows.Controls.ListBox deletebox;
        List<SliderController> controller;
        int number;

        public SliderController(System.Windows.Controls.ListBox listbox, Dispatcher dispature, System.Windows.Controls.ListBox playbox, System.Windows.Controls.ListBox pausebox, System.Windows.Controls.ListBox deletebox, int number, List<SliderController> controller)
        {
            disp = dispature;
            openFileDialog = getPath();
            this.listbox = listbox;
            this.playbox = playbox;
            this.pausebox = pausebox;
            this.deletebox = deletebox;
            this.controller = controller;


            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    audioFileReader = new AudioFileReader(openFileDialog.FileName);
                    waveOutDevice.Init(audioFileReader);

                    slider.Maximum = audioFileReader.Length;
                    slider.Width = 400;
                    slider.LostMouseCapture += Slider_LostMouseCapture;
                    listbox.Items.Add(slider);
                    System.Windows.Controls.Button button = new System.Windows.Controls.Button();
                    System.Windows.Controls.Button pause = new System.Windows.Controls.Button();
                    System.Windows.Controls.Button delete = new System.Windows.Controls.Button();
                    button.Height = 20;
                    button.Width = 50;
                    pause.Height = 20;
                    pause.Width = 50;
                    delete.Width = 50;
                    delete.Height = 20;
                    button.Content = "play";
                    pause.Content = "pause";
                    delete.Content = "remove";
                    pause.Click += pauseButton;
                    button.Click += playButton;
                    delete.Click += Delete_Click;
                    waveOutDevice.Play();
                    playbox.Items.Add(button);
                    pausebox.Items.Add(pause);
                    deletebox.Items.Add(delete);
                    sliderThread = new Thread(updateslider);
                    sliderThread.Start();

                }
                catch
                {
                    return;
                }
            }
            return;

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            waveOutDevice.Dispose();
            sliderThread.Abort();
            pausebox.Items.RemoveAt(number);
            playbox.Items.RemoveAt(number);
            deletebox.Items.RemoveAt(number);
            listbox.Items.RemoveAt(number);
            controller.RemoveAt(number);

        }

        private void Slider_LostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
        {
            audioFileReader.Position = (long)slider.Value;

        }


        private void updateslider()
        {
            while (true)
            {


                if (slider.IsMouseOver)
                {

                }
                else
                    UpdatePosition(audioFileReader.Position);


                Thread.Sleep(500);
            }

        }

        void UpdatePosition(double byteAmount)
        {
            Action action = () => { slider.Value = byteAmount; };
            disp.BeginInvoke(action);
        }

        private void pauseButton(object sender, RoutedEventArgs e)
        {
            waveOutDevice.Pause();
        }

        private void playButton(object sender, RoutedEventArgs e)
        {
            waveOutDevice.Play();
        }

        public OpenFileDialog getPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "mp3 files (*.mp3)|*.mp3";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            return openFileDialog;
        }

        public void pauseMusic()
        {
            waveOutDevice.Pause();
        }

        public void playMusic()
        {
            waveOutDevice.Play();
        }

        public void closeThreads()
        {
            sliderThread.Abort();
        }

        public void closeAll()
        {
            waveOutDevice.Dispose();
            sliderThread.Abort();
            pausebox.Items.RemoveAt(number);
            playbox.Items.RemoveAt(number);
            deletebox.Items.RemoveAt(number);
            listbox.Items.RemoveAt(number);
            controller.RemoveAt(number);
        }

        public long getPosition()
        {
            return audioFileReader.Position;
        }

        public AudioFileReader AudioFileReader()
        {
            return audioFileReader;
        }

        public string getFilePath()
        {
            return openFileDialog.FileName;
        }
    }

}

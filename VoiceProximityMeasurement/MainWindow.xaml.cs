using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoGenerateSnellenVisionChart;
using Microsoft.CognitiveServices.Speech;
using VoiceProximityMeasurement.ViewModel;

namespace VoiceProximityMeasurement
{
    public partial class MainWindow : Window
    {
        private MainVM _mainVM = new MainVM();

        private string _subscriptionKey = "2a0f03ac052147cb808fa799634b1209";
        private string _serviceRegion = "southeastasia";

        private SpeechConfig _config;
        private SpeechRecognizer _recognizer;
        private TaskCompletionSource<int> _stopRecognition;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = _mainVM;

            // Initialize Speech recognizer
            _config = SpeechConfig.FromSubscription(_subscriptionKey, _serviceRegion);
            _recognizer = new SpeechRecognizer(_config);

            // Subscribe to Speech events
            _recognizer.Recognizing += Recognizer_Recognizing;
            _recognizer.Recognized += Recognizer_Recognized;
            _recognizer.Canceled += Recognizer_Canceled;
            _recognizer.SessionStarted += Recognizer_SessionStarted;
            _recognizer.SessionStopped += Recognizer_SessionStopped;

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Unsubscribe from Speech events
            _recognizer.Recognizing -= Recognizer_Recognizing;
            _recognizer.Recognized -= Recognizer_Recognized;
            _recognizer.Canceled -= Recognizer_Canceled;
            _recognizer.SessionStarted -= Recognizer_SessionStarted;
            _recognizer.SessionStopped -= Recognizer_SessionStopped;

            // Dispose Speech recognizer
            _recognizer.Dispose();
        }

        private async void GenerateChart_Click(object sender, RoutedEventArgs e)
        {
            _mainVM.Images.Clear();
            for (int i = 0; i < 8; i++)
            {
                LoadRandomImages(i);
                await Task.Delay(2000); // Wait for 2 seconds
            }
        }

        private void LoadRandomImages(int loop)
        {
            _mainVM.Images.Clear();
            // Get the path of the Img folder
            string imgFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Img");

            // Get all image paths in the Img folder
            string[] imagePaths = Directory.GetFiles(imgFolderPath, "*.jpg");

            if (imagePaths.Length == 0)
            {
                MessageBox.Show("No images found in the 'Img' folder.");
                return;
            }

            Random rng = new Random();

            // Choose 10 random images from the Img folder
            for (int i = 0; i < 6; i++)
            {
                string randomImagePath = imagePaths[rng.Next(imagePaths.Length)];
                BitmapImage bitmap = new BitmapImage(new Uri(randomImagePath, UriKind.RelativeOrAbsolute));
                double scaleFactor = 1.0 - (loop * 0.1);
                double newWidth = bitmap.PixelWidth * scaleFactor;
                double newHeight = bitmap.PixelHeight * scaleFactor;

                // Create a TransformedBitmap to resize the image
                TransformedBitmap transformedBitmap = new TransformedBitmap(bitmap, new ScaleTransform(newWidth / bitmap.PixelWidth, newHeight / bitmap.PixelHeight));

                _mainVM.Images.Add(new ImageWrapper
                {
                    Bitmap = bitmap,
                    ImageWidth = newWidth,
                    ImageHeight = newHeight
                });
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // Recognizer is in a stopped state
            if (_mainVM.StartStop == "Start")
            {
                _mainVM.StartStop = "Stop";

                _stopRecognition = new TaskCompletionSource<int>();

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await _recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Waits for completion.
                // Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { _stopRecognition.Task });
            }
            else if (_mainVM.StartStop == "Stop")
            {
                // Stops recognition.
                await _recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

                _mainVM.StartStop = "Start";
                _mainVM.Speech = String.Empty;
            }
        }

        private void Recognizer_SessionStopped(object sender, SessionEventArgs e)
        {
            _mainVM.Status = "Speech recognition session stopped";
            _stopRecognition.TrySetResult(0);
        }

        private void Recognizer_SessionStarted(object sender, SessionEventArgs e)
        {
            _mainVM.Status = "Speech recognition session started...";
        }

        private void Recognizer_Canceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            _mainVM.Status = ($"CANCELED: Reason={e.Reason}");
            if (e.Reason == CancellationReason.Error)
            {
                //Just writing to debug output here, could display this to user
                Debug.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                Debug.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                Debug.WriteLine($"CANCELED: Did you update the subscription info?");
            }
            _stopRecognition.TrySetResult(0);
        }

        private void Recognizer_Recognized(object sender, SpeechRecognitionEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Result.Text))
                _mainVM.Speech = string.Empty;

            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                _mainVM.Transcribed += e.Result.Text;
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                _mainVM.Status = "Unable to recognize speech";
            }
        }

        private void Recognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
        {
            _mainVM.Speech = e.Result.Text;
        }

        private void ClearText(object sender, RoutedEventArgs e)
        {
            _mainVM.Speech = string.Empty;
            _mainVM.Transcribed = string.Empty;
        }
    }
}

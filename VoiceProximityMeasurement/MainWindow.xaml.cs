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
using OfficeOpenXml;
using VoiceProximityMeasurement.ViewModel;

namespace VoiceProximityMeasurement
{
    public partial class MainWindow : Window
    {
        private MainVM _mainVM = new MainVM();

        private string _subscriptionKey = "2a0f03ac052147cb808fa799634b1209";
        private string _serviceRegion = "southeastasia";
        private int loop = 2;
        private int countdownTime = 10;

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
            for (int i = 0; i < loop; i++)
            {
                LoadRandomImages(i);
                for (int j = countdownTime; j >= 0; j--)
                {
                    UpdateCountdown(j);
                    await Task.Delay(1000);
                }
                _mainVM.LoadedResult.Add(ProcessTranscribed(_mainVM.Transcribed));
                _mainVM.Transcribed = string.Empty;
            }
        }

        private void UpdateCountdown(int secondsLeft)
        {
            Dispatcher.Invoke(() =>
            {
                CountdownText.Text = $"Time out: {secondsLeft}";
            });
        }

        private void LoadRandomImages(int round)
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

            string loadedImageNames = "";

            // Choose random images from the Img folder
            for (int i = 0; i < 6; i++)
            {
                string randomImagePath = imagePaths[rng.Next(imagePaths.Length)];
                BitmapImage bitmap = new BitmapImage(new Uri(randomImagePath, UriKind.RelativeOrAbsolute));
                double scaleFactor = 1.0 - (round * 0.1);
                double newWidth = bitmap.PixelWidth * scaleFactor;
                double newHeight = bitmap.PixelHeight * scaleFactor;

                // Create a TransformedBitmap to resize the image
                TransformedBitmap transformedBitmap = new TransformedBitmap(bitmap, new ScaleTransform(newWidth / bitmap.PixelWidth, newHeight / bitmap.PixelHeight));

                _mainVM.Images.Add(new ImageWrapper
                {
                    Bitmap = bitmap,
                    ImageWidth = newWidth,
                    ImageHeight = newHeight,
                    CorrectAnswer = Path.GetFileNameWithoutExtension(randomImagePath)
                });

                string imageName = Path.GetFileNameWithoutExtension(randomImagePath);
                loadedImageNames += imageName + ",";
            }
            // Remove the trailing comma, if any
            if (!string.IsNullOrEmpty(loadedImageNames))
            {
                loadedImageNames = loadedImageNames.TrimEnd(',');
            }

            // Append the concatenated image names to the LoadedImagesNames list
            _mainVM.LoadedImagesNames.Add(loadedImageNames);

            //MessageBox.Show("Loaded image names: " + loadedImageNames);
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

        private void ExportToExcel()
        {
            // Setup Excel package
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            FileInfo fi = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Answer.xlsx"));

            using (var package = new ExcelPackage(fi))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault() ?? package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells["A1"].Value = "Picture Question";
                worksheet.Cells["B1"].Value = "Picture Answer";

                // Assuming _mainVM.Images and _mainVM.Transcribed are ready to be processed
                //var answers = _mainVM.Transcribed.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                //               .Select(ans => ans.Trim())
                //               .ToList();

                //for (int i = 0; i < _mainVM.Images.Count && i < answers.Count; i++)
                //{
                //    worksheet.Cells[i + 2, 1].Value = _mainVM.Images[i].CorrectAnswer; // Câu hỏi
                //    if (i < answers.Count)
                //    {
                //        worksheet.Cells[i + 2, 2].Value = answers[i]; // Câu trả lời
                //    }
                //}

                for (int i = 0; i < loop; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = _mainVM.LoadedImagesNames[i];
                    worksheet.Cells[i + 2, 2].Value = _mainVM.LoadedResult[i];                  
                }

                // Attempt to save
                try
                {
                    package.Save();
                    MessageBox.Show("Exported to Excel successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save the Excel file. Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void ProcessFinalResults()
        {
            var validCommands = new HashSet<string> { "Up", "Down", "Right", "Left" };
            var processedResults = _mainVM.Transcribed
                .Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(result => validCommands.Contains(result, StringComparer.OrdinalIgnoreCase))
                .ToList();

            // Now, reconstruct the _mainVM.Transcribed with only valid results.
            _mainVM.Transcribed = string.Join(", ", processedResults);
        }
        private string ProcessTranscribed(string transcribed)
        {
            var validCommands = new HashSet<string> { "Up", "Down", "Right", "Left" };
            var processedResults = transcribed
                .Split(new[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(word => validCommands.Contains(word, StringComparer.OrdinalIgnoreCase) ? word : "")
                .ToList();
            
            string result = string.Join(", ", processedResults).ToLower();
            MessageBox.Show($"Loaded new transcribed : '{result}'");
            return result;
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            // Existing logic to generate the chart...

            // Process final results before exporting
            //ProcessFinalResults();

            // Then export to Excel
            ExportToExcel();
        }

        private void OpenResult(object sender, RoutedEventArgs e)
        {
            ExportToExcelButton_Click(sender, e);
            Result result = new Result();
            result.Show();
            this.Close();
        }
    }
}

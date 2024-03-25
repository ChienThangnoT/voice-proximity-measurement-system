using Microsoft.CognitiveServices.Speech;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceProximityMeasurement.ViewModel;

namespace VoiceProximityMeasurement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string subscriptionKey = "2a0f03ac052147cb808fa799634b1209";
        string serviceRegion = "southeastasia";

        SpeechConfig config;

        SpeechRecognizer recognizer;

        TaskCompletionSource<int> stopRecognition;

        private MainVM _mainVM = new MainVM();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = _mainVM;

            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key // and service region (e.g., "westus").
            config = SpeechConfig.FromSubscription(subscriptionKey, serviceRegion);

            recognizer = new SpeechRecognizer(config);

            //subscribe to events
            recognizer.Recognizing += Recognizer_Recognizing;
            recognizer.Recognized += Recognizer_Recognized;
            recognizer.Canceled += Recognizer_Canceled;
            recognizer.SessionStarted += Recognizer_SessionStarted;
            recognizer.SessionStopped += Recognizer_SessionStopped;

            Closing += MainWindow_Closing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //unsubscribe from events (just in case its not being done in Dispose)
            recognizer.Recognizing -= Recognizer_Recognizing;
            recognizer.Recognized -= Recognizer_Recognized;
            recognizer.Canceled -= Recognizer_Canceled;
            recognizer.SessionStarted -= Recognizer_SessionStarted;
            recognizer.SessionStopped -= Recognizer_SessionStopped;

            recognizer.Dispose();
        }

        /// <summary>
        /// Start/Stop speech recognition session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //recognizer is in a stopped state
            if (_mainVM.StartStop == "Start")
            {
                _mainVM.StartStop = "Stop";

                stopRecognition = new TaskCompletionSource<int>();

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                // Waits for completion.
                // Use Task.WaitAny to keep the task rooted.
                Task.WaitAny(new[] { stopRecognition.Task });
            }
            else if (_mainVM.StartStop == "Stop")
            {
                // Stops recognition.
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

                _mainVM.StartStop = "Start";
                _mainVM.Speech = String.Empty;
            }
        }

        /// <summary>
        /// Speech recognition session stopped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recognizer_SessionStopped(object sender, SessionEventArgs e)
        {
            _mainVM.Status = "Speech recognition session stopped";
            stopRecognition.TrySetResult(0);
        }

        /// <summary>
        /// Speech recgnition session started
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recognizer_SessionStarted(object sender, SessionEventArgs e)
        {
            _mainVM.Status = "Speech recognition session started...";
        }

        /// <summary>
        ///  Signal for events containing canceled recognition results 
        ///  (indicating a recognition attempt that was canceled as a result or a direct cancellation request or, alternatively, a transport or protocol failure)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            stopRecognition.TrySetResult(0);
        }

        /// <summary>
        /// Final recognition results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// intermediate recognition resuts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recognizer_Recognizing(object sender, SpeechRecognitionEventArgs e)
        {
            _mainVM.Speech = e.Result.Text;
        }

        /// <summary>
        /// Clears all text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearText(object sender, RoutedEventArgs e)
        {
            _mainVM.Speech = string.Empty;
            _mainVM.Transcribed = string.Empty;
        }
    }
}
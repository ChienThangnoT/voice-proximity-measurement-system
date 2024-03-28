using AutoGenerateSnellenVisionChart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceProximityMeasurement.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Intermediate recognition result
        /// </summary>
        private string _speech = "";
        public string Speech
        {
            get { return _speech; }
            set
            {
                _speech = value;
                OnPropertyChanged("Speech");
            }
        }

        /// <summary>
        /// The transcribed audio
        /// </summary>
        private string _transcribed = "";
        public string Transcribed
        {
            get { return _transcribed; }
            set
            {
                _transcribed = value;
                OnPropertyChanged("Transcribed");
            }
        }

        /// <summary>
        /// Indicates if the session is started or stopped
        /// </summary>
        private string _startstop = "Start";
        public string StartStop
        {
            get { return _startstop; }
            set
            {
                _startstop = value;
                OnPropertyChanged("StartStop");
            }
        }

        /// <summary>
        /// Status or error messages
        /// </summary>
        private string _status = "";
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<ImageWrapper> _images = new ObservableCollection<ImageWrapper>();
        public ObservableCollection<ImageWrapper> Images
        {
            get { return _images; }
            set
            {
                _images = value;
                OnPropertyChanged(nameof(Images));
            }
        }
        #endregion
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SightHighlighter
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Data binding
        // ref: https://medium.com/oldbeedev/wpf-data-binding-concept-mode-updatesourcetrigger-648735b2444
        //private double _similarityThresholdPercent = 99;
        //private double _similarityThreshold = 99;
        //public double SimilarityThresholdPercent
        //{
        //    get { return _similarityThresholdPercent; }
        //    set { 
        //        _similarityThresholdPercent = value;
        //        _similarityThreshold = _similarityThreshold / 100.0;
        //        OnPropertyChanged("SimilarityThreshold");
        //    }
        //}

        public ButtonCommand ApplySimilarityCommand { get; set; }
        public MainWindowViewModel()
        {
            ApplySimilarityCommand = new ButtonCommand(ApplySimilarity,CanApplySimilarity);
            // ApplySimilarityCommand = new DelegateCommand(ApplySimilarity); // requires prism
        }

        // role of delegateCommand: separate callback command and real action in MVVM model
        // in this time, command:ApplySimilarityCommand, Action:ApplySimilarity
        // ref: https://chashtag.tistory.com/57
        // public DelegateCommand ApplySimilarityCommand { get; private set; }

        private void ApplySimilarity(object? obj)
        {
            //Debug.WriteLine(_similarityThreshold.ToString());
            // ImageProcessor.threshold = _similarityThreshold/100.0; // use private variable
        }

        private bool CanApplySimilarity(object? obj)
        {
            return true;
        }



        private double similarityThresholdPercent;
        public double SimilarityThresholdPercent { get => similarityThresholdPercent; set => SetProperty(ref similarityThresholdPercent, value); }

        // Images

    }
}

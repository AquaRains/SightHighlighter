using System.ComponentModel;
using System.Diagnostics;

namespace SightHighlighter
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Data binding
        // ref: https://medium.com/oldbeedev/wpf-data-binding-concept-mode-updatesourcetrigger-648735b2444
        private double _similarityThreshold = 100;
        public double similarityThreshold
        {
            get { return _similarityThreshold; }
            set { _similarityThreshold = value; OnPropertyChanged("similarityThreshold"); }
        }

        public ButtonCommand ApplySimilarityCommand { get; set; }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // role of delegateCommand: separate callback command and real action in MVVM model
        // in this time, command:ApplySimilarityCommand, Action:ApplySimilarity
        // ref: https://chashtag.tistory.com/57
        // public DelegateCommand ApplySimilarityCommand { get; private set; }

        public MainWindowViewModel()
        {
            ApplySimilarityCommand = new ButtonCommand(ApplySimilarity,CanApplySimilarity);
            // ApplySimilarityCommand = new DelegateCommand(ApplySimilarity);
        }

        private void ApplySimilarity(object? obj)
        {
            //Debug.WriteLine(_similarityThreshold.ToString());
            ImageProcessor.threshold = _similarityThreshold/100.0; // use private variable
        }

        private bool CanApplySimilarity(object? obj)
        {
            return true;
        }

    }
}

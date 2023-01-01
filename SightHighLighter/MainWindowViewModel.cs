using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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



        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // role of delegateCommand: separate callback command and real action in MVVM model
        // in this time, command:ApplySimilarityCommand, Action:ApplySimilarity
        // ref: https://chashtag.tistory.com/57
        public DelegateCommand ApplySimilarityCommand { get; private set; }

        public MainWindowViewModel()
        {
            ApplySimilarityCommand = new DelegateCommand(ApplySimilarity);
        }

        private void ApplySimilarity()
        {
            // OnPropertyChanged("similarityThreshold");
        }
    }
}

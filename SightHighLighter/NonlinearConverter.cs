using System;
using System.Globalization;
using System.Windows.Data;

namespace SightHighlighter
{

    public class NonlinearConverter: IValueConverter
    {
        // Convert: source(data) to target(UI), ConvertBack: reverse way
        // ref: https://medium.com/oldbeedev/wpf-data-binding-with-ivalueconverter-f5e459e03f8e
        
        // approximate y= A*Log10(x) + b
        // where y:similarityThreshold, x:sliderval(1-100)
        private double A = 49.5;
        private double b = 1.0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Pow(10, ((double)value - b) / A);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (A * Math.Log10((double)value) + b); // sliderVal to similarity
        }
    }
}

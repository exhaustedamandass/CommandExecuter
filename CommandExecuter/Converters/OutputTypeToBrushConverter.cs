using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using CommandExecuter.Models;

namespace CommandExecuter.Converters;

public class OutputTypeToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is OutputType outputType)
        {
            return outputType switch
            {
                OutputType.Standard => Brushes.Blue,
                OutputType.Error => Brushes.Red,
                OutputType.Status => Brushes.Green,
                _ => Brushes.Blue,
            };
        }
        return Brushes.Blue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
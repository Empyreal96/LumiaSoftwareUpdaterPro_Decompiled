// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Converters.UniversalConverter
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Windows.Data;

namespace GalaSoft.MvvmLight.Converters
{
  public class UniversalConverter : IValueConverter
  {
    private static Dictionary<string, Delegate> _operations;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => this.GetValue(0, value, parameter);

    private object GetValue(int index, object value, object expression)
    {
      if (value == null || expression == null)
        return value;
      string str = expression.ToString();
      if (string.IsNullOrEmpty(str))
        return value;
      string[] strArray = new string[1]{ str };
      if (str.IndexOf("#") > -1)
        strArray = str.Split('#');
      if (index > strArray.Length - 1)
        return value;
      Delegate @delegate = UniversalConverter.ConstructOperation(value, strArray[index]);
      if ((object) @delegate == null)
        return value;
      return @delegate.DynamicInvoke(value);
    }

    private static Delegate ConstructOperation(object value, string parameter)
    {
      if (UniversalConverter._operations == null)
        UniversalConverter._operations = new Dictionary<string, Delegate>();
      if (UniversalConverter._operations.ContainsKey(parameter))
        return UniversalConverter._operations[parameter];
      int length = parameter.IndexOf("=>");
      string name = length >= 0 ? parameter.Substring(0, length) : throw new ArgumentException("No lambda operator =>", nameof (parameter));
      string expression = parameter.Substring(length + 2);
      Delegate @delegate = DynamicExpressionEx.ParseLambda(new ParameterExpression[1]
      {
        Expression.Parameter(value.GetType(), name)
      }, typeof (object), expression, value).Compile();
      UniversalConverter._operations.Add(parameter, @delegate);
      return @delegate;
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      return this.GetValue(1, value, parameter);
    }
  }
}

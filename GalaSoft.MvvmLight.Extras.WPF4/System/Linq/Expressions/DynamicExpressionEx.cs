// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.DynamicExpressionEx
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System.Collections.Generic;

namespace System.Linq.Expressions
{
  public static class DynamicExpressionEx
  {
    public static Expression Parse(
      Type resultType,
      string expression,
      params object[] values)
    {
      return new ExpressionParser((ParameterExpression[]) null, expression, values).Parse(resultType);
    }

    public static LambdaExpression ParseLambda(
      Type itType,
      Type resultType,
      string expression,
      params object[] values)
    {
      return DynamicExpressionEx.ParseLambda(new ParameterExpression[1]
      {
        Expression.Parameter(itType, "")
      }, resultType, expression, values);
    }

    public static LambdaExpression ParseLambda(
      ParameterExpression[] parameters,
      Type resultType,
      string expression,
      params object[] values)
    {
      return Expression.Lambda(new ExpressionParser(parameters, expression, values).Parse(resultType), parameters);
    }

    public static Expression<Func<T, S>> ParseLambda<T, S>(
      string expression,
      params object[] values)
    {
      return (Expression<Func<T, S>>) DynamicExpressionEx.ParseLambda(typeof (T), typeof (S), expression, values);
    }

    public static Type CreateClass(params DynamicProperty[] properties) => ClassFactory.Instance.GetDynamicClass((IEnumerable<DynamicProperty>) properties);

    public static Type CreateClass(IEnumerable<DynamicProperty> properties) => ClassFactory.Instance.GetDynamicClass(properties);
  }
}

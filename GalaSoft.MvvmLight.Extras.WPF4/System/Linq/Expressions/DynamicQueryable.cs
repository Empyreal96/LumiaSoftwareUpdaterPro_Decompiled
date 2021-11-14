// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.DynamicQueryable
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System.Collections.Generic;

namespace System.Linq.Expressions
{
  public static class DynamicQueryable
  {
    public static IQueryable<T> Where<T>(
      this IQueryable<T> source,
      string predicate,
      params object[] values)
    {
      return (IQueryable<T>) source.Where(predicate, values);
    }

    public static IQueryable Where(
      this IQueryable source,
      string predicate,
      params object[] values)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (predicate == null)
        throw new ArgumentNullException(nameof (predicate));
      LambdaExpression lambda = DynamicExpressionEx.ParseLambda(source.ElementType, typeof (bool), predicate, values);
      return source.Provider.CreateQuery((Expression) Expression.Call(typeof (Queryable), nameof (Where), new Type[1]
      {
        source.ElementType
      }, source.Expression, (Expression) Expression.Quote((Expression) lambda)));
    }

    public static IQueryable Select(
      this IQueryable source,
      string selector,
      params object[] values)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (selector == null)
        throw new ArgumentNullException(nameof (selector));
      LambdaExpression lambda = DynamicExpressionEx.ParseLambda(source.ElementType, (Type) null, selector, values);
      return source.Provider.CreateQuery((Expression) Expression.Call(typeof (Queryable), nameof (Select), new Type[2]
      {
        source.ElementType,
        lambda.Body.Type
      }, source.Expression, (Expression) Expression.Quote((Expression) lambda)));
    }

    public static IQueryable<T> OrderBy<T>(
      this IQueryable<T> source,
      string ordering,
      params object[] values)
    {
      return (IQueryable<T>) source.OrderBy(ordering, values);
    }

    public static IQueryable OrderBy(
      this IQueryable source,
      string ordering,
      params object[] values)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (ordering == null)
        throw new ArgumentNullException(nameof (ordering));
      ParameterExpression[] parameters = new ParameterExpression[1]
      {
        Expression.Parameter(source.ElementType, "")
      };
      IEnumerable<DynamicOrdering> ordering1 = new ExpressionParser(parameters, ordering, values).ParseOrdering();
      Expression expression = source.Expression;
      string str1 = nameof (OrderBy);
      string str2 = "OrderByDescending";
      foreach (DynamicOrdering dynamicOrdering in ordering1)
      {
        expression = (Expression) Expression.Call(typeof (Queryable), dynamicOrdering.Ascending ? str1 : str2, new Type[2]
        {
          source.ElementType,
          dynamicOrdering.Selector.Type
        }, expression, (Expression) Expression.Quote((Expression) Expression.Lambda(dynamicOrdering.Selector, parameters)));
        str1 = "ThenBy";
        str2 = "ThenByDescending";
      }
      return source.Provider.CreateQuery(expression);
    }

    public static IQueryable Take(this IQueryable source, int count)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      return source.Provider.CreateQuery((Expression) Expression.Call(typeof (Queryable), nameof (Take), new Type[1]
      {
        source.ElementType
      }, source.Expression, (Expression) Expression.Constant((object) count)));
    }

    public static IQueryable Skip(this IQueryable source, int count)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      return source.Provider.CreateQuery((Expression) Expression.Call(typeof (Queryable), nameof (Skip), new Type[1]
      {
        source.ElementType
      }, source.Expression, (Expression) Expression.Constant((object) count)));
    }

    public static IQueryable GroupBy(
      this IQueryable source,
      string keySelector,
      string elementSelector,
      params object[] values)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (keySelector == null)
        throw new ArgumentNullException(nameof (keySelector));
      if (elementSelector == null)
        throw new ArgumentNullException(nameof (elementSelector));
      LambdaExpression lambda1 = DynamicExpressionEx.ParseLambda(source.ElementType, (Type) null, keySelector, values);
      LambdaExpression lambda2 = DynamicExpressionEx.ParseLambda(source.ElementType, (Type) null, elementSelector, values);
      return source.Provider.CreateQuery((Expression) Expression.Call(typeof (Queryable), nameof (GroupBy), new Type[3]
      {
        source.ElementType,
        lambda1.Body.Type,
        lambda2.Body.Type
      }, source.Expression, (Expression) Expression.Quote((Expression) lambda1), (Expression) Expression.Quote((Expression) lambda2)));
    }

    public static bool Any(this IQueryable source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      return (bool) source.Provider.Execute((Expression) Expression.Call(typeof (Queryable), nameof (Any), new Type[1]
      {
        source.ElementType
      }, source.Expression));
    }

    public static int Count(this IQueryable source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      return (int) source.Provider.Execute((Expression) Expression.Call(typeof (Queryable), nameof (Count), new Type[1]
      {
        source.ElementType
      }, source.Expression));
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.ExpressionParser
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace System.Linq.Expressions
{
  internal class ExpressionParser
  {
    private static readonly Type[] predefinedTypes = new Type[20]
    {
      typeof (object),
      typeof (bool),
      typeof (char),
      typeof (string),
      typeof (sbyte),
      typeof (byte),
      typeof (short),
      typeof (ushort),
      typeof (int),
      typeof (uint),
      typeof (long),
      typeof (ulong),
      typeof (float),
      typeof (double),
      typeof (Decimal),
      typeof (DateTime),
      typeof (TimeSpan),
      typeof (Guid),
      typeof (Math),
      typeof (Convert)
    };
    private static readonly Expression trueLiteral = (Expression) Expression.Constant((object) true);
    private static readonly Expression falseLiteral = (Expression) Expression.Constant((object) false);
    private static readonly Expression nullLiteral = (Expression) Expression.Constant((object) null);
    private static readonly string keywordIt = nameof (it);
    private static readonly string keywordIif = "iif";
    private static readonly string keywordNew = "new";
    private static Dictionary<string, object> keywords;
    private Dictionary<string, object> symbols;
    private IDictionary<string, object> externals;
    private Dictionary<Expression, string> literals;
    private ParameterExpression it;
    private string text;
    private int textPos;
    private int textLen;
    private char ch;
    private ExpressionParser.Token token;

    public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
    {
      if (expression == null)
        throw new ArgumentNullException(nameof (expression));
      if (ExpressionParser.keywords == null)
        ExpressionParser.keywords = ExpressionParser.CreateKeywords();
      this.symbols = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.literals = new Dictionary<Expression, string>();
      if (parameters != null)
        this.ProcessParameters(parameters);
      if (values != null)
        this.ProcessValues(values);
      this.text = expression;
      this.textLen = this.text.Length;
      this.SetTextPos(0);
      this.NextToken();
    }

    private void ProcessParameters(ParameterExpression[] parameters)
    {
      foreach (ParameterExpression parameter in parameters)
      {
        if (!string.IsNullOrEmpty(parameter.Name))
          this.AddSymbol(parameter.Name, (object) parameter);
      }
      if (parameters.Length != 1 || !string.IsNullOrEmpty(parameters[0].Name))
        return;
      this.it = parameters[0];
    }

    private void ProcessValues(object[] values)
    {
      for (int index = 0; index < values.Length; ++index)
      {
        object obj = values[index];
        if (index == values.Length - 1 && obj is IDictionary<string, object>)
          this.externals = (IDictionary<string, object>) obj;
        else
          this.AddSymbol("@" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture), obj);
      }
    }

    private void AddSymbol(string name, object value)
    {
      if (this.symbols.ContainsKey(name))
        throw this.ParseError("The identifier '{0}' was defined more than once", (object) name);
      this.symbols.Add(name, value);
    }

    public Expression Parse(Type resultType)
    {
      int pos = this.token.pos;
      Expression expr = this.ParseExpression();
      if (resultType != (Type) null && (expr = this.PromoteExpression(expr, resultType, true)) == null)
        throw this.ParseError(pos, "Expression of type '{0}' expected", (object) ExpressionParser.GetTypeName(resultType));
      this.ValidateToken(ExpressionParser.TokenId.End, "Syntax error");
      return expr;
    }

    public IEnumerable<DynamicOrdering> ParseOrdering()
    {
      List<DynamicOrdering> dynamicOrderingList = new List<DynamicOrdering>();
      while (true)
      {
        Expression expression = this.ParseExpression();
        bool flag = true;
        if (this.TokenIdentifierIs("asc") || this.TokenIdentifierIs("ascending"))
          this.NextToken();
        else if (this.TokenIdentifierIs("desc") || this.TokenIdentifierIs("descending"))
        {
          this.NextToken();
          flag = false;
        }
        dynamicOrderingList.Add(new DynamicOrdering()
        {
          Selector = expression,
          Ascending = flag
        });
        if (this.token.id == ExpressionParser.TokenId.Comma)
          this.NextToken();
        else
          break;
      }
      this.ValidateToken(ExpressionParser.TokenId.End, "Syntax error");
      return (IEnumerable<DynamicOrdering>) dynamicOrderingList;
    }

    private Expression ParseExpression()
    {
      int pos = this.token.pos;
      Expression test = this.ParseLogicalOr();
      if (this.token.id == ExpressionParser.TokenId.Question)
      {
        this.NextToken();
        Expression expression1 = this.ParseExpression();
        this.ValidateToken(ExpressionParser.TokenId.Colon, "':' expected");
        this.NextToken();
        Expression expression2 = this.ParseExpression();
        test = this.GenerateConditional(test, expression1, expression2, pos);
      }
      return test;
    }

    private Expression ParseLogicalOr()
    {
      Expression left = this.ParseLogicalAnd();
      while (this.token.id == ExpressionParser.TokenId.DoubleBar || this.TokenIdentifierIs("or"))
      {
        ExpressionParser.Token token = this.token;
        this.NextToken();
        Expression logicalAnd = this.ParseLogicalAnd();
        this.CheckAndPromoteOperands(typeof (ExpressionParser.ILogicalSignatures), token.text, ref left, ref logicalAnd, token.pos);
        left = (Expression) Expression.OrElse(left, logicalAnd);
      }
      return left;
    }

    private Expression ParseLogicalAnd()
    {
      Expression left = this.ParseComparison();
      while (this.token.id == ExpressionParser.TokenId.DoubleAmphersand || this.TokenIdentifierIs("and"))
      {
        ExpressionParser.Token token = this.token;
        this.NextToken();
        Expression comparison = this.ParseComparison();
        this.CheckAndPromoteOperands(typeof (ExpressionParser.ILogicalSignatures), token.text, ref left, ref comparison, token.pos);
        left = (Expression) Expression.AndAlso(left, comparison);
      }
      return left;
    }

    private Expression ParseComparison()
    {
      Expression left = this.ParseAdditive();
      while (this.token.id == ExpressionParser.TokenId.Equal || this.token.id == ExpressionParser.TokenId.DoubleEqual || (this.token.id == ExpressionParser.TokenId.ExclamationEqual || this.token.id == ExpressionParser.TokenId.LessGreater) || (this.token.id == ExpressionParser.TokenId.GreaterThan || this.token.id == ExpressionParser.TokenId.GreaterThanEqual || (this.token.id == ExpressionParser.TokenId.LessThan || this.token.id == ExpressionParser.TokenId.LessThanEqual)))
      {
        ExpressionParser.Token token = this.token;
        this.NextToken();
        Expression right = this.ParseAdditive();
        bool flag = token.id == ExpressionParser.TokenId.Equal || token.id == ExpressionParser.TokenId.DoubleEqual || token.id == ExpressionParser.TokenId.ExclamationEqual || token.id == ExpressionParser.TokenId.LessGreater;
        if (flag && !left.Type.IsValueType && !right.Type.IsValueType)
        {
          if (left.Type != right.Type)
          {
            if (left.Type.IsAssignableFrom(right.Type))
            {
              right = (Expression) Expression.Convert(right, left.Type);
            }
            else
            {
              if (!right.Type.IsAssignableFrom(left.Type))
                throw this.IncompatibleOperandsError(token.text, left, right, token.pos);
              left = (Expression) Expression.Convert(left, right.Type);
            }
          }
        }
        else if (ExpressionParser.IsEnumType(left.Type) || ExpressionParser.IsEnumType(right.Type))
        {
          if (left.Type != right.Type)
          {
            Expression expression1;
            if ((expression1 = this.PromoteExpression(right, left.Type, true)) != null)
            {
              right = expression1;
            }
            else
            {
              Expression expression2;
              if ((expression2 = this.PromoteExpression(left, right.Type, true)) == null)
                throw this.IncompatibleOperandsError(token.text, left, right, token.pos);
              left = expression2;
            }
          }
        }
        else
          this.CheckAndPromoteOperands(flag ? typeof (ExpressionParser.IEqualitySignatures) : typeof (ExpressionParser.IRelationalSignatures), token.text, ref left, ref right, token.pos);
        switch (token.id)
        {
          case ExpressionParser.TokenId.LessThan:
            left = this.GenerateLessThan(left, right);
            continue;
          case ExpressionParser.TokenId.Equal:
          case ExpressionParser.TokenId.DoubleEqual:
            left = this.GenerateEqual(left, right);
            continue;
          case ExpressionParser.TokenId.GreaterThan:
            left = this.GenerateGreaterThan(left, right);
            continue;
          case ExpressionParser.TokenId.ExclamationEqual:
          case ExpressionParser.TokenId.LessGreater:
            left = this.GenerateNotEqual(left, right);
            continue;
          case ExpressionParser.TokenId.LessThanEqual:
            left = this.GenerateLessThanEqual(left, right);
            continue;
          case ExpressionParser.TokenId.GreaterThanEqual:
            left = this.GenerateGreaterThanEqual(left, right);
            continue;
          default:
            continue;
        }
      }
      return left;
    }

    private Expression ParseAdditive()
    {
      Expression left = this.ParseMultiplicative();
      while (this.token.id == ExpressionParser.TokenId.Plus || this.token.id == ExpressionParser.TokenId.Minus || this.token.id == ExpressionParser.TokenId.Amphersand)
      {
        ExpressionParser.Token token = this.token;
        this.NextToken();
        Expression multiplicative = this.ParseMultiplicative();
        switch (token.id)
        {
          case ExpressionParser.TokenId.Amphersand:
            left = this.GenerateStringConcat(left, multiplicative);
            continue;
          case ExpressionParser.TokenId.Plus:
            if (!(left.Type == typeof (string)) && !(multiplicative.Type == typeof (string)))
            {
              this.CheckAndPromoteOperands(typeof (ExpressionParser.IAddSignatures), token.text, ref left, ref multiplicative, token.pos);
              left = this.GenerateAdd(left, multiplicative);
              continue;
            }
            goto case ExpressionParser.TokenId.Amphersand;
          case ExpressionParser.TokenId.Minus:
            this.CheckAndPromoteOperands(typeof (ExpressionParser.ISubtractSignatures), token.text, ref left, ref multiplicative, token.pos);
            left = this.GenerateSubtract(left, multiplicative);
            continue;
          default:
            continue;
        }
      }
      return left;
    }

    private Expression ParseMultiplicative()
    {
      Expression left = this.ParseUnary();
      while (this.token.id == ExpressionParser.TokenId.Asterisk || this.token.id == ExpressionParser.TokenId.Slash || (this.token.id == ExpressionParser.TokenId.Percent || this.TokenIdentifierIs("mod")))
      {
        ExpressionParser.Token token = this.token;
        this.NextToken();
        Expression unary = this.ParseUnary();
        this.CheckAndPromoteOperands(typeof (ExpressionParser.IArithmeticSignatures), token.text, ref left, ref unary, token.pos);
        switch (token.id)
        {
          case ExpressionParser.TokenId.Identifier:
          case ExpressionParser.TokenId.Percent:
            left = (Expression) Expression.Modulo(left, unary);
            continue;
          case ExpressionParser.TokenId.Asterisk:
            left = (Expression) Expression.Multiply(left, unary);
            continue;
          case ExpressionParser.TokenId.Slash:
            left = (Expression) Expression.Divide(left, unary);
            continue;
          default:
            continue;
        }
      }
      return left;
    }

    private Expression ParseUnary()
    {
      if (this.token.id != ExpressionParser.TokenId.Minus && this.token.id != ExpressionParser.TokenId.Exclamation && !this.TokenIdentifierIs("not"))
        return this.ParsePrimary();
      ExpressionParser.Token token = this.token;
      this.NextToken();
      if (token.id == ExpressionParser.TokenId.Minus && (this.token.id == ExpressionParser.TokenId.IntegerLiteral || this.token.id == ExpressionParser.TokenId.RealLiteral))
      {
        this.token.text = "-" + this.token.text;
        this.token.pos = token.pos;
        return this.ParsePrimary();
      }
      Expression unary = this.ParseUnary();
      Expression expression;
      if (token.id == ExpressionParser.TokenId.Minus)
      {
        this.CheckAndPromoteOperand(typeof (ExpressionParser.INegationSignatures), token.text, ref unary, token.pos);
        expression = (Expression) Expression.Negate(unary);
      }
      else
      {
        this.CheckAndPromoteOperand(typeof (ExpressionParser.INotSignatures), token.text, ref unary, token.pos);
        expression = (Expression) Expression.Not(unary);
      }
      return expression;
    }

    private Expression ParsePrimary()
    {
      Expression expression = this.ParsePrimaryStart();
      while (true)
      {
        while (this.token.id != ExpressionParser.TokenId.Dot)
        {
          if (this.token.id != ExpressionParser.TokenId.OpenBracket)
            return expression;
          expression = this.ParseElementAccess(expression);
        }
        this.NextToken();
        expression = this.ParseMemberAccess((Type) null, expression);
      }
    }

    private Expression ParsePrimaryStart()
    {
      switch (this.token.id)
      {
        case ExpressionParser.TokenId.Identifier:
          return this.ParseIdentifier();
        case ExpressionParser.TokenId.StringLiteral:
          return this.ParseStringLiteral();
        case ExpressionParser.TokenId.IntegerLiteral:
          return this.ParseIntegerLiteral();
        case ExpressionParser.TokenId.RealLiteral:
          return this.ParseRealLiteral();
        case ExpressionParser.TokenId.OpenParen:
          return this.ParseParenExpression();
        default:
          throw this.ParseError("Expression expected");
      }
    }

    private Expression ParseStringLiteral()
    {
      this.ValidateToken(ExpressionParser.TokenId.StringLiteral);
      char ch = this.token.text[0];
      string text = this.token.text.Substring(1, this.token.text.Length - 2);
      int startIndex1 = 0;
      while (true)
      {
        int startIndex2 = text.IndexOf(ch, startIndex1);
        if (startIndex2 >= 0)
        {
          text = text.Remove(startIndex2, 1);
          startIndex1 = startIndex2 + 1;
        }
        else
          break;
      }
      if (ch == '\'')
      {
        if (text.Length != 1)
          throw this.ParseError("Character literal must contain exactly one character");
        this.NextToken();
        return this.CreateLiteral((object) text[0], text);
      }
      this.NextToken();
      return this.CreateLiteral((object) text, text);
    }

    private Expression ParseIntegerLiteral()
    {
      this.ValidateToken(ExpressionParser.TokenId.IntegerLiteral);
      string text = this.token.text;
      if (text[0] != '-')
      {
        ulong result;
        if (!ulong.TryParse(text, out result))
          throw this.ParseError("Invalid integer literal '{0}'", (object) text);
        this.NextToken();
        if (result <= (ulong) int.MaxValue)
          return this.CreateLiteral((object) (int) result, text);
        if (result <= (ulong) uint.MaxValue)
          return this.CreateLiteral((object) (uint) result, text);
        return result <= (ulong) long.MaxValue ? this.CreateLiteral((object) (long) result, text) : this.CreateLiteral((object) result, text);
      }
      long result1;
      if (!long.TryParse(text, out result1))
        throw this.ParseError("Invalid integer literal '{0}'", (object) text);
      this.NextToken();
      return result1 >= (long) int.MinValue && result1 <= (long) int.MaxValue ? this.CreateLiteral((object) (int) result1, text) : this.CreateLiteral((object) result1, text);
    }

    private Expression ParseRealLiteral()
    {
      this.ValidateToken(ExpressionParser.TokenId.RealLiteral);
      string text = this.token.text;
      object obj = (object) null;
      switch (text[text.Length - 1])
      {
        case 'F':
        case 'f':
          float result1;
          if (float.TryParse(text.Substring(0, text.Length - 1), out result1))
          {
            obj = (object) result1;
            break;
          }
          break;
        default:
          double result2;
          if (double.TryParse(text, out result2))
          {
            obj = (object) result2;
            break;
          }
          break;
      }
      if (obj == null)
        throw this.ParseError("Invalid real literal '{0}'", (object) text);
      this.NextToken();
      return this.CreateLiteral(obj, text);
    }

    private Expression CreateLiteral(object value, string text)
    {
      ConstantExpression constantExpression = Expression.Constant(value);
      this.literals.Add((Expression) constantExpression, text);
      return (Expression) constantExpression;
    }

    private Expression ParseParenExpression()
    {
      this.ValidateToken(ExpressionParser.TokenId.OpenParen, "'(' expected");
      this.NextToken();
      Expression expression = this.ParseExpression();
      this.ValidateToken(ExpressionParser.TokenId.CloseParen, "')' or operator expected");
      this.NextToken();
      return expression;
    }

    private Expression ParseIdentifier()
    {
      this.ValidateToken(ExpressionParser.TokenId.Identifier);
      object obj;
      if (ExpressionParser.keywords.TryGetValue(this.token.text, out obj))
      {
        if ((object) (obj as Type) != null)
          return this.ParseTypeAccess((Type) obj);
        if (obj == (object) ExpressionParser.keywordIt)
          return this.ParseIt();
        if (obj == (object) ExpressionParser.keywordIif)
          return this.ParseIif();
        if (obj == (object) ExpressionParser.keywordNew)
          return this.ParseNew();
        this.NextToken();
        return (Expression) obj;
      }
      if (this.symbols.TryGetValue(this.token.text, out obj) || this.externals != null && this.externals.TryGetValue(this.token.text, out obj))
      {
        if (!(obj is Expression expression2))
          expression2 = (Expression) Expression.Constant(obj);
        else if (expression2 is LambdaExpression lambda4)
          return this.ParseLambdaInvocation(lambda4);
        this.NextToken();
        return expression2;
      }
      return this.it != null ? this.ParseMemberAccess((Type) null, (Expression) this.it) : throw this.ParseError("Unknown identifier '{0}'", (object) this.token.text);
    }

    private Expression ParseIt()
    {
      if (this.it == null)
        throw this.ParseError("No 'it' is in scope");
      this.NextToken();
      return (Expression) this.it;
    }

    private Expression ParseIif()
    {
      int pos = this.token.pos;
      this.NextToken();
      Expression[] argumentList = this.ParseArgumentList();
      return argumentList.Length == 3 ? this.GenerateConditional(argumentList[0], argumentList[1], argumentList[2], pos) : throw this.ParseError(pos, "The 'iif' function requires three arguments");
    }

    private Expression GenerateConditional(
      Expression test,
      Expression expr1,
      Expression expr2,
      int errorPos)
    {
      if (test.Type != typeof (bool))
        throw this.ParseError(errorPos, "The first expression must be of type 'Boolean'");
      if (expr1.Type != expr2.Type)
      {
        Expression expression1 = expr2 != ExpressionParser.nullLiteral ? this.PromoteExpression(expr1, expr2.Type, true) : (Expression) null;
        Expression expression2 = expr1 != ExpressionParser.nullLiteral ? this.PromoteExpression(expr2, expr1.Type, true) : (Expression) null;
        if (expression1 != null && expression2 == null)
          expr1 = expression1;
        else if (expression2 != null && expression1 == null)
        {
          expr2 = expression2;
        }
        else
        {
          string str1 = expr1 != ExpressionParser.nullLiteral ? expr1.Type.Name : "null";
          string str2 = expr2 != ExpressionParser.nullLiteral ? expr2.Type.Name : "null";
          if (expression1 != null && expression2 != null)
            throw this.ParseError(errorPos, "Both of the types '{0}' and '{1}' convert to the other", (object) str1, (object) str2);
          throw this.ParseError(errorPos, "Neither of the types '{0}' and '{1}' converts to the other", (object) str1, (object) str2);
        }
      }
      return (Expression) Expression.Condition(test, expr1, expr2);
    }

    private Expression ParseNew()
    {
      this.NextToken();
      this.ValidateToken(ExpressionParser.TokenId.OpenParen, "'(' expected");
      this.NextToken();
      List<DynamicProperty> dynamicPropertyList = new List<DynamicProperty>();
      List<Expression> expressionList = new List<Expression>();
      int pos;
      while (true)
      {
        pos = this.token.pos;
        Expression expression = this.ParseExpression();
        string name;
        if (this.TokenIdentifierIs("as"))
        {
          this.NextToken();
          name = this.GetIdentifier();
          this.NextToken();
        }
        else if (expression is MemberExpression memberExpression4)
          name = memberExpression4.Member.Name;
        else
          break;
        expressionList.Add(expression);
        dynamicPropertyList.Add(new DynamicProperty(name, expression.Type));
        if (this.token.id == ExpressionParser.TokenId.Comma)
          this.NextToken();
        else
          goto label_8;
      }
      throw this.ParseError(pos, "Expression is missing an 'as' clause");
label_8:
      this.ValidateToken(ExpressionParser.TokenId.CloseParen, "')' or ',' expected");
      this.NextToken();
      Type type = DynamicExpressionEx.CreateClass((IEnumerable<DynamicProperty>) dynamicPropertyList);
      MemberBinding[] memberBindingArray = new MemberBinding[dynamicPropertyList.Count];
      for (int index = 0; index < memberBindingArray.Length; ++index)
        memberBindingArray[index] = (MemberBinding) Expression.Bind((MemberInfo) type.GetProperty(dynamicPropertyList[index].Name), expressionList[index]);
      return (Expression) Expression.MemberInit(Expression.New(type), memberBindingArray);
    }

    private Expression ParseLambdaInvocation(LambdaExpression lambda)
    {
      int pos = this.token.pos;
      this.NextToken();
      Expression[] argumentList = this.ParseArgumentList();
      if (this.FindMethod(lambda.Type, "Invoke", false, argumentList, out MethodBase _) != 1)
        throw this.ParseError(pos, "Argument list incompatible with lambda expression");
      return (Expression) Expression.Invoke((Expression) lambda, argumentList);
    }

    private Expression ParseTypeAccess(Type type)
    {
      int pos = this.token.pos;
      this.NextToken();
      if (this.token.id == ExpressionParser.TokenId.Question)
      {
        if (!type.IsValueType || ExpressionParser.IsNullableType(type))
          throw this.ParseError(pos, "Type '{0}' has no nullable form", (object) ExpressionParser.GetTypeName(type));
        type = typeof (Nullable<>).MakeGenericType(type);
        this.NextToken();
      }
      if (this.token.id == ExpressionParser.TokenId.OpenParen)
      {
        Expression[] argumentList = this.ParseArgumentList();
        MethodBase method;
        switch (this.FindBestMethod((IEnumerable<MethodBase>) type.GetConstructors(), argumentList, out method))
        {
          case 0:
            if (argumentList.Length == 1)
              return this.GenerateConversion(argumentList[0], type, pos);
            throw this.ParseError(pos, "No matching constructor in type '{0}'", (object) ExpressionParser.GetTypeName(type));
          case 1:
            return (Expression) Expression.New((ConstructorInfo) method, argumentList);
          default:
            throw this.ParseError(pos, "Ambiguous invocation of '{0}' constructor", (object) ExpressionParser.GetTypeName(type));
        }
      }
      else
      {
        this.ValidateToken(ExpressionParser.TokenId.Dot, "'.' or '(' expected");
        this.NextToken();
        return this.ParseMemberAccess(type, (Expression) null);
      }
    }

    private Expression GenerateConversion(Expression expr, Type type, int errorPos)
    {
      Type type1 = expr.Type;
      if (type1 == type)
        return expr;
      if (type1.IsValueType && type.IsValueType)
      {
        if ((ExpressionParser.IsNullableType(type1) || ExpressionParser.IsNullableType(type)) && ExpressionParser.GetNonNullableType(type1) == ExpressionParser.GetNonNullableType(type))
          return (Expression) Expression.Convert(expr, type);
        if ((ExpressionParser.IsNumericType(type1) || ExpressionParser.IsEnumType(type1)) && ExpressionParser.IsNumericType(type) || ExpressionParser.IsEnumType(type))
          return (Expression) Expression.ConvertChecked(expr, type);
      }
      if (type1.IsAssignableFrom(type) || type.IsAssignableFrom(type1) || (type1.IsInterface || type.IsInterface))
        return (Expression) Expression.Convert(expr, type);
      throw this.ParseError(errorPos, "A value of type '{0}' cannot be converted to type '{1}'", (object) ExpressionParser.GetTypeName(type1), (object) ExpressionParser.GetTypeName(type));
    }

    private Expression ParseMemberAccess(Type type, Expression instance)
    {
      if (instance != null)
        type = instance.Type;
      int pos = this.token.pos;
      string identifier = this.GetIdentifier();
      this.NextToken();
      if (this.token.id == ExpressionParser.TokenId.OpenParen)
      {
        if (instance != null && type != typeof (string))
        {
          Type genericType = ExpressionParser.FindGenericType(typeof (IEnumerable<>), type);
          if (genericType != (Type) null)
          {
            Type genericArgument = genericType.GetGenericArguments()[0];
            return this.ParseAggregate(instance, genericArgument, identifier, pos);
          }
        }
        Expression[] argumentList = this.ParseArgumentList();
        MethodBase method1;
        switch (this.FindMethod(type, identifier, instance == null, argumentList, out method1))
        {
          case 0:
            throw this.ParseError(pos, "No applicable method '{0}' exists in type '{1}'", (object) identifier, (object) ExpressionParser.GetTypeName(type));
          case 1:
            MethodInfo method2 = (MethodInfo) method1;
            if (!ExpressionParser.IsPredefinedType(method2.DeclaringType))
              throw this.ParseError(pos, "Methods on type '{0}' are not accessible", (object) ExpressionParser.GetTypeName(method2.DeclaringType));
            if (method2.ReturnType == typeof (void))
              throw this.ParseError(pos, "Method '{0}' in type '{1}' does not return a value", (object) identifier, (object) ExpressionParser.GetTypeName(method2.DeclaringType));
            return (Expression) Expression.Call(instance, method2, argumentList);
          default:
            throw this.ParseError(pos, "Ambiguous invocation of method '{0}' in type '{1}'", (object) identifier, (object) ExpressionParser.GetTypeName(type));
        }
      }
      else
      {
        MemberInfo propertyOrField = this.FindPropertyOrField(type, identifier, instance == null);
        if (propertyOrField == (MemberInfo) null)
          throw this.ParseError(pos, "No property or field '{0}' exists in type '{1}'", (object) identifier, (object) ExpressionParser.GetTypeName(type));
        return (object) (propertyOrField as PropertyInfo) == null ? (Expression) Expression.Field(instance, (FieldInfo) propertyOrField) : (Expression) Expression.Property(instance, (PropertyInfo) propertyOrField);
      }
    }

    private static Type FindGenericType(Type generic, Type type)
    {
      for (; type != (Type) null && type != typeof (object); type = type.BaseType)
      {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == generic)
          return type;
        if (generic.IsInterface)
        {
          foreach (Type type1 in type.GetInterfaces())
          {
            Type genericType = ExpressionParser.FindGenericType(generic, type1);
            if (genericType != (Type) null)
              return genericType;
          }
        }
      }
      return (Type) null;
    }

    private Expression ParseAggregate(
      Expression instance,
      Type elementType,
      string methodName,
      int errorPos)
    {
      ParameterExpression it = this.it;
      ParameterExpression parameterExpression = Expression.Parameter(elementType, "");
      this.it = parameterExpression;
      Expression[] argumentList = this.ParseArgumentList();
      this.it = it;
      MethodBase method;
      if (this.FindMethod(typeof (ExpressionParser.IEnumerableSignatures), methodName, false, argumentList, out method) != 1)
        throw this.ParseError(errorPos, "No applicable aggregate method '{0}' exists", (object) methodName);
      Type[] typeArguments;
      if (method.Name == "Min" || method.Name == "Max")
        typeArguments = new Type[2]
        {
          elementType,
          argumentList[0].Type
        };
      else
        typeArguments = new Type[1]{ elementType };
      Expression[] expressionArray;
      if (argumentList.Length == 0)
        expressionArray = new Expression[1]{ instance };
      else
        expressionArray = new Expression[2]
        {
          instance,
          (Expression) Expression.Lambda(argumentList[0], parameterExpression)
        };
      return (Expression) Expression.Call(typeof (Enumerable), method.Name, typeArguments, expressionArray);
    }

    private Expression[] ParseArgumentList()
    {
      this.ValidateToken(ExpressionParser.TokenId.OpenParen, "'(' expected");
      this.NextToken();
      Expression[] expressionArray = this.token.id != ExpressionParser.TokenId.CloseParen ? this.ParseArguments() : new Expression[0];
      this.ValidateToken(ExpressionParser.TokenId.CloseParen, "')' or ',' expected");
      this.NextToken();
      return expressionArray;
    }

    private Expression[] ParseArguments()
    {
      List<Expression> expressionList = new List<Expression>();
      while (true)
      {
        expressionList.Add(this.ParseExpression());
        if (this.token.id == ExpressionParser.TokenId.Comma)
          this.NextToken();
        else
          break;
      }
      return expressionList.ToArray();
    }

    private Expression ParseElementAccess(Expression expr)
    {
      int pos = this.token.pos;
      this.ValidateToken(ExpressionParser.TokenId.OpenBracket, "'(' expected");
      this.NextToken();
      Expression[] arguments = this.ParseArguments();
      this.ValidateToken(ExpressionParser.TokenId.CloseBracket, "']' or ',' expected");
      this.NextToken();
      if (expr.Type.IsArray)
      {
        if (expr.Type.GetArrayRank() != 1 || arguments.Length != 1)
          throw this.ParseError(pos, "Indexing of multi-dimensional arrays is not supported");
        return (Expression) Expression.ArrayIndex(expr, this.PromoteExpression(arguments[0], typeof (int), true) ?? throw this.ParseError(pos, "Array index must be an integer expression"));
      }
      MethodBase method;
      switch (this.FindIndexer(expr.Type, arguments, out method))
      {
        case 0:
          throw this.ParseError(pos, "No applicable indexer exists in type '{0}'", (object) ExpressionParser.GetTypeName(expr.Type));
        case 1:
          return (Expression) Expression.Call(expr, (MethodInfo) method, arguments);
        default:
          throw this.ParseError(pos, "Ambiguous invocation of indexer in type '{0}'", (object) ExpressionParser.GetTypeName(expr.Type));
      }
    }

    private static bool IsPredefinedType(Type type)
    {
      foreach (Type predefinedType in ExpressionParser.predefinedTypes)
      {
        if (predefinedType == type)
          return true;
      }
      return false;
    }

    private static bool IsNullableType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>);

    private static Type GetNonNullableType(Type type) => !ExpressionParser.IsNullableType(type) ? type : type.GetGenericArguments()[0];

    private static string GetTypeName(Type type)
    {
      Type nonNullableType = ExpressionParser.GetNonNullableType(type);
      string name = nonNullableType.Name;
      if (type != nonNullableType)
        name += (string) (object) '?';
      return name;
    }

    private static bool IsNumericType(Type type) => ExpressionParser.GetNumericTypeKind(type) != 0;

    private static bool IsSignedIntegralType(Type type) => ExpressionParser.GetNumericTypeKind(type) == 2;

    private static bool IsUnsignedIntegralType(Type type) => ExpressionParser.GetNumericTypeKind(type) == 3;

    private static int GetNumericTypeKind(Type type)
    {
      type = ExpressionParser.GetNonNullableType(type);
      if (type.IsEnum)
        return 0;
      switch (Type.GetTypeCode(type))
      {
        case TypeCode.Char:
        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
          return 1;
        case TypeCode.SByte:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
          return 2;
        case TypeCode.Byte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
          return 3;
        default:
          return 0;
      }
    }

    private static bool IsEnumType(Type type) => ExpressionParser.GetNonNullableType(type).IsEnum;

    private void CheckAndPromoteOperand(
      Type signatures,
      string opName,
      ref Expression expr,
      int errorPos)
    {
      Expression[] args = new Expression[1]
      {
        expr
      };
      expr = this.FindMethod(signatures, "F", false, args, out MethodBase _) == 1 ? args[0] : throw this.ParseError(errorPos, "Operator '{0}' incompatible with operand type '{1}'", (object) opName, (object) ExpressionParser.GetTypeName(args[0].Type));
    }

    private void CheckAndPromoteOperands(
      Type signatures,
      string opName,
      ref Expression left,
      ref Expression right,
      int errorPos)
    {
      Expression[] args = new Expression[2]
      {
        left,
        right
      };
      left = this.FindMethod(signatures, "F", false, args, out MethodBase _) == 1 ? args[0] : throw this.IncompatibleOperandsError(opName, left, right, errorPos);
      right = args[1];
    }

    private Exception IncompatibleOperandsError(
      string opName,
      Expression left,
      Expression right,
      int pos)
    {
      return this.ParseError(pos, "Operator '{0}' incompatible with operand types '{1}' and '{2}'", (object) opName, (object) ExpressionParser.GetTypeName(left.Type), (object) ExpressionParser.GetTypeName(right.Type));
    }

    private MemberInfo FindPropertyOrField(
      Type type,
      string memberName,
      bool staticAccess)
    {
      BindingFlags bindingAttr = (BindingFlags) (18 | (staticAccess ? 8 : 4));
      foreach (Type selfAndBaseType in ExpressionParser.SelfAndBaseTypes(type))
      {
        MemberInfo[] members = selfAndBaseType.FindMembers(MemberTypes.Field | MemberTypes.Property, bindingAttr, Type.FilterNameIgnoreCase, (object) memberName);
        if (members.Length != 0)
          return members[0];
      }
      return (MemberInfo) null;
    }

    private int FindMethod(
      Type type,
      string methodName,
      bool staticAccess,
      Expression[] args,
      out MethodBase method)
    {
      BindingFlags bindingAttr = (BindingFlags) (18 | (staticAccess ? 8 : 4));
      foreach (Type selfAndBaseType in ExpressionParser.SelfAndBaseTypes(type))
      {
        int bestMethod = this.FindBestMethod(selfAndBaseType.FindMembers(MemberTypes.Method, bindingAttr, Type.FilterNameIgnoreCase, (object) methodName).Cast<MethodBase>(), args, out method);
        if (bestMethod != 0)
          return bestMethod;
      }
      method = (MethodBase) null;
      return 0;
    }

    private int FindIndexer(Type type, Expression[] args, out MethodBase method)
    {
      foreach (Type selfAndBaseType in ExpressionParser.SelfAndBaseTypes(type))
      {
        MemberInfo[] defaultMembers = selfAndBaseType.GetDefaultMembers();
        if (defaultMembers.Length != 0)
        {
          int bestMethod = this.FindBestMethod(defaultMembers.OfType<PropertyInfo>().Select<PropertyInfo, MethodBase>((Func<PropertyInfo, MethodBase>) (p => (MethodBase) p.GetGetMethod())).Where<MethodBase>((Func<MethodBase, bool>) (m => m != (MethodBase) null)), args, out method);
          if (bestMethod != 0)
            return bestMethod;
        }
      }
      method = (MethodBase) null;
      return 0;
    }

    private static IEnumerable<Type> SelfAndBaseTypes(Type type)
    {
      if (!type.IsInterface)
        return ExpressionParser.SelfAndBaseClasses(type);
      List<Type> types = new List<Type>();
      ExpressionParser.AddInterface(types, type);
      return (IEnumerable<Type>) types;
    }

    private static IEnumerable<Type> SelfAndBaseClasses(Type type)
    {
      for (; type != (Type) null; type = type.BaseType)
        yield return type;
    }

    private static void AddInterface(List<Type> types, Type type)
    {
      if (types.Contains(type))
        return;
      types.Add(type);
      foreach (Type type1 in type.GetInterfaces())
        ExpressionParser.AddInterface(types, type1);
    }

    private int FindBestMethod(
      IEnumerable<MethodBase> methods,
      Expression[] args,
      out MethodBase method)
    {
      ExpressionParser.MethodData[] applicable = methods.Select<MethodBase, ExpressionParser.MethodData>((Func<MethodBase, ExpressionParser.MethodData>) (m => new ExpressionParser.MethodData()
      {
        MethodBase = m,
        Parameters = m.GetParameters()
      })).Where<ExpressionParser.MethodData>((Func<ExpressionParser.MethodData, bool>) (m => this.IsApplicable(m, args))).ToArray<ExpressionParser.MethodData>();
      if (applicable.Length > 1)
        applicable = ((IEnumerable<ExpressionParser.MethodData>) applicable).Where<ExpressionParser.MethodData>((Func<ExpressionParser.MethodData, bool>) (m => ((IEnumerable<ExpressionParser.MethodData>) applicable).All<ExpressionParser.MethodData>((Func<ExpressionParser.MethodData, bool>) (n => m == n || ExpressionParser.IsBetterThan(args, m, n))))).ToArray<ExpressionParser.MethodData>();
      if (applicable.Length == 1)
      {
        ExpressionParser.MethodData methodData = applicable[0];
        for (int index = 0; index < args.Length; ++index)
          args[index] = methodData.Args[index];
        method = methodData.MethodBase;
      }
      else
        method = (MethodBase) null;
      return applicable.Length;
    }

    private bool IsApplicable(ExpressionParser.MethodData method, Expression[] args)
    {
      if (method.Parameters.Length != args.Length)
        return false;
      Expression[] expressionArray = new Expression[args.Length];
      for (int index = 0; index < args.Length; ++index)
      {
        ParameterInfo parameter = method.Parameters[index];
        if (parameter.IsOut)
          return false;
        Expression expression = this.PromoteExpression(args[index], parameter.ParameterType, false);
        if (expression == null)
          return false;
        expressionArray[index] = expression;
      }
      method.Args = expressionArray;
      return true;
    }

    private Expression PromoteExpression(Expression expr, Type type, bool exact)
    {
      if (expr.Type == type)
        return expr;
      if (expr is ConstantExpression)
      {
        ConstantExpression constantExpression = (ConstantExpression) expr;
        if (constantExpression == ExpressionParser.nullLiteral)
        {
          if (!type.IsValueType || ExpressionParser.IsNullableType(type))
            return (Expression) Expression.Constant((object) null, type);
        }
        else
        {
          string str;
          if (this.literals.TryGetValue((Expression) constantExpression, out str))
          {
            Type nonNullableType = ExpressionParser.GetNonNullableType(type);
            object obj = (object) null;
            switch (Type.GetTypeCode(constantExpression.Type))
            {
              case TypeCode.Int32:
              case TypeCode.UInt32:
              case TypeCode.Int64:
              case TypeCode.UInt64:
                obj = ExpressionParser.ParseNumber(str, nonNullableType);
                break;
              case TypeCode.Double:
                if (nonNullableType == typeof (Decimal))
                {
                  obj = ExpressionParser.ParseNumber(str, nonNullableType);
                  break;
                }
                break;
              case TypeCode.String:
                obj = ExpressionParser.ParseEnum(str, nonNullableType);
                break;
            }
            if (obj != null)
              return (Expression) Expression.Constant(obj, type);
          }
        }
      }
      if (!ExpressionParser.IsCompatibleWith(expr.Type, type))
        return (Expression) null;
      return type.IsValueType || exact ? (Expression) Expression.Convert(expr, type) : expr;
    }

    private static object ParseNumber(string text, Type type)
    {
      switch (Type.GetTypeCode(ExpressionParser.GetNonNullableType(type)))
      {
        case TypeCode.SByte:
          sbyte result1;
          if (sbyte.TryParse(text, out result1))
            return (object) result1;
          break;
        case TypeCode.Byte:
          byte result2;
          if (byte.TryParse(text, out result2))
            return (object) result2;
          break;
        case TypeCode.Int16:
          short result3;
          if (short.TryParse(text, out result3))
            return (object) result3;
          break;
        case TypeCode.UInt16:
          ushort result4;
          if (ushort.TryParse(text, out result4))
            return (object) result4;
          break;
        case TypeCode.Int32:
          int result5;
          if (int.TryParse(text, out result5))
            return (object) result5;
          break;
        case TypeCode.UInt32:
          uint result6;
          if (uint.TryParse(text, out result6))
            return (object) result6;
          break;
        case TypeCode.Int64:
          long result7;
          if (long.TryParse(text, out result7))
            return (object) result7;
          break;
        case TypeCode.UInt64:
          ulong result8;
          if (ulong.TryParse(text, out result8))
            return (object) result8;
          break;
        case TypeCode.Single:
          float result9;
          if (float.TryParse(text, out result9))
            return (object) result9;
          break;
        case TypeCode.Double:
          double result10;
          if (double.TryParse(text, out result10))
            return (object) result10;
          break;
        case TypeCode.Decimal:
          Decimal result11;
          if (Decimal.TryParse(text, out result11))
            return (object) result11;
          break;
      }
      return (object) null;
    }

    private static object ParseEnum(string name, Type type)
    {
      if (type.IsEnum)
      {
        MemberInfo[] members = type.FindMembers(MemberTypes.Field, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public, Type.FilterNameIgnoreCase, (object) name);
        if (members.Length != 0)
          return ((FieldInfo) members[0]).GetValue((object) null);
      }
      return (object) null;
    }

    private static bool IsCompatibleWith(Type source, Type target)
    {
      if (source == target)
        return true;
      if (!target.IsValueType)
        return target.IsAssignableFrom(source);
      Type nonNullableType1 = ExpressionParser.GetNonNullableType(source);
      Type nonNullableType2 = ExpressionParser.GetNonNullableType(target);
      if (nonNullableType1 != source && nonNullableType2 == target)
        return false;
      TypeCode typeCode1 = nonNullableType1.IsEnum ? TypeCode.Object : Type.GetTypeCode(nonNullableType1);
      TypeCode typeCode2 = nonNullableType2.IsEnum ? TypeCode.Object : Type.GetTypeCode(nonNullableType2);
      switch (typeCode1)
      {
        case TypeCode.SByte:
          switch (typeCode2)
          {
            case TypeCode.SByte:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.Byte:
          switch (typeCode2)
          {
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.Int16:
          switch (typeCode2)
          {
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.UInt16:
          switch (typeCode2)
          {
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.Int32:
          switch (typeCode2)
          {
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.UInt32:
          switch (typeCode2)
          {
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.Int64:
          switch (typeCode2)
          {
            case TypeCode.Int64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.UInt64:
          switch (typeCode2)
          {
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
              return true;
          }
          break;
        case TypeCode.Single:
          switch (typeCode2)
          {
            case TypeCode.Single:
            case TypeCode.Double:
              return true;
          }
          break;
        default:
          if (nonNullableType1 == nonNullableType2)
            return true;
          break;
      }
      return false;
    }

    private static bool IsBetterThan(
      Expression[] args,
      ExpressionParser.MethodData m1,
      ExpressionParser.MethodData m2)
    {
      bool flag = false;
      for (int index = 0; index < args.Length; ++index)
      {
        int num = ExpressionParser.CompareConversions(args[index].Type, m1.Parameters[index].ParameterType, m2.Parameters[index].ParameterType);
        if (num < 0)
          return false;
        if (num > 0)
          flag = true;
      }
      return flag;
    }

    private static int CompareConversions(Type s, Type t1, Type t2)
    {
      if (t1 == t2)
        return 0;
      if (s == t1)
        return 1;
      if (s == t2)
        return -1;
      bool flag1 = ExpressionParser.IsCompatibleWith(t1, t2);
      bool flag2 = ExpressionParser.IsCompatibleWith(t2, t1);
      if (flag1 && !flag2)
        return 1;
      if (flag2 && !flag1)
        return -1;
      if (ExpressionParser.IsSignedIntegralType(t1) && ExpressionParser.IsUnsignedIntegralType(t2))
        return 1;
      return ExpressionParser.IsSignedIntegralType(t2) && ExpressionParser.IsUnsignedIntegralType(t1) ? -1 : 0;
    }

    private Expression GenerateEqual(Expression left, Expression right) => (Expression) Expression.Equal(left, right);

    private Expression GenerateNotEqual(Expression left, Expression right) => (Expression) Expression.NotEqual(left, right);

    private Expression GenerateGreaterThan(Expression left, Expression right) => left.Type == typeof (string) ? (Expression) Expression.GreaterThan(this.GenerateStaticMethodCall("Compare", left, right), (Expression) Expression.Constant((object) 0)) : (Expression) Expression.GreaterThan(left, right);

    private Expression GenerateGreaterThanEqual(Expression left, Expression right) => left.Type == typeof (string) ? (Expression) Expression.GreaterThanOrEqual(this.GenerateStaticMethodCall("Compare", left, right), (Expression) Expression.Constant((object) 0)) : (Expression) Expression.GreaterThanOrEqual(left, right);

    private Expression GenerateLessThan(Expression left, Expression right) => left.Type == typeof (string) ? (Expression) Expression.LessThan(this.GenerateStaticMethodCall("Compare", left, right), (Expression) Expression.Constant((object) 0)) : (Expression) Expression.LessThan(left, right);

    private Expression GenerateLessThanEqual(Expression left, Expression right) => left.Type == typeof (string) ? (Expression) Expression.LessThanOrEqual(this.GenerateStaticMethodCall("Compare", left, right), (Expression) Expression.Constant((object) 0)) : (Expression) Expression.LessThanOrEqual(left, right);

    private Expression GenerateAdd(Expression left, Expression right) => left.Type == typeof (string) && right.Type == typeof (string) ? this.GenerateStaticMethodCall("Concat", left, right) : (Expression) Expression.Add(left, right);

    private Expression GenerateSubtract(Expression left, Expression right) => (Expression) Expression.Subtract(left, right);

    private Expression GenerateStringConcat(Expression left, Expression right) => (Expression) Expression.Call((Expression) null, typeof (string).GetMethod("Concat", new Type[2]
    {
      typeof (object),
      typeof (object)
    }), new Expression[2]{ left, right });

    private MethodInfo GetStaticMethod(
      string methodName,
      Expression left,
      Expression right)
    {
      return left.Type.GetMethod(methodName, new Type[2]
      {
        left.Type,
        right.Type
      });
    }

    private Expression GenerateStaticMethodCall(
      string methodName,
      Expression left,
      Expression right)
    {
      return (Expression) Expression.Call((Expression) null, this.GetStaticMethod(methodName, left, right), new Expression[2]
      {
        left,
        right
      });
    }

    private void SetTextPos(int pos)
    {
      this.textPos = pos;
      this.ch = this.textPos < this.textLen ? this.text[this.textPos] : char.MinValue;
    }

    private void NextChar()
    {
      if (this.textPos < this.textLen)
        ++this.textPos;
      this.ch = this.textPos < this.textLen ? this.text[this.textPos] : char.MinValue;
    }

    private void NextToken()
    {
      while (char.IsWhiteSpace(this.ch))
        this.NextChar();
      int textPos = this.textPos;
      ExpressionParser.TokenId tokenId;
      switch (this.ch)
      {
        case '!':
          this.NextChar();
          if (this.ch == '=')
          {
            this.NextChar();
            tokenId = ExpressionParser.TokenId.ExclamationEqual;
            break;
          }
          tokenId = ExpressionParser.TokenId.Exclamation;
          break;
        case '"':
        case '\'':
          char ch = this.ch;
          do
          {
            this.NextChar();
            while (this.textPos < this.textLen && (int) this.ch != (int) ch)
              this.NextChar();
            if (this.textPos == this.textLen)
              throw this.ParseError(this.textPos, "Unterminated string literal");
            this.NextChar();
          }
          while ((int) this.ch == (int) ch);
          tokenId = ExpressionParser.TokenId.StringLiteral;
          break;
        case '%':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Percent;
          break;
        case '&':
          this.NextChar();
          if (this.ch == '&')
          {
            this.NextChar();
            tokenId = ExpressionParser.TokenId.DoubleAmphersand;
            break;
          }
          tokenId = ExpressionParser.TokenId.Amphersand;
          break;
        case '(':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.OpenParen;
          break;
        case ')':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.CloseParen;
          break;
        case '*':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Asterisk;
          break;
        case '+':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Plus;
          break;
        case ',':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Comma;
          break;
        case '-':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Minus;
          break;
        case '.':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Dot;
          break;
        case '/':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Slash;
          break;
        case ':':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Colon;
          break;
        case '<':
          this.NextChar();
          if (this.ch == '=')
          {
            this.NextChar();
            tokenId = ExpressionParser.TokenId.LessThanEqual;
            break;
          }
          if (this.ch == '>')
          {
            this.NextChar();
            tokenId = ExpressionParser.TokenId.LessGreater;
            break;
          }
          tokenId = ExpressionParser.TokenId.LessThan;
          break;
        case '=':
          this.NextChar();
          if (this.ch == '=')
          {
            this.NextChar();
            tokenId = ExpressionParser.TokenId.DoubleEqual;
            break;
          }
          tokenId = ExpressionParser.TokenId.Equal;
          break;
        case '>':
          this.NextChar();
          if (this.ch == '=')
          {
            this.NextChar();
            tokenId = ExpressionParser.TokenId.GreaterThanEqual;
            break;
          }
          tokenId = ExpressionParser.TokenId.GreaterThan;
          break;
        case '?':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.Question;
          break;
        case '[':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.OpenBracket;
          break;
        case ']':
          this.NextChar();
          tokenId = ExpressionParser.TokenId.CloseBracket;
          break;
        case '|':
          this.NextChar();
          if (this.ch == '|')
          {
            this.NextChar();
            tokenId = ExpressionParser.TokenId.DoubleBar;
            break;
          }
          tokenId = ExpressionParser.TokenId.Bar;
          break;
        default:
          if (char.IsLetter(this.ch) || this.ch == '@' || this.ch == '_')
          {
            do
            {
              this.NextChar();
            }
            while (char.IsLetterOrDigit(this.ch) || this.ch == '_');
            tokenId = ExpressionParser.TokenId.Identifier;
            break;
          }
          if (char.IsDigit(this.ch))
          {
            tokenId = ExpressionParser.TokenId.IntegerLiteral;
            do
            {
              this.NextChar();
            }
            while (char.IsDigit(this.ch));
            if (this.ch == '.')
            {
              tokenId = ExpressionParser.TokenId.RealLiteral;
              this.NextChar();
              this.ValidateDigit();
              do
              {
                this.NextChar();
              }
              while (char.IsDigit(this.ch));
            }
            if (this.ch == 'E' || this.ch == 'e')
            {
              tokenId = ExpressionParser.TokenId.RealLiteral;
              this.NextChar();
              if (this.ch == '+' || this.ch == '-')
                this.NextChar();
              this.ValidateDigit();
              do
              {
                this.NextChar();
              }
              while (char.IsDigit(this.ch));
            }
            if (this.ch == 'F' || this.ch == 'f')
            {
              this.NextChar();
              break;
            }
            break;
          }
          if (this.textPos == this.textLen)
          {
            tokenId = ExpressionParser.TokenId.End;
            break;
          }
          throw this.ParseError(this.textPos, "Syntax error '{0}'", (object) this.ch);
      }
      this.token.id = tokenId;
      this.token.text = this.text.Substring(textPos, this.textPos - textPos);
      this.token.pos = textPos;
    }

    private bool TokenIdentifierIs(string id) => this.token.id == ExpressionParser.TokenId.Identifier && string.Equals(id, this.token.text, StringComparison.OrdinalIgnoreCase);

    private string GetIdentifier()
    {
      this.ValidateToken(ExpressionParser.TokenId.Identifier, "Identifier expected");
      string str = this.token.text;
      if (str.Length > 1 && str[0] == '@')
        str = str.Substring(1);
      return str;
    }

    private void ValidateDigit()
    {
      if (!char.IsDigit(this.ch))
        throw this.ParseError(this.textPos, "Digit expected");
    }

    private void ValidateToken(ExpressionParser.TokenId t, string errorMessage)
    {
      if (this.token.id != t)
        throw this.ParseError(errorMessage);
    }

    private void ValidateToken(ExpressionParser.TokenId t)
    {
      if (this.token.id != t)
        throw this.ParseError("Syntax error");
    }

    private Exception ParseError(string format, params object[] args) => this.ParseError(this.token.pos, format, args);

    private Exception ParseError(int pos, string format, params object[] args) => (Exception) new ParseException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args), pos);

    private static Dictionary<string, object> CreateKeywords()
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dictionary.Add("true", (object) ExpressionParser.trueLiteral);
      dictionary.Add("false", (object) ExpressionParser.falseLiteral);
      dictionary.Add("null", (object) ExpressionParser.nullLiteral);
      dictionary.Add(ExpressionParser.keywordIt, (object) ExpressionParser.keywordIt);
      dictionary.Add(ExpressionParser.keywordIif, (object) ExpressionParser.keywordIif);
      dictionary.Add(ExpressionParser.keywordNew, (object) ExpressionParser.keywordNew);
      foreach (Type predefinedType in ExpressionParser.predefinedTypes)
        dictionary.Add(predefinedType.Name, (object) predefinedType);
      return dictionary;
    }

    private struct Token
    {
      public ExpressionParser.TokenId id;
      public string text;
      public int pos;
    }

    private enum TokenId
    {
      Unknown,
      End,
      Identifier,
      StringLiteral,
      IntegerLiteral,
      RealLiteral,
      Exclamation,
      Percent,
      Amphersand,
      OpenParen,
      CloseParen,
      Asterisk,
      Plus,
      Comma,
      Minus,
      Dot,
      Slash,
      Colon,
      LessThan,
      Equal,
      GreaterThan,
      Question,
      OpenBracket,
      CloseBracket,
      Bar,
      ExclamationEqual,
      DoubleAmphersand,
      LessThanEqual,
      LessGreater,
      DoubleEqual,
      GreaterThanEqual,
      DoubleBar,
    }

    private interface ILogicalSignatures
    {
      void F(bool x, bool y);

      void F(bool? x, bool? y);
    }

    private interface IArithmeticSignatures
    {
      void F(int x, int y);

      void F(uint x, uint y);

      void F(long x, long y);

      void F(ulong x, ulong y);

      void F(float x, float y);

      void F(double x, double y);

      void F(Decimal x, Decimal y);

      void F(int? x, int? y);

      void F(uint? x, uint? y);

      void F(long? x, long? y);

      void F(ulong? x, ulong? y);

      void F(float? x, float? y);

      void F(double? x, double? y);

      void F(Decimal? x, Decimal? y);
    }

    private interface IRelationalSignatures : ExpressionParser.IArithmeticSignatures
    {
      void F(string x, string y);

      void F(char x, char y);

      void F(DateTime x, DateTime y);

      void F(TimeSpan x, TimeSpan y);

      void F(char? x, char? y);

      void F(DateTime? x, DateTime? y);

      void F(TimeSpan? x, TimeSpan? y);
    }

    private interface IEqualitySignatures : 
      ExpressionParser.IRelationalSignatures,
      ExpressionParser.IArithmeticSignatures
    {
      void F(bool x, bool y);

      void F(bool? x, bool? y);
    }

    private interface IAddSignatures : ExpressionParser.IArithmeticSignatures
    {
      void F(DateTime x, TimeSpan y);

      void F(TimeSpan x, TimeSpan y);

      void F(DateTime? x, TimeSpan? y);

      void F(TimeSpan? x, TimeSpan? y);
    }

    private interface ISubtractSignatures : 
      ExpressionParser.IAddSignatures,
      ExpressionParser.IArithmeticSignatures
    {
      void F(DateTime x, DateTime y);

      void F(DateTime? x, DateTime? y);
    }

    private interface INegationSignatures
    {
      void F(int x);

      void F(long x);

      void F(float x);

      void F(double x);

      void F(Decimal x);

      void F(int? x);

      void F(long? x);

      void F(float? x);

      void F(double? x);

      void F(Decimal? x);
    }

    private interface INotSignatures
    {
      void F(bool x);

      void F(bool? x);
    }

    private interface IEnumerableSignatures
    {
      void Where(bool predicate);

      void Any();

      void Any(bool predicate);

      void All(bool predicate);

      void Count();

      void Count(bool predicate);

      void Min(object selector);

      void Max(object selector);

      void Sum(int selector);

      void Sum(int? selector);

      void Sum(long selector);

      void Sum(long? selector);

      void Sum(float selector);

      void Sum(float? selector);

      void Sum(double selector);

      void Sum(double? selector);

      void Sum(Decimal selector);

      void Sum(Decimal? selector);

      void Average(int selector);

      void Average(int? selector);

      void Average(long selector);

      void Average(long? selector);

      void Average(float selector);

      void Average(float? selector);

      void Average(double selector);

      void Average(double? selector);

      void Average(Decimal selector);

      void Average(Decimal? selector);
    }

    private class MethodData
    {
      public MethodBase MethodBase;
      public ParameterInfo[] Parameters;
      public Expression[] Args;
    }
  }
}

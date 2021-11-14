// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.ExceptionExtensions
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.LsuPro
{
  public static class ExceptionExtensions
  {
    public static int GetHResult(this Exception exception) => (int) ((IEnumerable<PropertyInfo>) exception.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).First<PropertyInfo>((Func<PropertyInfo, bool>) (pr => pr.Name.Equals("HResult"))).GetValue((object) exception, (object[]) null);
  }
}

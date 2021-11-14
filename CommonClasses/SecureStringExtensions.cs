// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SecureStringExtensions
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.LsuPro
{
  public static class SecureStringExtensions
  {
    public static string GetString(this SecureString secureString)
    {
      IntPtr num = IntPtr.Zero;
      try
      {
        num = Marshal.SecureStringToBSTR(secureString);
        return Marshal.PtrToStringUni(num);
      }
      finally
      {
        Marshal.ZeroFreeBSTR(num);
      }
    }

    public static void SetString(this SecureString secureValue, string value)
    {
      secureValue.Clear();
      foreach (char c in value)
        secureValue.AppendChar(c);
    }
  }
}

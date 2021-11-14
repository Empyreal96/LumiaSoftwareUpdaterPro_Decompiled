// Decompiled with JetBrains decompiler
// Type: System.Linq.Expressions.ParseException
// Assembly: GalaSoft.MvvmLight.Extras.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=1673db7d5906b0ad
// MVID: C70B08F8-E577-41D6-BDC6-D5E26E362355
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.Extras.WPF4.dll

namespace System.Linq.Expressions
{
  public sealed class ParseException : Exception
  {
    private int position;

    public ParseException(string message, int position)
      : base(message)
    {
      this.position = position;
    }

    public int Position => this.position;

    public override string ToString() => string.Format("{0} (at index {1})", (object) this.Message, (object) this.position);
  }
}

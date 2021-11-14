// Decompiled with JetBrains decompiler
// Type: GalaSoft.MvvmLight.Helpers.IExecuteWithObject
// Assembly: GalaSoft.MvvmLight.WPF4, Version=4.0.23.37706, Culture=neutral, PublicKeyToken=63eb5c012e0b3c1c
// MVID: EB319B27-5901-4170-8F30-7B0FB2D79775
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\GalaSoft.MvvmLight.WPF4.dll

namespace GalaSoft.MvvmLight.Helpers
{
  public interface IExecuteWithObject
  {
    object Target { get; }

    void ExecuteWithObject(object parameter);

    void MarkForDeletion();
  }
}

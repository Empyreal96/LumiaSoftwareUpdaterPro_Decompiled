// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.AlphaCollinsEmergencyFlashFiles
// Assembly: Wp8EmergencyFlash, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: FB4E3FD2-E1AC-4420-A6BD-0981454BEEB7
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Wp8EmergencyFlash.dll

namespace Microsoft.LsuPro
{
  public class AlphaCollinsEmergencyFlashFiles
  {
    private ThreadSafeObservableCollection<EmergencyFileInfo> hexFiles;
    private ThreadSafeObservableCollection<EmergencyFileInfo> mbnFiles;

    internal AlphaCollinsEmergencyFlashFiles()
    {
      this.hexFiles = new ThreadSafeObservableCollection<EmergencyFileInfo>();
      this.mbnFiles = new ThreadSafeObservableCollection<EmergencyFileInfo>();
    }

    public ThreadSafeObservableCollection<EmergencyFileInfo> HexFiles => this.hexFiles;

    public ThreadSafeObservableCollection<EmergencyFileInfo> MbnFiles => this.mbnFiles;

    internal void AddHexFile(string hexFile) => this.hexFiles.Add(new EmergencyFileInfo(hexFile));

    internal void AddMbnFile(string mbnFile) => this.mbnFiles.Add(new EmergencyFileInfo(mbnFile));

    public bool IsValid => this.hexFiles.Count > 0 && this.mbnFiles.Count > 0;
  }
}

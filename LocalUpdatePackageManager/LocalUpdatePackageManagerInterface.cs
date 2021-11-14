// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.LocalUpdatePackageManagerInterface
// Assembly: LocalUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 0F47836D-0FA4-443D-8CFF-1138F5AB3C6A
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\LocalUpdatePackageManager.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  public class LocalUpdatePackageManagerInterface
  {
    private LocalUpdatePackageManager localUpdatePackageManager;
    private OnlineUpdatePackageManager onlineUpdatePackageManager;

    public event EventHandler<EventArgs> SqlDbUpdated;

    public LocalUpdatePackageManagerInterface(
      LocalUpdatePackageManager localUpdatePackageManager,
      OnlineUpdatePackageManager onlineUpdatePackageManager)
    {
      this.localUpdatePackageManager = localUpdatePackageManager;
      this.localUpdatePackageManager.SqlDbUpdated += new EventHandler<EventArgs>(this.OnSqlDbUpdated);
      this.onlineUpdatePackageManager = onlineUpdatePackageManager;
    }

    public void SearchForProductCodesSw(string productType, string softwareVersion) => this.onlineUpdatePackageManager.GetProductCodesSw(productType, softwareVersion);

    public void SearchForProductCodesOs(string productType, string winOsVersion) => this.onlineUpdatePackageManager.GetProductCodesOs(productType, winOsVersion);

    public void SearchForSoftwareVersions(string productType, string productCode) => this.onlineUpdatePackageManager.GetSoftwareVersions(productType, productCode);

    public void SearchForSoftwareVersionsSw(string productType) => this.onlineUpdatePackageManager.GetSoftwareVersionsSw(productType);

    public void SearchForProductCodes(string productType) => this.onlineUpdatePackageManager.GetProductCodes(productType);

    public void SearchForWinOsVersionsOs(string productType) => this.onlineUpdatePackageManager.GetWinOsVersionsOs(productType);

    public void SearchForUpdatePackagesSimple(
      string productType,
      string productCode,
      bool online,
      bool forceRescan)
    {
      if (forceRescan)
        this.localUpdatePackageManager.SearchForLocalUpdatePackages(false);
      if (!online)
        return;
      this.onlineUpdatePackageManager.GetLatestUpdatePackageSimple(productType, productCode);
    }

    public void SearchForUpdatePackages(
      string productType,
      string productCode,
      bool online,
      bool forceRescan)
    {
      if (forceRescan)
        this.localUpdatePackageManager.SearchForLocalUpdatePackages(false);
      if (!online)
        return;
      this.onlineUpdatePackageManager.GetLatestUpdatePackage(productType, productCode);
    }

    public void SearchForUpdatePackagesSlow(string productType, bool online, bool forceRescan)
    {
      if (!online)
        return;
      if (this.onlineUpdatePackageManager.OngoingOperations.ContainsKey(productType))
      {
        while (this.onlineUpdatePackageManager.OngoingOperations[productType].Equals("ongoing"))
          Task.Delay(500).Wait();
        if (this.onlineUpdatePackageManager.OngoingOperations[productType].Equals("completed"))
          return;
      }
      this.onlineUpdatePackageManager.GetUpdatePackagesSlow(productType, forceRescan);
    }

    private void OnSqlDbUpdated(object sender, EventArgs e)
    {
      EventHandler<EventArgs> sqlDbUpdated = this.SqlDbUpdated;
      if (sqlDbUpdated == null)
        return;
      sqlDbUpdated((object) this, new EventArgs());
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.OnlineUpdatePackageManager
// Assembly: OnlineUpdatePackageManager, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: F4E4364C-5913-465E-931E-3641FD37012E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\OnlineUpdatePackageManager.dll

using Microsoft.LsuPro.SoftwareRepository;
using SoftwareRepository.Discovery;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Microsoft.LsuPro
{
  public class OnlineUpdatePackageManager
  {
    private SqlHelper sqlHelper;
    private SoftwareRepositoryDiscovery softwareRepositoryDiscovery;
    private IWebProxy webProxy;
    private string accessToken;
    public Dictionary<string, string> OngoingOperations;

    public OnlineUpdatePackageManager()
      : this(new SqlHelper())
    {
    }

    public OnlineUpdatePackageManager(SqlHelper sqlHelper)
    {
      this.sqlHelper = sqlHelper;
      this.OngoingOperations = new Dictionary<string, string>();
    }

    public void SetWebProxy(IWebProxy webProxy)
    {
      this.webProxy = webProxy;
      if (this.softwareRepositoryDiscovery == null)
        return;
      this.softwareRepositoryDiscovery.SetWebProxy(webProxy);
    }

    public void SetAccessToken(string accessToken)
    {
      this.accessToken = accessToken;
      if (this.softwareRepositoryDiscovery == null)
        return;
      this.softwareRepositoryDiscovery.SetAccessToken(accessToken);
    }

    public void Connect(string softwareRepositoryBaseUrl = null) => this.softwareRepositoryDiscovery = new SoftwareRepositoryDiscovery(this.webProxy, this.accessToken, softwareRepositoryBaseUrl);

    public List<string> GetEmergencyTypeDesignators()
    {
      Tracer.Information("MSR: GetEmergencyTypeDesignators");
      while (this.softwareRepositoryDiscovery == null)
        Task.Delay(1000).Wait();
      SoftwarePackages emergencyTypeDesignators = this.softwareRepositoryDiscovery.GetEmergencyTypeDesignators();
      List<string> stringList = new List<string>();
      if (emergencyTypeDesignators != null)
      {
        foreach (SoftwarePackage softwarePackage in emergencyTypeDesignators.SoftwarePackageList)
          stringList.Add(softwarePackage.ManufacturerHardwareModel[0]);
      }
      return stringList;
    }

    public UpdatePackage GetEmergencyPackageMetadata(string productType)
    {
      try
      {
        Tracer.Information("MSR: GetEmergencyPackageMetadata for {0}", (object) productType);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now1 = DateTime.Now;
        SoftwarePackage emergencyPackage = this.softwareRepositoryDiscovery.GetEmergencyPackage(productType, "Public");
        Tracer.Information("MSR: operation took: {0}s", (object) (DateTime.Now - now1).TotalSeconds);
        if (emergencyPackage == null)
        {
          Tracer.Information("MSR: package was not found");
          return (UpdatePackage) null;
        }
        Tracer.Information("MSR: package found");
        List<UpdatePackageFile> updatePackageFileList1 = new List<UpdatePackageFile>();
        DateTime now2 = DateTime.Now;
        List<UpdatePackageFile> updatePackageFileList2;
        try
        {
          updatePackageFileList2 = new List<UpdatePackageFile>(emergencyPackage.Files.Count);
          foreach (SoftwareFile file in emergencyPackage.Files)
          {
            UpdatePackageFile updatePackageFile = new UpdatePackageFile(productType, file.FileName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Products\\{0}\\Manufacturing SW Package", (object) productType), file.FileSize, now2);
            updatePackageFileList2.Add(updatePackageFile);
          }
        }
        catch (Exception ex)
        {
          Tracer.Information("MSR: Cannot add files: {0}", (object) ex.Message);
          return (UpdatePackage) null;
        }
        return new UpdatePackage(emergencyPackage.ManufacturerHardwareModel[0], string.Empty, emergencyPackage.PackageRevision, string.Empty, emergencyPackage.PackageTitle, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, emergencyPackage.Id, string.Empty, true, DateTime.Now, updatePackageFileList2.ToArray());
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetEmergencyPackageMetadata failed: {0}", (object) ex.Message);
        throw;
      }
    }

    public void GetUpdatePackagesSlow(string productType, bool forceRepository = true)
    {
      try
      {
        Tracer.Information("MSR: GetUpdatePackagesSlow for {0}", (object) productType);
        if (!forceRepository)
          return;
        this.OngoingOperations.Add(productType, "ongoing");
        this.sqlHelper.DeleteObsoleteUpdatePackagesForProductType(productType);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEventSlow);
        this.softwareRepositoryDiscovery.GetListOfAllVariantsSlow(productType).Wait();
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEventSlow);
        Tracer.Information("MSR: GetUpdatePackagesSlow: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
        this.OngoingOperations[productType] = "completed";
      }
      catch (AuthenticationException ex)
      {
        this.OngoingOperations[productType] = "failed";
        throw;
      }
      catch (Exception ex)
      {
        this.OngoingOperations[productType] = "failed";
        Tracer.Warning("MSR: GetUpdatePackagesSlow failed {0}", (object) ex.Message);
      }
    }

    public void GetLatestUpdatePackageSimple(string productType, string productCode)
    {
      try
      {
        Tracer.Information("MSR: GetLatestUpdatePackageSimple for {0} and {1}", (object) productType, (object) productCode);
        this.sqlHelper.DeleteAllUpdatePackagesForProductType(productType);
        this.OngoingOperations.Remove(productType);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetLatestUpdatePackageSimple(productType, productCode);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetLatestUpdatePackageSimple: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetLatestUpdatePackageSimple failed {0}", (object) ex.Message);
      }
    }

    public void GetLatestUpdatePackage(string productType, string productCode)
    {
      try
      {
        Tracer.Information("MSR: GetLatestUpdatePackage for {0} and {1}", (object) productType, (object) productCode);
        this.sqlHelper.DeleteAllUpdatePackagesForProductType(productType);
        this.OngoingOperations.Remove(productType);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetLatestUpdatePackage(productType, productCode);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetLatestUpdatePackage: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetLatestUpdatePackage failed {0}", (object) ex.Message);
      }
    }

    public void GetSoftwareVersions(string productType, string productCode)
    {
      try
      {
        Tracer.Information("MSR: GetSoftwareVersions for {0} and {1}", (object) productType, (object) productCode);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetSoftwareVersions(productType, productCode);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetSoftwareVersions: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetSoftwareVersions failed {0}", (object) ex.Message);
      }
    }

    public void GetProductCodes(string productType)
    {
      try
      {
        Tracer.Information("MSR: GetProductCodes for {0}", (object) productType);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetProductCodes(productType);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetProductCodes: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetProductCodes failed {0}", (object) ex.Message);
      }
    }

    public void GetProductCodesSw(string productType, string softwareVersion)
    {
      try
      {
        Tracer.Information("MSR: GetProductCodesSw for {0} and {1}", (object) productType, (object) softwareVersion);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetProductCodesSw(productType, softwareVersion);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetProductCodesSw: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetProductCodesSw failed {0}", (object) ex.Message);
      }
    }

    public void GetProductCodesOs(string productType, string winOSVersion)
    {
      try
      {
        Tracer.Information("MSR: GetProductCodesOs for {0} and {1}", (object) productType, (object) winOSVersion);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetProductCodesOs(productType, winOSVersion);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetProductCodesOs: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetProductCodesOs failed {0}", (object) ex.Message);
      }
    }

    public void GetSoftwareVersionsSw(string productType)
    {
      try
      {
        Tracer.Information("MSR: GetSoftwareVersionsSw for {0}", (object) productType);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetSoftwareVersionsSw(productType);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetSoftwareVersionsSw: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetSoftwareVersionsSw failed {0}", (object) ex.Message);
      }
    }

    public void GetWinOsVersionsOs(string productType)
    {
      try
      {
        Tracer.Information("MSR: GetWinOsVersionsOs for {0}", (object) productType);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent += new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        this.softwareRepositoryDiscovery.GetWinOsVersionsOs(productType);
        this.softwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEvent -= new EventHandler<SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs>(this.OnSoftwareRepositoryDiscoveryEvent);
        Tracer.Information("MSR: GetWinOsVersionsOs: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
      }
      catch (AuthenticationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetWinOsVersionsOs failed {0}", (object) ex.Message);
      }
    }

    public UpdatePackage GetUpdatePackageMetadata(
      SrdfItem srdfItem,
      out string packageState,
      out string productType)
    {
      try
      {
        Tracer.Information("MSR: GetUpdatePackageMetadata for {0}", (object) srdfItem.Query.ManufacturerPackageId);
        packageState = string.Empty;
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        SoftwarePackage variant = (SoftwarePackage) null;
        if (srdfItem.Query.ManufacturerHardwareVariant == null)
        {
          variant = this.softwareRepositoryDiscovery.GetVariant(srdfItem);
        }
        else
        {
          foreach (string str in new List<string>((IEnumerable<string>) srdfItem.Query.ManufacturerHardwareVariant))
          {
            srdfItem.Query.ManufacturerHardwareVariant[0] = str;
            variant = this.softwareRepositoryDiscovery.GetVariant(srdfItem);
            if (variant != null)
              break;
          }
        }
        Tracer.Information("MSR: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
        if (variant == null)
        {
          Tracer.Information("MSR: package was not found");
          packageState = string.Empty;
          productType = string.Empty;
          return (UpdatePackage) null;
        }
        packageState = variant.PackageState;
        productType = variant.ManufacturerHardwareModel[0];
        Tracer.Information("MSR: package found in state: {0}", (object) packageState);
        return packageState == "Completed" || packageState == "" ? this.WriteUpdatePackage(variant.ManufacturerHardwareModel[0], variant) : new UpdatePackage(variant.ManufacturerHardwareModel[0], variant.ManufacturerHardwareVariant[0], variant.PackageRevision, variant.PackageSubRevision, variant.PackageTitle, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, variant.Id, string.Empty, true, DateTime.Now, (UpdatePackageFile[]) null);
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetUpdatePackageMetadata failed: {0}", (object) ex.Message);
        throw;
      }
    }

    public SrdfItem GetVariantMetadata(
      string productType,
      string productCode,
      string softwareVersion,
      string variantVersion)
    {
      try
      {
        Tracer.Information("MSR: GetVariantMetadata for {0}, {1}", (object) productType, (object) productCode);
        while (this.softwareRepositoryDiscovery == null)
          Task.Delay(1000).Wait();
        DateTime now = DateTime.Now;
        SrdfItem variant = this.softwareRepositoryDiscovery.GetVariant(productType, productCode, softwareVersion, variantVersion);
        Tracer.Information("MSR: operation took: {0}s", (object) (DateTime.Now - now).TotalSeconds);
        if (variant == null)
        {
          Tracer.Information("MSR: package was not found");
          return (SrdfItem) null;
        }
        Tracer.Information("MSR: package found");
        return variant;
      }
      catch (Exception ex)
      {
        Tracer.Warning("MSR: GetVariantMetadata failed{0}", (object) ex.Message);
        throw;
      }
    }

    private void OnSoftwareRepositoryDiscoveryEvent(
      object sender,
      SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs softwareRepositoryDiscoveryEventArgs)
    {
      foreach (SoftwarePackage softwarePackage in softwareRepositoryDiscoveryEventArgs.SoftwarePackageList)
      {
        if (softwarePackage.PackageState == "Build Completed" || softwarePackage.PackageState == "Completed" || string.IsNullOrEmpty(softwarePackage.PackageState))
          this.WriteUpdatePackage(softwareRepositoryDiscoveryEventArgs.TypeDesignator, softwarePackage);
      }
    }

    private void OnSoftwareRepositoryDiscoveryEventSlow(
      object sender,
      SoftwareRepositoryDiscovery.SoftwareRepositoryDiscoveryEventArgs softwareRepositoryDiscoveryEventArgs)
    {
      foreach (SoftwarePackage softwarePackage in softwareRepositoryDiscoveryEventArgs.SoftwarePackageList)
      {
        if (softwarePackage.PackageState == "Build Completed" || softwarePackage.PackageState == "Completed" || string.IsNullOrEmpty(softwarePackage.PackageState))
          this.WriteUpdatePackage(softwareRepositoryDiscoveryEventArgs.TypeDesignator, softwarePackage);
      }
    }

    private UpdatePackage WriteUpdatePackage(
      string productType,
      SoftwarePackage variant)
    {
      List<UpdatePackageFile> updatePackageFileList = new List<UpdatePackageFile>();
      DateTime now = DateTime.Now;
      try
      {
        updatePackageFileList = new List<UpdatePackageFile>(variant.Files.Count);
        foreach (SoftwareFile file in variant.Files)
        {
          UpdatePackageFile updatePackageFile = new UpdatePackageFile(productType, file.FileName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Products\\{0}", (object) productType), file.FileSize, now);
          updatePackageFileList.Add(updatePackageFile);
        }
      }
      catch (Exception ex)
      {
      }
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string empty3 = string.Empty;
      string packageUseType = string.Empty;
      string empty4 = string.Empty;
      string empty5 = string.Empty;
      try
      {
        if (variant.ExtendedAttributes != null)
        {
          if (!variant.ExtendedAttributes.Dictionary.TryGetValue("OSVersion", out empty1))
            empty1 = string.Empty;
          if (!variant.ExtendedAttributes.Dictionary.TryGetValue("AKVersion", out empty2))
            empty2 = string.Empty;
          if (!variant.ExtendedAttributes.Dictionary.TryGetValue("BSPVersion", out empty3))
            empty3 = string.Empty;
          if (!variant.ExtendedAttributes.Dictionary.TryGetValue("purpose", out empty4))
            empty4 = string.Empty;
          if (!variant.ExtendedAttributes.Dictionary.TryGetValue("releaseType", out empty5))
            empty5 = string.Empty;
          if (!variant.ExtendedAttributes.Dictionary.TryGetValue("imageType", out packageUseType))
            packageUseType = string.Empty;
          if (!string.IsNullOrEmpty(packageUseType))
            packageUseType = packageUseType.ToLowerInvariant().Contains("public") || packageUseType.ToLowerInvariant().Contains("retail") ? "retail" : "RnD";
        }
      }
      catch (Exception ex)
      {
      }
      UpdatePackage updatePackage = new UpdatePackage(productType, variant.ManufacturerHardwareVariant == null ? (string) null : variant.ManufacturerHardwareVariant[0], variant.PackageRevision, variant.PackageSubRevision, variant.PackageTitle, empty4, packageUseType, empty5, empty1, empty2, empty3, string.Empty, string.IsNullOrEmpty(variant.Id) ? productType + variant.ManufacturerHardwareVariant[0] + variant.PackageRevision + variant.PackageSubRevision : variant.Id, string.Empty, true, now, updatePackageFileList.ToArray());
      this.sqlHelper.InsertUpdatePackage(updatePackage);
      return updatePackage;
    }
  }
}

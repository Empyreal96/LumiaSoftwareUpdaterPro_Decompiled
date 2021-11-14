// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SelfExtractingPackageCreater
// Assembly: SelfExtractingPackageCreator, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 15D44AF4-CB22-48AD-A5ED-B49E916EE3E9
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SelfExtractingPackageCreator.dll

using Ionic.Zip;
using SelfExtractingPackageCreator.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Microsoft.LsuPro
{
  public class SelfExtractingPackageCreater
  {
    private int previousProgress;
    private string previousMessage;
    private IList<string> fileNames = (IList<string>) new List<string>();

    public event EventHandler<SelfExtractingPackageCreationProgressChangedEventArgs> SelfExtractingPackageCreationProgressChanged;

    public SelfExtractingPackageCreater()
    {
      this.ProductName = "LSU";
      this.ProductVersion = "1.2.3.4";
      this.TypeDesignator = "RM-";
    }

    public void SetFilesToPackage(IEnumerable<string> files)
    {
      foreach (string file in files)
      {
        this.fileNames.Add(file);
        Tracer.Information("File \"{0}\" added.", (object) file);
      }
      Tracer.Information("Package will contain {0} files", (object) this.fileNames.Count);
    }

    public string ExeFilename { get; set; }

    public string ProductName { get; set; }

    public string ProductVersion { get; set; }

    public string TypeDesignator { get; set; }

    public void CreatePackage()
    {
      string path = SpecialFolders.Bin + "\\selfExtractor.ico";
      Tracer.Information("Start CreatePackage");
      try
      {
        using (ZipFile zipFile = new ZipFile())
        {
          zipFile.ParallelDeflateThreshold = -1L;
          zipFile.TempFileFolder = Path.GetTempPath();
          this.previousProgress = 0;
          SelfExtractorSaveOptions options = new SelfExtractorSaveOptions();
          options.Description = "Self-extracting Variant Package";
          options.DefaultExtractDirectory = "C:\\ProgramData\\Microsoft\\Packages\\Products\\";
          options.ProductName = this.ProductName;
          try
          {
            Icon selfExtractorIcon = Resource.SelfExtractorIcon;
            FileStream fileStream1 = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            FileStream fileStream2 = fileStream1;
            selfExtractorIcon.Save((Stream) fileStream2);
            fileStream1.Close();
            options.IconFile = path;
          }
          catch (Exception ex)
          {
            object[] objArray = new object[0];
            Tracer.Warning(ex, "Unable to change icon to self extracting package. Continuing with default package.", objArray);
          }
          options.ProductVersion = this.ProductVersion;
          options.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
          options.Copyright = "© Microsoft Mobile";
          options.Flavor = SelfExtractorFlavor.ConsoleApplication;
          zipFile.SaveProgress += (EventHandler<SaveProgressEventArgs>) ((sender, e) =>
          {
            if (e.EventType == ZipProgressEventType.Saving_Started)
              this.OnSelfExtractingPackageCreationProgressChanged(new SelfExtractingPackageCreationProgressChangedEventArgs(string.Format("Saving: {0}", (object) e.ArchiveName), 0));
            else if (e.EventType == ZipProgressEventType.Saving_AfterWriteEntry)
              this.previousProgress = 0;
            else if (e.EventType == ZipProgressEventType.Saving_Completed)
            {
              this.OnSelfExtractingPackageCreationProgressChanged(new SelfExtractingPackageCreationProgressChangedEventArgs("Done", 100));
              this.previousProgress = 0;
            }
            else if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
            {
              string str = string.Format("Writing: {0} ({1}/{2})", (object) e.CurrentEntry.FileName, (object) (e.EntriesSaved + 1), (object) e.EntriesTotal);
              if (!(this.previousMessage != str))
                return;
              this.previousMessage = str;
              this.OnSelfExtractingPackageCreationProgressChanged(new SelfExtractingPackageCreationProgressChangedEventArgs(this.previousMessage, 0));
            }
            else
            {
              if (e.EventType != ZipProgressEventType.Saving_EntryBytesRead)
                return;
              int percentage = (int) (100L * e.BytesTransferred / e.TotalBytesToTransfer);
              if (percentage <= this.previousProgress)
                return;
              this.OnSelfExtractingPackageCreationProgressChanged(new SelfExtractingPackageCreationProgressChangedEventArgs(this.previousMessage, percentage));
              this.previousProgress = percentage;
            }
          });
          zipFile.AddFiles((IEnumerable<string>) this.fileNames, this.TypeDesignator);
          Tracer.Information("Start creating the executable.");
          zipFile.SaveSelfExtractor(this.ExeFilename, options);
        }
        Tracer.Information("CreatePackage done!");
      }
      catch (Exception ex)
      {
        object[] objArray = new object[0];
        Tracer.Error(ex, "Exception during self extracting zip package creation.", objArray);
        throw;
      }
      finally
      {
        File.Delete(path);
      }
    }

    protected virtual void OnSelfExtractingPackageCreationProgressChanged(
      SelfExtractingPackageCreationProgressChangedEventArgs eventArgs)
    {
      EventHandler<SelfExtractingPackageCreationProgressChangedEventArgs> creationProgressChanged = this.SelfExtractingPackageCreationProgressChanged;
      if (creationProgressChanged == null)
        return;
      creationProgressChanged((object) this, eventArgs);
    }
  }
}

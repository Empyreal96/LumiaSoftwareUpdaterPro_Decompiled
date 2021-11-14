// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.FileStreamer
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SoftwareRepository.Streaming
{
  public class FileStreamer : Streamer
  {
    private static HashSet<char> invalidFileCharsSet;
    public const string ResumeExtension = ".resume";
    public const string AppDataSubFolder = "SoftwareRepositoryResume";

    private static HashSet<char> InvalidFileCharsSet => FileStreamer.invalidFileCharsSet ?? (FileStreamer.invalidFileCharsSet = new HashSet<char>((IEnumerable<char>) Path.GetInvalidFileNameChars()));

    public static string DefaultResumeFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SoftwareRepositoryResume");

    public string ResumeFileName { get; set; }

    public string FileName { get; set; }

    public FileStreamer(string downloadPath, string packageId, bool multiPath = false)
      : this(downloadPath, packageId, FileStreamer.DefaultResumeFolder, multiPath)
    {
    }

    public FileStreamer(
      string downloadPath,
      string packageId,
      string resumeFolder,
      bool multiPath = false)
    {
      if (string.IsNullOrEmpty(downloadPath))
        throw new ArgumentNullException(nameof (downloadPath));
      if (string.IsNullOrEmpty(packageId))
        throw new ArgumentNullException(nameof (packageId));
      if (string.IsNullOrEmpty(resumeFolder))
        throw new ArgumentNullException(nameof (resumeFolder));
      this.FileName = downloadPath;
      this.ResumeFileName = FileStreamer.GetResumePath(downloadPath, packageId, resumeFolder, multiPath);
    }

    public static string GetResumePath(
      string downloadPath,
      string packageId,
      string resumeFolder = null,
      bool multiPath = false)
    {
      if (string.IsNullOrEmpty(downloadPath))
        throw new ArgumentNullException(nameof (downloadPath));
      if (string.IsNullOrEmpty(packageId))
        throw new ArgumentNullException(nameof (packageId));
      resumeFolder = resumeFolder ?? FileStreamer.DefaultResumeFolder;
      string str;
      if (multiPath)
      {
        using (SHA256 shA256 = SHA256.Create())
        {
          byte[] hash = shA256.ComputeHash(Encoding.Unicode.GetBytes(downloadPath));
          str = BitConverter.ToString(((IEnumerable<byte>) hash).Take<byte>(4).Concat<byte>(((IEnumerable<byte>) hash).Skip<byte>(28)).ToArray<byte>()).Replace("-", string.Empty).ToLowerInvariant();
        }
      }
      else
        str = Path.GetFileName(downloadPath);
      return Path.Combine(resumeFolder, new string(packageId.Select<char, char>((Func<char, char>) (c => !FileStreamer.InvalidFileCharsSet.Contains(c) ? c : '-')).ToArray<char>()) + "_" + str + ".resume");
    }

    public override void SetMetadata(byte[] metadata)
    {
      if (metadata != null)
      {
        string directoryName = Path.GetDirectoryName(this.ResumeFileName);
        if (!string.IsNullOrEmpty(directoryName))
          Directory.CreateDirectory(directoryName);
        using (FileStream fileStream = new FileStream(this.ResumeFileName, FileMode.Create))
          fileStream.Write(metadata, 0, metadata.Length);
      }
      else
        this.ClearMetadata();
    }

    public override byte[] GetMetadata()
    {
      if (File.Exists(this.ResumeFileName))
      {
        if (File.Exists(this.FileName))
        {
          using (FileStream fileStream = new FileStream(this.ResumeFileName, FileMode.Open))
          {
            fileStream.Seek(0L, SeekOrigin.Begin);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            return buffer;
          }
        }
        else
          this.ClearMetadata();
      }
      return (byte[]) null;
    }

    public override void ClearMetadata()
    {
      if (!File.Exists(this.ResumeFileName))
        return;
      File.Delete(this.ResumeFileName);
    }

    protected override Stream GetStreamInternal()
    {
      string directoryName = Path.GetDirectoryName(this.FileName);
      if (!string.IsNullOrEmpty(directoryName))
        Directory.CreateDirectory(directoryName);
      return (Stream) new FileStream(this.FileName, FileMode.OpenOrCreate);
    }
  }
}

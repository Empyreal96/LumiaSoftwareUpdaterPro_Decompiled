// Decompiled with JetBrains decompiler
// Type: SoftwareRepository.Streaming.DownloadMetadata
// Assembly: SoftwareRepository, Version=2.1.5.19959, Culture=neutral, PublicKeyToken=null
// MVID: E5A69774-90A6-4202-AB96-618C2EE657B8
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\SoftwareRepository.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SoftwareRepository.Streaming
{
  [Serializable]
  internal class DownloadMetadata
  {
    internal ChunkState[] ChunkStates;
    internal Dictionary<int, long> PartialProgress;

    internal byte[] Serialize()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        try
        {
          new BinaryFormatter().Serialize((Stream) memoryStream, (object) this);
        }
        catch
        {
          return (byte[]) null;
        }
        return memoryStream.ToArray();
      }
    }

    internal static DownloadMetadata Deserialize(byte[] data)
    {
      DownloadMetadata downloadMetadata;
      using (MemoryStream memoryStream = new MemoryStream(data))
      {
        try
        {
          downloadMetadata = new BinaryFormatter().Deserialize((Stream) memoryStream) as DownloadMetadata;
        }
        catch
        {
          return (DownloadMetadata) null;
        }
      }
      return downloadMetadata == null || !downloadMetadata.IsValid() ? (DownloadMetadata) null : downloadMetadata;
    }

    private bool IsValid()
    {
      if (this.ChunkStates == null)
        return false;
      int num = 0;
      for (int key = 0; key < this.ChunkStates.Length; ++key)
      {
        if (this.ChunkStates[key] == ChunkState.PartiallyDownloaded)
        {
          ++num;
          if (this.PartialProgress == null || !this.PartialProgress.ContainsKey(key))
            return false;
        }
      }
      return this.PartialProgress == null || num == this.PartialProgress.Count;
    }
  }
}

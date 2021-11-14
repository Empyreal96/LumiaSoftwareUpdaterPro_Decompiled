// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.MEOPENINFO
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  internal struct MEOPENINFO
  {
    internal IntPtr pwszFileName;
    internal IntPtr pwszPassword;
    internal IntPtr pwszTempPath;
    internal int lcidLocale;
    internal int cbBufferPool;
    internal int fEncrypt;
    internal int dwAutoShrinkPercent;
    internal int dwFlushInterval;
    internal int cMaxPages;
    internal int cMaxTmpPages;
    internal int dwDefaultTimeout;
    internal int dwDefaultEscalation;
    internal SEOPENFLAGS dwFlags;
    internal int dwEncryptionMode;
    internal int dwLocaleFlags;
  }
}

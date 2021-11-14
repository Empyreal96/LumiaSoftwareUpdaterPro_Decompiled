// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalSilentTokenAcquisitionException
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [Serializable]
  public class AdalSilentTokenAcquisitionException : AdalException
  {
    public AdalSilentTokenAcquisitionException()
      : base("failed_to_acquire_token_silently", "Failed to acquire token silently. Call method AcquireToken")
    {
    }

    protected AdalSilentTokenAcquisitionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      base.GetObjectData(info, context);
    }
  }
}

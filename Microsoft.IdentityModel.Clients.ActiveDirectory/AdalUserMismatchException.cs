// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.AdalUserMismatchException
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 639EF55C-2FA6-4A39-9C07-D7E0AA203205
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
  [Serializable]
  public class AdalUserMismatchException : AdalException
  {
    public AdalUserMismatchException(string requestedUser, string returnedUser)
      : base("user_mismatch", string.Format("User '{0}' returned by service does not match user '{1}' in the request", (object) returnedUser, (object) requestedUser))
    {
      this.RequestedUser = requestedUser;
      this.ReturnedUser = returnedUser;
    }

    public string RequestedUser { get; private set; }

    public string ReturnedUser { get; private set; }

    public override string ToString() => base.ToString() + string.Format("\n\tRequestedUser: {0}\n\tReturnedUser: {1}", (object) this.RequestedUser, (object) this.ReturnedUser);

    protected AdalUserMismatchException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.RequestedUser = info.GetString(nameof (RequestedUser));
      this.ReturnedUser = info.GetString(nameof (ReturnedUser));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      info.AddValue("RequestedUser", (object) this.RequestedUser);
      info.AddValue("ReturnedUser", (object) this.ReturnedUser);
      base.GetObjectData(info, context);
    }
  }
}

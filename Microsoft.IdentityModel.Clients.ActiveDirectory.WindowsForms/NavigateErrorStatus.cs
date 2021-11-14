// Decompiled with JetBrains decompiler
// Type: Microsoft.IdentityModel.Clients.ActiveDirectory.Internal.NavigateErrorStatus
// Assembly: Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 8A881C32-CCB6-4F02-9376-495633C07EEC
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Microsoft.IdentityModel.Clients.ActiveDirectory.WindowsForms.dll

using System.Collections.Generic;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory.Internal
{
  internal class NavigateErrorStatus
  {
    public Dictionary<int, string> Messages;

    public NavigateErrorStatus() => this.Messages = new Dictionary<int, string>()
    {
      {
        400,
        "The request could not be processed by the server due to invalid syntax."
      },
      {
        401,
        "The requested resource requires user authentication."
      },
      {
        402,
        "Not currently implemented in the HTTP protocol."
      },
      {
        403,
        "The server understood the request, but is refusing to fulfill it."
      },
      {
        404,
        "The server has not found anything matching the requested URI (Uniform Resource Identifier)."
      },
      {
        405,
        "The HTTP verb used is not allowed."
      },
      {
        406,
        "No responses acceptable to the client were found."
      },
      {
        407,
        "Proxy authentication required."
      },
      {
        408,
        "The server timed out waiting for the request."
      },
      {
        409,
        "The request could not be completed due to a conflict with the current state of the resource. The user should resubmit with more information."
      },
      {
        410,
        "The requested resource is no longer available at the server, and no forwarding address is known."
      },
      {
        411,
        "The server refuses to accept the request without a defined content length."
      },
      {
        412,
        "The precondition given in one or more of the request header fields evaluated to false when it was tested on the server."
      },
      {
        413,
        "The server is refusing to process a request because the request entity is larger than the server is willing or able to process."
      },
      {
        414,
        "The server is refusing to service the request because the request URI (Uniform Resource Identifier) is longer than the server is willing to interpret."
      },
      {
        415,
        "The server is refusing to service the request because the entity of the request is in a format not supported by the requested resource for the requested method."
      },
      {
        449,
        "The request should be retried after doing the appropriate action."
      },
      {
        500,
        "The server encountered an unexpected condition that prevented it from fulfilling the request."
      },
      {
        501,
        "The server does not support the functionality required to fulfill the request."
      },
      {
        502,
        "The server, while acting as a gateway or proxy, received an invalid response from the upstream server it accessed in attempting to fulfill the request."
      },
      {
        503,
        "The service is temporarily overloaded."
      },
      {
        504,
        "The request was timed out waiting for a gateway."
      },
      {
        505,
        "The server does not support, or refuses to support, the HTTP protocol version that was used in the request message."
      },
      {
        -2146697207,
        "Authentication is needed to access the object."
      },
      {
        -2146697212,
        "The attempt to connect to the Internet has failed."
      },
      {
        -2146697200,
        "CoCreateInstance failed."
      },
      {
        -2146697201,
        "The object could not be loaded."
      },
      {
        -2146697194,
        "The requested resource could not be locked."
      },
      {
        -2146696448,
        "Cannot replace a file that is protected by SFP."
      },
      {
        -2146696960,
        "The component download was declined by the user."
      },
      {
        -2146696192,
        "Internet Explorer 6 for Windows XP SP2 and later. The Authenticode prompt for installing a ActiveX control was not shown because the page restricts the installation of the ActiveX controls. The usual cause is that the Information Bar is shown instead of the Authenticode prompt."
      },
      {
        -2146695936,
        "Internet Explorer 6 for Windows XP SP2 and later. Installation of ActiveX control (as identified by cryptographic file hash) has been disallowed by registry key policy."
      },
      {
        -2146697205,
        "The Internet connection has timed out."
      },
      {
        -2146697209,
        "An Internet connection was established, but the data cannot be retrieved."
      },
      {
        -2146697208,
        "The download has failed (the connection was interrupted)."
      },
      {
        -2146697191,
        "The SSL certificate is invalid."
      },
      {
        -2146697204,
        "The request was invalid."
      },
      {
        -2146697214,
        "The URL could not be parsed."
      },
      {
        -2146697213,
        "No Internet session was established."
      },
      {
        -2146697206,
        "The object is not in one of the acceptable MIME types."
      },
      {
        -2146697210,
        "The object was not found."
      },
      {
        -2146697196,
        "WinInet cannot redirect. This error code might also be returned by a custom protocol handler."
      },
      {
        -2146697195,
        "The request is being redirected to a directory."
      },
      {
        -2146697211,
        "The server or proxy was not found."
      },
      {
        -2146696704,
        "The binding has already been completed and the result has been dispatched, so your abort call has been canceled."
      },
      {
        -2146697202,
        "A security problem was encountered."
      },
      {
        -2146697192,
        "Binding was terminated. (See IBinding::GetBindResult.)"
      },
      {
        -2146697203,
        "The protocol is not known and no pluggable protocols have been entered that match."
      },
      {
        -2146697193,
        "(Microsoft internal.) Reissue request with extended binding."
      }
    };
  }
}

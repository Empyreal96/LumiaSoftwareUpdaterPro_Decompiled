// Decompiled with JetBrains decompiler
// Type: Nokia.Lucid.TcpServer.TcpServerIo
// Assembly: Nokia.Lucid, Version=2.5.193.1435, Culture=neutral, PublicKeyToken=null
// MVID: D962F4C7-242B-4AC5-B046-53CA9A990952
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\Nokia.Lucid.dll

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Nokia.Lucid.TcpServer
{
  public class TcpServerIo
  {
    private readonly object clientObjectLock = new object();
    private CancellationTokenSource cancelServer;
    private Task serverThread;
    private Socket client;
    private Socket listener;
    private bool disposed;

    ~TcpServerIo() => this.Dispose(false);

    public event EventHandler<MessageIoEventArgs> OnExternalSendRequest;

    public bool PhonetProtocol { get; set; }

    public int Port { get; private set; }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void Start(int portNumber, int portRange)
    {
      this.CheckIfDisposed();
      if (portNumber <= 0)
        throw new ArgumentOutOfRangeException(nameof (portNumber), "Port number cannot be 0 or negative");
      if (portRange < 0)
        throw new ArgumentOutOfRangeException(nameof (portRange), "Port range cannot be negative");
      this.listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      int num = portNumber + portRange;
      while (true)
      {
        try
        {
          this.listener.Bind((EndPoint) new IPEndPoint(IPAddress.Any, portNumber));
          break;
        }
        catch (SocketException ex)
        {
          if (ex.ErrorCode == 10048 && portNumber == num)
            throw new IndexOutOfRangeException("No ports were available in the specified ports range", (Exception) ex);
          if (ex.ErrorCode == 10048 && portNumber < num)
            ++portNumber;
          else
            throw;
        }
      }
      this.Port = portNumber;
      this.listener.Listen(1);
      this.cancelServer = new CancellationTokenSource();
      CancellationToken token = this.cancelServer.Token;
      this.serverThread = new Task((Action) (() => this.ServerExecution(token)));
      this.serverThread.Start();
    }

    public void Stop()
    {
      this.CheckIfDisposed();
      try
      {
        if (this.cancelServer != null)
          this.cancelServer.Cancel();
        if (this.listener != null)
        {
          this.listener.Close();
          this.listener = (Socket) null;
        }
        lock (this.clientObjectLock)
        {
          if (this.client != null)
          {
            this.client.Shutdown(SocketShutdown.Both);
            this.client.Close();
            this.client = (Socket) null;
          }
        }
        if (this.serverThread != null)
        {
          this.serverThread.Wait(5000);
          this.serverThread.Dispose();
          this.serverThread = (Task) null;
        }
        if (this.cancelServer == null)
          return;
        this.cancelServer.Dispose();
        this.cancelServer = (CancellationTokenSource) null;
      }
      catch (AggregateException ex)
      {
      }
    }

    public void Send(ref byte[] dataToSend, uint length)
    {
      this.CheckIfDisposed();
      if (this.PhonetProtocol)
        dataToSend[0] = (byte) 29;
      try
      {
        if (this.client == null || !this.client.Connected)
          return;
        this.client.BeginSend(dataToSend, 0, (int) length, SocketFlags.None, new AsyncCallback(this.AsyncSend), (object) this.client);
      }
      catch (Exception ex)
      {
      }
    }

    private void AsyncSend(IAsyncResult ar)
    {
      try
      {
        ((Socket) ar.AsyncState).EndSend(ar);
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    private void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      int num = disposing ? 1 : 0;
      this.Stop();
      this.disposed = true;
    }

    private void SendToDevice(ref byte[] dataToSend)
    {
      if (this.OnExternalSendRequest == null)
        return;
      this.OnExternalSendRequest((object) this, new MessageIoEventArgs(ref dataToSend));
    }

    private void CheckIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException("TcpServerIo object has been disposed.");
    }

    private void ServerExecution(CancellationToken ct)
    {
      byte[] buffer = new byte[1048576];
      while (!ct.IsCancellationRequested)
      {
        try
        {
          if (this.client == null)
            this.client = this.listener.Accept();
          int count = this.client.Receive(buffer);
          if (count < 1)
          {
            lock (this.clientObjectLock)
            {
              this.client.Disconnect(false);
              this.client.Dispose();
              this.client = (Socket) null;
            }
          }
          else
          {
            byte[] dataToSend = new byte[count];
            Buffer.BlockCopy((Array) buffer, 0, (Array) dataToSend, 0, count);
            this.SendToDevice(ref dataToSend);
          }
        }
        catch (SocketException ex)
        {
          lock (this.clientObjectLock)
          {
            this.client.Disconnect(false);
            this.client.Dispose();
            this.client = (Socket) null;
          }
        }
      }
    }
  }
}

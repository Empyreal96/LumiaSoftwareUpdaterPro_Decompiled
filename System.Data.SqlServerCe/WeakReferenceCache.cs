// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.WeakReferenceCache
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

namespace System.Data.SqlServerCe
{
  internal class WeakReferenceCache
  {
    private bool _trackResurrection;
    protected WeakReference[] items;

    static WeakReferenceCache() => KillBitHelper.ThrowIfKillBitIsSet();

    internal int Count
    {
      get
      {
        lock (this)
          return this.items.Length;
      }
    }

    internal WeakReferenceCache(bool trackResurrection)
    {
      this.items = new WeakReference[20];
      this._trackResurrection = trackResurrection;
    }

    internal int Add(object value)
    {
      lock (this)
      {
        int length = this.items.Length;
        for (int index = 0; index < length; ++index)
        {
          WeakReference weakReference = this.items[index];
          if (weakReference == null)
          {
            this.items[index] = new WeakReference(value, this._trackResurrection);
            return index;
          }
          if (!ADP.IsAlive(weakReference))
          {
            weakReference.Target = value;
            return index;
          }
        }
        WeakReference[] weakReferenceArray = new WeakReference[5 == length ? 15 : length + 15];
        for (int index = 0; index < length; ++index)
          weakReferenceArray[index] = this.items[index];
        weakReferenceArray[length] = new WeakReference(value, this._trackResurrection);
        this.items = weakReferenceArray;
        return length;
      }
    }

    internal object GetObject(int indx)
    {
      lock (this)
      {
        WeakReference weakReference = this.items[indx];
        return ADP.IsAlive(weakReference) ? weakReference.Target : (object) null;
      }
    }

    internal void Remove(object value)
    {
      lock (this)
      {
        int length = this.items.Length;
        for (int index = 0; index < length; ++index)
        {
          WeakReference weakReference = this.items[index];
          if (ADP.IsAlive(weakReference) && value == weakReference.Target)
          {
            this.items[index] = (WeakReference) null;
            break;
          }
        }
      }
    }

    internal void RemoveAt(int index)
    {
      lock (this)
        this.items[index] = (WeakReference) null;
    }
  }
}

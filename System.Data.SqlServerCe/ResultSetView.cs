// Decompiled with JetBrains decompiler
// Type: System.Data.SqlServerCe.ResultSetView
// Assembly: System.Data.SqlServerCe, Version=4.0.0.1, Culture=neutral, PublicKeyToken=89845dcd8080cc91
// MVID: 7FFB8205-147A-42A6-BE59-8BAD2AC4B376
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Data.SqlServerCe.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace System.Data.SqlServerCe
{
  public sealed class ResultSetView : 
    ITypedList,
    IBindingList,
    IList,
    ICollection,
    IEnumerable,
    IDisposable
  {
    private SqlCeResultSet parent;
    private ArrayList rowViewList;
    private PropertyDescriptorCollection propDescCollection;
    private int[] ordinals;
    private ListChangedEventHandler onListChanged;
    private ResultSetChangedEventHandler onResultSetChanged;

    static ResultSetView() => KillBitHelper.ThrowIfKillBitIsSet();

    internal ResultSetView(SqlCeResultSet resultSet)
    {
      this.parent = resultSet;
      IList<int> bookmarkArray = this.parent.BookmarkArray;
      this.rowViewList = new ArrayList(bookmarkArray.Count);
      for (int index = 0; index < bookmarkArray.Count; ++index)
        this.rowViewList.Add((object) new RowView(this, bookmarkArray[index]));
      this.onResultSetChanged = new ResultSetChangedEventHandler(this.OnResultSetChanged);
      this.parent.ResultSetChanged += this.onResultSetChanged;
    }

    internal SqlCeResultSet SqlCeResultSet => this.parent;

    public string[] Columns
    {
      get
      {
        if (this.ordinals != null)
        {
          string[] strArray = new string[this.ordinals.Length];
          for (int index = 0; index < strArray.Length; ++index)
            strArray[index] = this.parent.GetName(this.ordinals[index]);
          return strArray;
        }
        string[] strArray1 = new string[this.parent.FieldCount];
        for (int ordinal = 0; ordinal < strArray1.Length; ++ordinal)
          strArray1[ordinal] = this.parent.GetName(ordinal);
        return strArray1;
      }
      set
      {
        try
        {
          this.ordinals = value != null ? new int[value.Length] : throw new ArgumentNullException(nameof (Columns));
          for (int index = 0; index < value.Length; ++index)
            this.ordinals[index] = this.parent.GetOrdinal(value[index]);
          this.FillPropertyDescriptorArray();
        }
        catch (Exception ex)
        {
          this.ordinals = (int[]) null;
          throw;
        }
      }
    }

    public int[] Ordinals
    {
      get
      {
        if (this.ordinals == null)
        {
          int fieldCount = this.parent.FieldCount;
          this.ordinals = new int[fieldCount];
          for (int index = 0; index < fieldCount; ++index)
            this.ordinals[index] = index;
        }
        return (int[]) this.ordinals.Clone();
      }
      set
      {
        try
        {
          if (value == null)
            throw new ArgumentNullException(nameof (Ordinals));
          for (int index = 0; index < value.Length; ++index)
          {
            if (value[index] < 0 || value[index] >= this.parent.FieldCount)
              throw new IndexOutOfRangeException();
          }
          this.ordinals = value;
          this.FillPropertyDescriptorArray();
        }
        catch (Exception ex)
        {
          this.ordinals = (int[]) null;
          throw;
        }
      }
    }

    void IDisposable.Dispose()
    {
      if (this.parent != null)
        this.parent.ResultSetChanged -= this.onResultSetChanged;
      if (this.rowViewList != null)
        this.rowViewList.Clear();
      if (this.propDescCollection != null)
        this.propDescCollection.Clear();
      this.ordinals = (int[]) null;
      this.parent = (SqlCeResultSet) null;
    }

    private void OnListChanged(ListChangedEventArgs e)
    {
      if (this.onListChanged == null)
        return;
      this.onListChanged((object) this, e);
    }

    private void OnResultSetChanged(object sender, ResultSetChangedEventArgs e)
    {
      if (this.onListChanged == null)
        return;
      RowView rowView = new RowView(e.RowView);
      rowView.Parent = this;
      if (ChangeType.RowDeleted != e.ChangeType)
        rowView.Refresh();
      if (e.ChangeType == ChangeType.RowInserted)
      {
        int newIndex = this.rowViewList.Add((object) rowView);
        if (-1 == newIndex)
          return;
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, newIndex));
      }
      else if (ChangeType.RowUpdated == e.ChangeType)
      {
        int num = ((IList) this).IndexOf((object) rowView);
        if (-1 == num)
          return;
        this.rowViewList[num] = (object) rowView;
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, num));
      }
      else
      {
        if (ChangeType.RowDeleted != e.ChangeType)
          return;
        int num = ((IList) this).IndexOf((object) rowView);
        if (-1 == num)
          return;
        this.rowViewList.RemoveAt(num);
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, num));
      }
    }

    public event ListChangedEventHandler ListChanged
    {
      add => this.onListChanged += value;
      remove => this.onListChanged -= value;
    }

    IEnumerator IEnumerable.GetEnumerator() => this.rowViewList.GetEnumerator();

    bool IList.IsFixedSize => !this.parent.Updatable;

    bool IList.IsReadOnly => !this.parent.Updatable;

    object IList.this[int index]
    {
      get => this.rowViewList[index];
      set => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IList.set_Item"));
    }

    int IList.Add(object value) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IList.Add"));

    void IList.Clear() => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IList.Clear"));

    bool IList.Contains(object value) => value is RowView && ((RowView) value).Parent == this && this.rowViewList.Contains(value);

    int IList.IndexOf(object value) => value is RowView && ((RowView) value).Parent == this ? this.rowViewList.IndexOf(value) : -1;

    void IList.Insert(int index, object value) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IList.Insert"));

    void IList.Remove(object value)
    {
      int index = ((IList) this).IndexOf(value);
      if (index < 0)
        return;
      ((IList) this).RemoveAt(index);
    }

    void IList.RemoveAt(int index)
    {
      if (!((IBindingList) this).AllowRemove)
        throw new InvalidOperationException();
      if (index < 0 || index >= this.rowViewList.Count)
        throw new ArgumentOutOfRangeException(nameof (index));
      RowView rowView = (RowView) this.rowViewList[index];
      if (rowView.IsNew)
      {
        this.rowViewList.RemoveAt(index);
        this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
      }
      else
      {
        try
        {
          if (!this.parent.GotoRow(rowView.Bookmark))
            return;
          this.parent.Delete();
          this.parent.RemoveBookmarkFromCache(rowView.Bookmark);
        }
        catch (SqlCeException ex)
        {
          if (-2147217906 == ex.HResult || -2147217885 == ex.HResult)
            this.parent.RemoveBookmarkFromCache(rowView.Bookmark);
          else
            rowView.Error = ex.Message;
        }
      }
    }

    int ICollection.Count => this.rowViewList.Count;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => (object) this;

    void ICollection.CopyTo(Array array, int index)
    {
      for (int index1 = 0; index1 < ((ICollection) this).Count; ++index1)
        array.SetValue(((IList) this)[index1], index++);
    }

    string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
    {
      if (listAccessors != null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (listAccessors)));
      return string.Empty;
    }

    private void FillPropertyDescriptorArray()
    {
      int length = this.parent.FieldCount;
      bool flag;
      if (this.ordinals != null)
      {
        length = this.ordinals.Length;
        flag = true;
      }
      else
      {
        this.ordinals = new int[length];
        flag = false;
      }
      int num = 0;
      this.propDescCollection = (PropertyDescriptorCollection) null;
      PropertyDescriptor[] properties = new PropertyDescriptor[length];
      for (int index = 0; index < length; ++index)
      {
        int ordinal;
        if (!flag)
          this.ordinals[index] = ordinal = index;
        else
          ordinal = this.ordinals[index];
        bool isReadOnly = this.parent.metadata[ordinal].isReadOnly;
        properties[num++] = (PropertyDescriptor) new DataColumnPropertyDescriptor(ordinal, this.parent, isReadOnly);
      }
      this.propDescCollection = new PropertyDescriptorCollection(properties);
    }

    PropertyDescriptorCollection ITypedList.GetItemProperties(
      PropertyDescriptor[] listAccessors)
    {
      if (listAccessors != null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, nameof (listAccessors)));
      if (this.propDescCollection == null)
        this.FillPropertyDescriptorArray();
      return this.propDescCollection;
    }

    bool IBindingList.AllowEdit => this.parent.Updatable;

    bool IBindingList.AllowNew => this.parent.Updatable;

    bool IBindingList.AllowRemove => this.parent.Updatable;

    bool IBindingList.SupportsChangeNotification => true;

    bool IBindingList.SupportsSearching => false;

    bool IBindingList.SupportsSorting => false;

    object IBindingList.AddNew()
    {
      if (!((IBindingList) this).AllowNew)
        throw new InvalidOperationException();
      RowView rowView = new RowView(this, -1, this.parent.CreateRecord());
      rowView.IsNew = true;
      this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, this.rowViewList.Add((object) rowView)));
      return (object) rowView;
    }

    bool IBindingList.IsSorted => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.IsSorted"));

    ListSortDirection IBindingList.SortDirection => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.SortDirection"));

    PropertyDescriptor IBindingList.SortProperty => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.SortProperty"));

    void IBindingList.AddIndex(PropertyDescriptor property) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.AddIndex"));

    void IBindingList.ApplySort(
      PropertyDescriptor property,
      ListSortDirection direction)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.ApplySort"));
    }

    int IBindingList.Find(PropertyDescriptor property, object key) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.Find"));

    void IBindingList.RemoveIndex(PropertyDescriptor property) => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.RemoveIndex"));

    void IBindingList.RemoveSort() => throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "IBindingList.RemoveSort"));
  }
}

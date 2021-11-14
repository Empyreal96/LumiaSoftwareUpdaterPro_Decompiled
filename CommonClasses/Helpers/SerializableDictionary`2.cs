// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.Helpers.SerializableDictionary`2
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.LsuPro.Helpers
{
  [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "reviewed")]
  public class SerializableDictionary<TK, TV>
  {
    public SerializableDictionary(IDictionary<TK, TV> dictionary) => this.OriginalDictionary = dictionary;

    public SerializableDictionary()
    {
    }

    [XmlIgnore]
    public IDictionary<TK, TV> OriginalDictionary { get; set; }

    private Collection<SerializableDictionary<TK, TV>.KeyAndValue> ActualList { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "reviewed")]
    [XmlElement]
    public Collection<SerializableDictionary<TK, TV>.KeyAndValue> KeysAndValues
    {
      get
      {
        if (this.ActualList == null)
          this.ActualList = new Collection<SerializableDictionary<TK, TV>.KeyAndValue>();
        if (this.OriginalDictionary == null)
          return this.ActualList;
        this.ActualList.Clear();
        foreach (KeyValuePair<TK, TV> original in (IEnumerable<KeyValuePair<TK, TV>>) this.OriginalDictionary)
          this.ActualList.Add(new SerializableDictionary<TK, TV>.KeyAndValue()
          {
            Key = original.Key,
            Value = original.Value
          });
        return this.ActualList;
      }
    }

    public Dictionary<TK, TV> ToDictionary() => this.KeysAndValues.ToDictionary<SerializableDictionary<TK, TV>.KeyAndValue, TK, TV>((Func<SerializableDictionary<TK, TV>.KeyAndValue, TK>) (keyAndValue => keyAndValue.Key), (Func<SerializableDictionary<TK, TV>.KeyAndValue, TV>) (keyAndValue => keyAndValue.Value));

    public class KeyAndValue
    {
      public TK Key { get; set; }

      public TV Value { get; set; }
    }
  }
}

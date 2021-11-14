// Decompiled with JetBrains decompiler
// Type: ExceptionStringTable
// Assembly: System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: AAE9A92C-FB4A-4427-A2C1-2E6256CD1F02
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\System.Windows.Interactivity.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

[DebuggerNonUserCode]
[CompilerGenerated]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
internal class ExceptionStringTable
{
  private static ResourceManager resourceMan;
  private static CultureInfo resourceCulture;

  internal ExceptionStringTable()
  {
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static ResourceManager ResourceManager
  {
    get
    {
      if (object.ReferenceEquals((object) ExceptionStringTable.resourceMan, (object) null))
        ExceptionStringTable.resourceMan = new ResourceManager(nameof (ExceptionStringTable), typeof (ExceptionStringTable).Assembly);
      return ExceptionStringTable.resourceMan;
    }
  }

  [EditorBrowsable(EditorBrowsableState.Advanced)]
  internal static CultureInfo Culture
  {
    get => ExceptionStringTable.resourceCulture;
    set => ExceptionStringTable.resourceCulture = value;
  }

  internal static string CannotHostBehaviorCollectionMultipleTimesExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (CannotHostBehaviorCollectionMultipleTimesExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string CannotHostBehaviorMultipleTimesExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (CannotHostBehaviorMultipleTimesExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string CannotHostTriggerActionMultipleTimesExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (CannotHostTriggerActionMultipleTimesExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string CannotHostTriggerCollectionMultipleTimesExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (CannotHostTriggerCollectionMultipleTimesExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string CannotHostTriggerMultipleTimesExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (CannotHostTriggerMultipleTimesExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string CommandDoesNotExistOnBehaviorWarningMessage => ExceptionStringTable.ResourceManager.GetString(nameof (CommandDoesNotExistOnBehaviorWarningMessage), ExceptionStringTable.resourceCulture);

  internal static string DefaultTriggerAttributeInvalidTriggerTypeSpecifiedExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (DefaultTriggerAttributeInvalidTriggerTypeSpecifiedExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string DuplicateItemInCollectionExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (DuplicateItemInCollectionExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string EventTriggerBaseInvalidEventExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (EventTriggerBaseInvalidEventExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string EventTriggerCannotFindEventNameExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (EventTriggerCannotFindEventNameExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string RetargetedTypeConstraintViolatedExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (RetargetedTypeConstraintViolatedExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string TypeConstraintViolatedExceptionMessage => ExceptionStringTable.ResourceManager.GetString(nameof (TypeConstraintViolatedExceptionMessage), ExceptionStringTable.resourceCulture);

  internal static string UnableToResolveTargetNameWarningMessage => ExceptionStringTable.ResourceManager.GetString(nameof (UnableToResolveTargetNameWarningMessage), ExceptionStringTable.resourceCulture);
}

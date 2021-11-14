// Decompiled with JetBrains decompiler
// Type: Microsoft.LsuPro.SharedControls.PleaseWaitControl
// Assembly: CommonClasses, Version=16.5.3001.0, Culture=neutral, PublicKeyToken=null
// MVID: 9C5F53C6-7909-4C9A-87E3-489AF522B35E
// Assembly location: C:\ProgramData\Microsoft\Lumia Software Updater Pro\Bin\CommonClasses.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.LsuPro.SharedControls
{
  public sealed partial class PleaseWaitControl : UserControl, IComponentConnector
  {
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof (Text), typeof (string), typeof (PleaseWaitControl), (PropertyMetadata) new UIPropertyMetadata((object) nameof (Text)));
    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(nameof (Description), typeof (string), typeof (PleaseWaitControl), (PropertyMetadata) new UIPropertyMetadata((object) nameof (Text)));
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    internal PleaseWaitControl PleaseWaitControlName;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    internal Ellipse Ellipse1;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    internal Ellipse Ellipse2;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    internal Ellipse Ellipse3;
    [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    internal Ellipse Ellipse4;
    private bool _contentLoaded;

    public PleaseWaitControl() => this.InitializeComponent();

    public string Text
    {
      get => (string) this.GetValue(PleaseWaitControl.TextProperty);
      set => this.SetValue(PleaseWaitControl.TextProperty, (object) value);
    }

    public string Description
    {
      get => (string) this.GetValue(PleaseWaitControl.DescriptionProperty);
      set => this.SetValue(PleaseWaitControl.DescriptionProperty, (object) value);
    }

    private void PleaseWaitControlOnIsVisibleChanged(
      object sender,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(this.FindResource((object) "EllipsesAnimation") is Storyboard resource))
        return;
      if (this.Visibility == Visibility.Visible)
        resource.Begin();
      else
        resource.Stop();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/CommonClasses;component/sharedcontrols/pleasewaitcontrol.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
    [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.PleaseWaitControlName = (PleaseWaitControl) target;
          this.PleaseWaitControlName.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.PleaseWaitControlOnIsVisibleChanged);
          break;
        case 2:
          this.Ellipse1 = (Ellipse) target;
          break;
        case 3:
          this.Ellipse2 = (Ellipse) target;
          break;
        case 4:
          this.Ellipse3 = (Ellipse) target;
          break;
        case 5:
          this.Ellipse4 = (Ellipse) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}

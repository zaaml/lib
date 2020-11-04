// <copyright file="FlexBlockDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Panels.Flexible
{
  [ContentProperty(nameof(Definitions))]
  public sealed class FlexBlockDefinition : InheritanceContextObject
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty StretchProperty = DPM.Register<FlexStretch, FlexBlockDefinition>
      ("Stretch", FlexStretch.None, d => d.OnLayoutPropertyChanged);

    public static readonly DependencyProperty SpacingProperty = DPM.Register<double, FlexBlockDefinition>
      ("Spacing", 0.0, d => d.OnLayoutPropertyChanged);

    public static readonly DependencyProperty DistributorProperty = DPM.Register<IFlexDistributor, FlexBlockDefinition>
      ("Distributor", FlexDistributor.Equalizer, d => d.OnLayoutPropertyChanged);

    private static readonly DependencyPropertyKey DefinitionsPropertyKey = DPM.RegisterReadOnly<FlexDefinitionCollection, FlexBlockDefinition>
      ("DefinitionsInt");

    public static readonly DependencyProperty DefinitionsProperty = DefinitionsPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    internal FlexElementCollection SharedFlexElements = new FlexElementCollection();

    #endregion

    #region Properties

    public FlexDefinitionCollection Definitions
    {
      get { return this.GetValueOrCreate(DefinitionsPropertyKey, () => new FlexDefinitionCollection()); }
    }

    public IFlexDistributor Distributor
    {
      get => (IFlexDistributor) GetValue(DistributorProperty);
      set => SetValue(DistributorProperty, value);
    }

    public double Spacing
    {
      get => (double) GetValue(SpacingProperty);
      set => SetValue(SpacingProperty, value);
    }

    public FlexStretch Stretch
    {
      get => (FlexStretch) GetValue(StretchProperty);
      set => SetValue(StretchProperty, value);
    }

    #endregion

    #region  Methods

    private void OnLayoutPropertyChanged()
    {
    }

    #endregion
  }
}
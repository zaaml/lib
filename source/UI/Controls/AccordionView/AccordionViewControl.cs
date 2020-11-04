// <copyright file="AccordionViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.AccordionView
{
  [TemplateContractType(typeof(AccordionViewControlTemplateContract))]
  public class AccordionViewControl : ItemsControlBase<AccordionViewControl, AccordionViewItem, AccordionViewItemCollection, AccordionViewItemsPresenter, AccordionViewPanel>, IHeaderedContentItemsControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SelectionModeProperty = DPM.Register<AccordionViewSelectionMode, AccordionViewControl>
      ("SelectionMode");

    public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<AccordionViewItemGeneratorBase, AccordionViewControl>
      ("ItemGenerator", a => a.OnItemGeneratorChanged);

    public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, AccordionViewControl>
      ("ItemContentTemplate", a => a.DefaultGeneratorImpl.OnItemContentTemplateChanged);

    public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, AccordionViewControl>
      ("ItemContentTemplateSelector", a => a.DefaultGeneratorImpl.OnItemContentTemplateSelectorChanged);

    public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, AccordionViewControl>
      ("ItemContentStringFormat", a => a.DefaultGeneratorImpl.OnItemContentStringFormatChanged);

    public static readonly DependencyProperty ItemHeaderTemplateProperty = DPM.Register<DataTemplate, AccordionViewControl>
      ("ItemHeaderTemplate", a => a.DefaultGeneratorImpl.OnItemHeaderTemplateChanged);

    public static readonly DependencyProperty ItemHeaderTemplateSelectorProperty = DPM.Register<DataTemplateSelector, AccordionViewControl>
      ("ItemHeaderTemplateSelector", a => a.DefaultGeneratorImpl.OnItemHeaderTemplateSelectorChanged);

    public static readonly DependencyProperty ItemHeaderStringFormatProperty = DPM.Register<string, AccordionViewControl>
      ("ItemHeaderStringFormat", a => a.DefaultGeneratorImpl.OnItemHeaderStringFormatChanged);

    public static readonly DependencyProperty ItemsSourceProperty = DPM.Register<IEnumerable, AccordionViewControl>
      ("ItemsSource", i => i.OnItemsSourceChangedPrivate);

    #endregion

    #region Fields

    private DelegateHeaderedContentItemGeneratorImpl<AccordionViewItem, DefaultAccordionViewItemGenerator> _defaultGeneratorImpl;

    #endregion

    #region Ctors

    static AccordionViewControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<AccordionViewControl>();
    }

    public AccordionViewControl()
    {
      this.OverrideStyleKey<AccordionViewControl>();
    }

    #endregion

    #region Properties

    public IEnumerable ItemsSource
    {
      get => (IEnumerable) GetValue(ItemsSourceProperty);
      set => SetValue(ItemsSourceProperty, value);
    }

    private AccordionViewItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

    private AccordionViewItemGeneratorBase DefaultGenerator => DefaultGeneratorImpl.Generator;

    private DelegateHeaderedContentItemGeneratorImpl<AccordionViewItem, DefaultAccordionViewItemGenerator> DefaultGeneratorImpl => _defaultGeneratorImpl ?? (_defaultGeneratorImpl = new DelegateHeaderedContentItemGeneratorImpl<AccordionViewItem, DefaultAccordionViewItemGenerator>(this));

    public AccordionViewItemGeneratorBase ItemGenerator
    {
      get => (AccordionViewItemGeneratorBase) GetValue(ItemGeneratorProperty);
      set => SetValue(ItemGeneratorProperty, value);
    }

    public AccordionViewSelectionMode SelectionMode
    {
      get => (AccordionViewSelectionMode) GetValue(SelectionModeProperty);
      set => SetValue(SelectionModeProperty, value);
    }

    public string ItemContentStringFormat
    {
      get => (string) GetValue(ItemContentStringFormatProperty);
      set => SetValue(ItemContentStringFormatProperty, value);
    }

    public DataTemplate ItemContentTemplate
    {
      get => (DataTemplate) GetValue(ItemContentTemplateProperty);
      set => SetValue(ItemContentTemplateProperty, value);
    }

    public DataTemplateSelector ItemContentTemplateSelector
    {
      get => (DataTemplateSelector) GetValue(ItemContentTemplateSelectorProperty);
      set => SetValue(ItemContentTemplateSelectorProperty, value);
    }

    public string ItemHeaderStringFormat
    {
      get => (string) GetValue(ItemHeaderStringFormatProperty);
      set => SetValue(ItemHeaderStringFormatProperty, value);
    }

    public DataTemplateSelector ItemHeaderTemplateSelector
    {
      get => (DataTemplateSelector) GetValue(ItemHeaderTemplateSelectorProperty);
      set => SetValue(ItemHeaderTemplateSelectorProperty, value);
    }

    public DataTemplate ItemHeaderTemplate
    {
      get => (DataTemplate) GetValue(ItemHeaderTemplateProperty);
      set => SetValue(ItemHeaderTemplateProperty, value);
    }

    #endregion

    #region  Methods

    private void OnItemsSourceChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
    {
      ItemsSourceCore = newSource;
    }

    protected override AccordionViewItemCollection CreateItemCollection()
    {
      return new AccordionViewItemCollection(this)
      {
        Generator = ActualGenerator
      };
    }

    internal override void OnItemAttachedInternal(AccordionViewItem item)
    {
      item.AccordionViewControl = this;

      base.OnItemAttachedInternal(item);
    }

    internal override void OnItemDetachedInternal(AccordionViewItem item)
    {
      base.OnItemDetachedInternal(item);

      item.AccordionViewControl = null;
    }

    internal virtual void OnItemGeneratorChanged(AccordionViewItemGeneratorBase oldGenerator, AccordionViewItemGeneratorBase newGenerator)
    {
      Items.Generator = ActualGenerator;
    }

    #endregion
  }

  public class AccordionViewControlTemplateContract : ItemsControlBaseTemplateContract<AccordionViewItemsPresenter>
  {
  }
}
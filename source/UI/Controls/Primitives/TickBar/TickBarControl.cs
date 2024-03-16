// <copyright file="TickBarControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Primitives.TickBar
{
	[TemplateContractType<TickBarTemplateContract>]
	public class TickBarControl : ItemsControlBase<TickBarControl, TickBarItem, TickBarItemCollection, TickBarItemsPresenter, TickBarPanel>, IContentItemsControl
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, TickBarControl>
			("Orientation");

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<TickBarItemGeneratorBase, TickBarControl>
			("ItemGenerator", t => t.OnItemGeneratorChanged);

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, TickBarControl>
			("SourceCollection", i => i.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty ItemContentTemplateProperty = DPM.Register<DataTemplate, TickBarControl>
			("ItemContentTemplate", a => a.DefaultGeneratorImplementation.OnItemContentTemplateChanged);

		public static readonly DependencyProperty ItemContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, TickBarControl>
			("ItemContentTemplateSelector", a => a.DefaultGeneratorImplementation.OnItemContentTemplateSelectorChanged);

		public static readonly DependencyProperty ItemContentStringFormatProperty = DPM.Register<string, TickBarControl>
			("ItemContentStringFormat", a => a.DefaultGeneratorImplementation.OnItemContentStringFormatChanged);

		public static readonly DependencyProperty ItemContentMemberProperty = DPM.Register<string, TickBarControl>
			("ItemContentMember", d => d.DefaultGeneratorImplementation.OnItemContentMemberChanged);

		private static readonly DependencyPropertyKey SubDivisionsPropertyKey = DPM.RegisterReadOnly<TickBarSubDivisionCollection, TickBarControl>
			("SubDivisionsPrivate");

		public static readonly DependencyProperty DivisionDrawingProperty = DPM.Register<Drawing, TickBarControl>
			("DivisionDrawing", d => d.OnDivisionDrawingPropertyChangedPrivate);

		public static readonly DependencyProperty ItemDockProperty = DPM.Register<TickBarItemDock, TickBarControl>
			("ItemDock", d => d.OnItemDockPropertyChangedPrivate);

		public static readonly DependencyProperty SubDivisionsProperty = SubDivisionsPropertyKey.DependencyProperty;

		private DelegateContentItemGeneratorImplementation<TickBarItem, DefaultTickBarItemGenerator> _defaultGeneratorImplementation;
		private DrawingBrush _divisionBrush;

		static TickBarControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TickBarControl>();
		}

		public TickBarControl()
		{
			this.OverrideStyleKey<TickBarControl>();
		}

		private TickBarItemGeneratorBase ActualGenerator => ItemGenerator ?? DefaultGenerator;

		private TickBarItemGeneratorBase DefaultGenerator => DefaultGeneratorImplementation.Generator;

		private DelegateContentItemGeneratorImplementation<TickBarItem, DefaultTickBarItemGenerator> DefaultGeneratorImplementation =>
			_defaultGeneratorImplementation ??= new DelegateContentItemGeneratorImplementation<TickBarItem, DefaultTickBarItemGenerator>(this);

		public Drawing DivisionDrawing
		{
			get => (Drawing)GetValue(DivisionDrawingProperty);
			set => SetValue(DivisionDrawingProperty, value);
		}

		internal DrawingBrush DivisionDrawingBrush
		{
			get
			{
				var drawing = DivisionDrawing;

				if (drawing == null)
					return null;

				return _divisionBrush ??= new DrawingBrush(DivisionDrawing);
			}
		}

		protected override bool HasLogicalOrientation => true;

		public TickBarItemDock ItemDock
		{
			get => (TickBarItemDock)GetValue(ItemDockProperty);
			set => SetValue(ItemDockProperty, value);
		}

		public TickBarItemGeneratorBase ItemGenerator
		{
			get => (TickBarItemGeneratorBase)GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		protected override Orientation LogicalOrientation => Orientation;

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		public TickBarSubDivisionCollection SubDivisions
		{
			get => this.GetValueOrCreate(SubDivisionsPropertyKey, () => new TickBarSubDivisionCollection(this));
		}

		private TickBarTemplateContract TemplateContract => (TickBarTemplateContract)TemplateContractCore;

		protected override TickBarItemCollection CreateItemCollection()
		{
			return new TickBarItemCollection(this)
			{
				Generator = ActualGenerator
			};
		}

		internal void InvalidateDivisionsInternal()
		{
			ItemsPresenter?.ItemsHostInternal?.InvalidateVisual();
		}

		private void OnDivisionDrawingPropertyChangedPrivate(Drawing oldValue, Drawing newValue)
		{
			InvalidateDivisionsInternal();
		}

		private void OnItemDockPropertyChangedPrivate()
		{
			ItemsPresenter?.ItemsHostInternal?.InvalidateMeasure();
		}

		internal virtual void OnItemGeneratorChanged(TickBarItemGeneratorBase oldGenerator, TickBarItemGeneratorBase newGenerator)
		{
			ItemCollection.Generator = ActualGenerator;
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldSource, IEnumerable newSource)
		{
			SourceCollectionCore = newSource;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.TickBarControl = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.TickBarControl = null;

			base.OnTemplateContractDetaching();
		}

		public string ItemContentMember
		{
			get => (string)GetValue(ItemContentMemberProperty);
			set => SetValue(ItemContentMemberProperty, value);
		}

		public string ItemContentStringFormat
		{
			get => (string)GetValue(ItemContentStringFormatProperty);
			set => SetValue(ItemContentStringFormatProperty, value);
		}

		public DataTemplate ItemContentTemplate
		{
			get => (DataTemplate)GetValue(ItemContentTemplateProperty);
			set => SetValue(ItemContentTemplateProperty, value);
		}

		public DataTemplateSelector ItemContentTemplateSelector
		{
			get => (DataTemplateSelector)GetValue(ItemContentTemplateSelectorProperty);
			set => SetValue(ItemContentTemplateSelectorProperty, value);
		}
	}
}
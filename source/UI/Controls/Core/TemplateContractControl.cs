// <copyright file="TemplateContractControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Reflection;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Core
{
	public abstract class TemplateContractControl : Control
	{
		private TemplateContract _templateContract;

		internal event EventHandler TemplateContractAttached;
		internal event EventHandler TemplateContractDetaching;

		protected internal bool IsTemplateAttached => TemplateContractCore.IsAttached;

		protected TemplateContract TemplateContractCore
		{
			get
			{
				if (_templateContract != null)
					return _templateContract;

				_templateContract = CreateTemplateContract();
				_templateContract.TemplateContractBinder = new TemplateContractBinder(GetTemplateChild);

				return _templateContract;
			}
		}

		internal TemplateContract TemplateContractCoreInternal => TemplateContractCore;

		protected virtual TemplateContract CreateTemplateContract()
		{
			return GetType().GetAttribute<TemplateContractTypeAttribute>().CreateTemplateContractInternal();
		}

		public override void OnApplyTemplate()
		{
			if (TemplateContractCore.IsAttached)
			{
				OnTemplateContractDetaching();
				TemplateContractCore.Detach();
			}

			base.OnApplyTemplate();

			if (Template != null)
			{
				TemplateContractCore.Attach();
				OnTemplateContractAttached();
			}
		}

		protected virtual void OnTemplateContractAttached()
		{
			TemplateContractAttached?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnTemplateContractDetaching()
		{
			TemplateContractDetaching?.Invoke(this, EventArgs.Empty);
		}
	}
}
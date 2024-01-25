// <copyright file="TemplateContractContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Reflection;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Controls.Core
{
	public abstract class TemplateContractContentControl : ContentControl
	{
		private TemplateContract _templateContract;

		internal event EventHandler TemplateContractAttached;
		internal event EventHandler TemplateContractDetaching;

		protected internal bool IsTemplateAttached => TemplateContractInternal.IsAttached;

		protected TemplateContract TemplateContractInternal
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

		protected virtual TemplateContract CreateTemplateContract()
		{
			return GetType().GetAttribute<TemplateContractTypeAttribute>().CreateTemplateContractInternal();
		}

		public sealed override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (TemplateContractInternal.IsAttached)
			{
				OnTemplateContractDetaching();

				TemplateContractInternal.Detach();
			}

			if (Template == null) return;

			TemplateContractInternal.Attach();

			OnTemplateContractAttached();
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
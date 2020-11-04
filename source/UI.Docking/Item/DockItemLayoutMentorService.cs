// <copyright file="DockItemLayoutMentorService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class DockItemLayoutMentorService
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty ServiceProperty = DPM.RegisterAttached<DockItemLayoutMentorService, DockItemLayoutMentorService>
      ("Service");

    #endregion

    #region Fields

    private DockItemGroup _currentMentor;

    #endregion

    #region Ctors

    private DockItemLayoutMentorService(DockItem dockItem)
    {
      DockItem = dockItem;
      Layout = new LayoutSettings();
    }

    #endregion

    #region Properties

    private DockItemGroup CurrentMentor
    {
      set
      {
        if (ReferenceEquals(_currentMentor, value))
          return;

        if (_currentMentor != null)
          LayoutSettings.CopySettings(DockItem, _currentMentor, FullLayout.LayoutProperties);

        _currentMentor = value;

        if (_currentMentor != null)
          LayoutSettings.CopySettings(_currentMentor, DockItem, FullLayout.LayoutProperties);
      }
    }

    private DockItem DockItem { get; }

    public LayoutSettings Layout { get; }

    private List<DockItemGroup> Mentors { get; } = new List<DockItemGroup>();

    #endregion

    #region  Methods

    public void AddMentor(DockItemGroup dockGroup)
    {
      if (Mentors.Count == 0)
        SaveLayout();

      Mentors.Add(dockGroup);
      CurrentMentor = dockGroup;
    }

    public static DockItemLayoutMentorService FromItem(DockItem itemGroup)
    {
      return itemGroup.GetValueOrCreate(ServiceProperty, () => new DockItemLayoutMentorService(itemGroup));
    }

    private void LoadLayout()
    {
      Layout.StoreSettings(DockItem, FullLayout.LayoutProperties);
    }

    public void RemoveMentor(DockItemGroup dockGroup)
    {
      Mentors.Remove(dockGroup);
      CurrentMentor = Mentors.LastOrDefault();

      if (Mentors.Count == 0)
        LoadLayout();
    }

    private void SaveLayout()
    {
      Layout.LoadSettings(DockItem, FullLayout.LayoutProperties);
    }

    #endregion
  }
}
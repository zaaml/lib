// <copyright file="VisualGroups.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Utils;
#if SILVERLIGHT
using System.Xml.Linq;
using Zaaml.PresentationCore;
#endif

namespace Zaaml.UI.Controls.VisualStates
{
  internal enum VisualGroupKind
  {
    Unknown,
    Common,
    Check,
    Focused,
    Selection,
    Expansion,
    Popup,
    Validation,
    ExpandDirection,
    HasItems,
    Increase,
    Decrease,
    InteractionMode,
    Locked,
    Active,
    Watermark,
    CalendarButtonFocus,
    BusyStatus,
    Visibility,
    Day,
		Open
	}

  [Flags]
  public enum VisualStateGroupsDefinition
  {
    CommonStates = 1 << 0,
    CheckStates = 1 << 1,
    FocusStates = 1 << 2,
    SelectionStates = 1 << 3,
    ExpansionStates = 1 << 4,
    PopupStates = 1 << 5,
    ValidationStates = 1 << 6,
    ExpandDirectionStates = 1 << 7,
    HasItemsStates = 1 << 8,
    IncreaseStates = 1 << 9,
    DecreaseStates = 1 << 10,
    InteractionModeStates = 1 << 11,
    LockedStates = 1 << 12,
    ActiveStates = 1 << 13,
    WatermarkStates = 1 << 14,
    CalendarButtonFocusStates = 1 << 15,
    BusyStatusStates = 1 << 16,
    VisibilityStates = 1 << 17,
    DayStates = 1 << 18,
		OpenStates = 1 << 19
  }

  internal static class VisualGroups
  {
    #region Static Fields and Constants

    public const string CommonGroupName = "CommonStates";
    public const string CheckGroupName = "CheckStates";
    public const string FocusGroupName = "FocusStates";
    public const string SelectionGroupName = "SelectionStates";
    public const string ExpansionGroupName = "ExpansionStates";
    public const string PopupGroupName = "PopupStates";
    public const string ValidationGroupName = "ValidationStates";
    public const string ExpandDirectionGroupName = "ExpandDirectionStates";
    public const string HasItemsGroupName = "HasItemsStates";
    public const string IncreaseGroupName = "IncreaseStates";
    public const string DecreaseGroupName = "DecreaseStates";
    public const string InteractionModeGroupName = "InteractionModeStates";
    public const string LockedGroupName = "LockedStates";
    public const string ActiveGroupName = "ActiveStates";
    public const string WatermarkGroupName = "WatermarkStates";
    public const string CalendarButtonFocusGroupName = "CalendarButtonFocusStates";
    public const string DayStatesGroupName = "DayStates";
    public const string BusyStatusGroupName = "BusyStatusStates";
    public const string VisibilityGroupName = "VisibilityStates";
    public const string OpenGroupName = "OpenStates";
    private static readonly TwoWayDictionary<VisualGroupKind, string> GroupDictionary = new TwoWayDictionary<VisualGroupKind, string>();
    private static readonly Dictionary<VisualGroupKind, string[]> GroupStatesDictionary = new Dictionary<VisualGroupKind, string[]>();

#if SILVERLIGHT
    private static readonly Dictionary<string, string> VisualStateGroupTemplates = new Dictionary<string, string>();
    private static readonly XName VisualStateGroupName = XName.Get("VisualStateGroup", XamlConstants.XamlNamespace);
    private static readonly XName VisualStateName = XName.Get("VisualState", XamlConstants.XamlNamespace);
    private static readonly XName NameAttributeName = XName.Get("Name", XamlConstants.XamlXNamespace);
#endif

    private static readonly List<VisualGroup> GroupsList = new List<VisualGroup>
    {
      Common,
      Focus,
      Check,
      Selection,
      HasItemsGroup
    };

    public static readonly string[] CommonGroupStates =
    {
      VisualStates.Normal, VisualStates.ReadOnly, VisualStates.MouseOver, VisualStates.Pressed,
      VisualStates.Disabled
    };

    public static readonly string[] CheckGroupStates = {VisualStates.Checked, VisualStates.Unchecked, VisualStates.Indeterminate};
    public static readonly string[] FocusGroupStates = {VisualStates.Unfocused, VisualStates.Focused};
    public static readonly string[] SelectionGroupStates = {VisualStates.Selected, VisualStates.Unselected, VisualStates.SelectedInactive};
    public static readonly string[] ExpansionGroupStates = {VisualStates.Expanded, VisualStates.Collapsed};
    public static readonly string[] PopupGroupStates = {VisualStates.PopupOpened, VisualStates.PopupClosed};
    public static readonly string[] ValidationGroupStates = {VisualStates.Valid, VisualStates.InvalidFocused, VisualStates.InvalidUnfocused, VisualStates.HasErrors, VisualStates.Empty};

    public static readonly string[] ExpandDirectionGroupStates =
    {
      VisualStates.ExpandDown,
			VisualStates.ExpandUp,
			VisualStates.ExpandLeft,
      VisualStates.ExpandRight
    };

    public static readonly string[] HasItemsGroupStates = {VisualStates.HasItems, VisualStates.NoItems};
    public static readonly string[] IncreaseGroupStates = {VisualStates.IncreaseEnabled, VisualStates.IncreaseDisabled};
    public static readonly string[] DecreaseGroupStates = {VisualStates.DecreaseEnabled, VisualStates.DecreaseDisabled};
    public static readonly string[] InteractionModeGroupStates = {VisualStates.Edit, VisualStates.Display};
    public static readonly string[] LockedGroupStates = {VisualStates.Locked, VisualStates.Unlocked};
    public static readonly string[] ActiveGroupStates = {VisualStates.Active, VisualStates.Inactive};
    public static readonly string[] WatermarkGroupStates = {VisualStates.Unwatermarked, VisualStates.Watermarked};
    public static readonly string[] CalendarButtonFocusGroupStates = {VisualStates.CalendarButtonUnfocused, VisualStates.CalendarButtonFocused};
    public static readonly string[] BusyStatusGroupStates = {VisualStates.Busy, VisualStates.Idle};
    public static readonly string[] VisibilityGroupStates = {VisualStates.Visible, VisualStates.Hidden};
    public static readonly string[] DayGroupStates = {VisualStates.RegularDay, VisualStates.Today};
    public static readonly string[] OpenGroupStates = {VisualStates.Open, VisualStates.Closed};

    #endregion

    #region Ctors

    static VisualGroups()
    {
      // Group names
      GroupDictionary.Add(VisualGroupKind.Common, CommonGroupName);
      GroupDictionary.Add(VisualGroupKind.Check, CheckGroupName);
      GroupDictionary.Add(VisualGroupKind.Focused, FocusGroupName);
      GroupDictionary.Add(VisualGroupKind.Selection, SelectionGroupName);
      GroupDictionary.Add(VisualGroupKind.Expansion, ExpansionGroupName);
      GroupDictionary.Add(VisualGroupKind.Popup, PopupGroupName);
      GroupDictionary.Add(VisualGroupKind.Validation, ValidationGroupName);
      GroupDictionary.Add(VisualGroupKind.ExpandDirection, ExpandDirectionGroupName);
      GroupDictionary.Add(VisualGroupKind.HasItems, HasItemsGroupName);
      GroupDictionary.Add(VisualGroupKind.Increase, IncreaseGroupName);
      GroupDictionary.Add(VisualGroupKind.Decrease, DecreaseGroupName);
      GroupDictionary.Add(VisualGroupKind.InteractionMode, InteractionModeGroupName);
      GroupDictionary.Add(VisualGroupKind.Locked, LockedGroupName);
      GroupDictionary.Add(VisualGroupKind.Active, ActiveGroupName);
      GroupDictionary.Add(VisualGroupKind.Watermark, WatermarkGroupName);
      GroupDictionary.Add(VisualGroupKind.CalendarButtonFocus, CalendarButtonFocusGroupName);
      GroupDictionary.Add(VisualGroupKind.BusyStatus, BusyStatusGroupName);
      GroupDictionary.Add(VisualGroupKind.Visibility, VisibilityGroupName);
      GroupDictionary.Add(VisualGroupKind.Day, DayStatesGroupName);
      GroupDictionary.Add(VisualGroupKind.Open, OpenGroupName);

      // Group states
      GroupStatesDictionary.Add(VisualGroupKind.Common, CommonGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Check, CheckGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Focused, FocusGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Selection, SelectionGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Expansion, ExpansionGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Popup, PopupGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Validation, ValidationGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.ExpandDirection, ExpandDirectionGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.HasItems, HasItemsGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Increase, IncreaseGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Decrease, DecreaseGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.InteractionMode, InteractionModeGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Locked, LockedGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Active, ActiveGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Watermark, WatermarkGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.CalendarButtonFocus, CalendarButtonFocusGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.BusyStatus, BusyStatusGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Visibility, VisibilityGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Day, DayGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Open, OpenGroupStates);
    }

    #endregion

    #region Properties

    public static VisualGroup Common => CommonVisualGroup.Instance;

    public static VisualGroup Focus => FocusVisualGroup.Instance;

    public static VisualGroup Check => CheckVisualGroup.Instance;

    public static VisualGroup Selection => SelectionStatesGroup.Instance;

    public static VisualGroup HasItemsGroup => HasItemsStatesGroup.Instance;

    public static IEnumerable<VisualGroup> Groups => GroupsList;

    #endregion

    #region  Methods

    public static string GetGroupName(VisualGroupKind kind)
    {
      return GroupDictionary.GetValueOrDefault(kind);
    }

    public static VisualGroupKind GetGroupKind(string groupName)
    {
      return GroupDictionary.GetKeyOrDefault(groupName);
    }

    public static string[] GetStates(VisualGroupKind kind)
    {
      return GroupStatesDictionary.GetValueOrDefault(kind);
    }

    public static string[] GetStates(string groupName)
    {
      return GroupStatesDictionary.GetValueOrDefault(GetGroupKind(groupName));
    }

#if SILVERLIGHT
    private static string CreateVisualStateTemplate(string groupName)
    {
      var groupKind = GetGroupKind(groupName);
      if (groupKind == VisualGroupKind.Unknown)
        return null;

      var groupNode = new XElement(VisualStateGroupName, new XAttribute(NameAttributeName, groupName));

      foreach (var state in GetStates(groupKind))
        groupNode.Add(new XElement(VisualStateName, new XAttribute(NameAttributeName, state)));

      return groupNode.ToString();
    }
#endif

    internal static VisualStateGroup BuildVisualStateGroup(string groupName)
    {
#if SILVERLIGHT
      var template = VisualStateGroupTemplates.GetValueOrCreate(groupName, CreateVisualStateTemplate);
      return template == null ? null : XamlUtils.Load<VisualStateGroup>(template);
#else
      var groupKind = GetGroupKind(groupName);
      if (groupKind == VisualGroupKind.Unknown)
        return null;

      var visualGroup = new VisualStateGroup {Name = groupName};

      foreach (var state in GetStates(groupKind))
        visualGroup.States.Add(new VisualState {Name = state});

      return visualGroup;
#endif
    }

    internal static IEnumerable<string> EnumerateGroups(VisualStateGroupsDefinition groupsDefinition)
    {
      return groupsDefinition.GetFlags().Select(groupFlag => groupFlag.ToString());
    }

    internal static IEnumerable<VisualStateGroup> CreateVisualStateGroups(VisualStateGroupsDefinition groupsDefinition)
    {
      return EnumerateGroups(groupsDefinition).Select(BuildVisualStateGroup).SkipNull();
    }

    #endregion
  }
}
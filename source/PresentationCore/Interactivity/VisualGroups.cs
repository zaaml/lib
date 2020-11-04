// <copyright file="VisualGroups.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;

#if SILVERLIGHT
using System.Xml.Linq;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Utils;
#endif

using ZaamlVisualStateGroup = Zaaml.PresentationCore.Interactivity.VSM.VisualStateGroup;
using ZaamlVisualState = Zaaml.PresentationCore.Interactivity.VSM.VisualState;

namespace Zaaml.PresentationCore.Interactivity
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
    BlackoutDay,
		Open,
    Sort
  }

  [Flags]
  public enum VisualStateGroupKind
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
	  DayStates = 1 << 16,
	  BlackoutDayStates = 1 << 17,
		BusyStatusStates = 1 << 18,
    VisibilityStates = 1 << 19,
		OpenStates = 1 << 20,
    SortStates = 1 << 21,
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
    public const string BlackoutDayStatesGroupName = "BlackoutDayStates";
    public const string BusyStatusGroupName = "BusyStatusStates";
    public const string VisibilityGroupName = "VisibilityStates";
    public const string OpenGroupName = "OpenStates";
    public const string SortGroupName = "SortStates";

	  private static readonly List<string> GroupNamesArray;
	  private static readonly List<ZaamlVisualStateGroup> GroupsArray;

    private static readonly TwoWayDictionary<VisualGroupKind, string> GroupDictionary = new TwoWayDictionary<VisualGroupKind, string>();
    private static readonly Dictionary<VisualGroupKind, string[]> GroupStatesDictionary = new Dictionary<VisualGroupKind, string[]>();

#if SILVERLIGHT
    private static readonly Dictionary<string, string> VisualStateGroupTemplates = new Dictionary<string, string>();
    private static readonly XName VisualStateGroupName = XName.Get("VisualStateGroup", XamlConstants.XamlNamespace);
    private static readonly XName VisualStateName = XName.Get("VisualState", XamlConstants.XamlNamespace);
    private static readonly XName NameAttributeName = XName.Get("Name", XamlConstants.XamlXNamespace);
#endif



    public static readonly string[] CommonGroupStates =
    {
      CommonVisualStates.Normal, CommonVisualStates.ReadOnly, CommonVisualStates.MouseOver, CommonVisualStates.Pressed,
      CommonVisualStates.Disabled
    };

    public static readonly string[] CheckGroupStates = {CommonVisualStates.Checked, CommonVisualStates.Unchecked, CommonVisualStates.Indeterminate};
    public static readonly string[] FocusGroupStates = {CommonVisualStates.Unfocused, CommonVisualStates.Focused};
    public static readonly string[] SelectionGroupStates = {CommonVisualStates.Selected, CommonVisualStates.Unselected, CommonVisualStates.SelectedInactive};
    public static readonly string[] ExpansionGroupStates = {CommonVisualStates.Expanded, CommonVisualStates.Collapsed};
    public static readonly string[] PopupGroupStates = {CommonVisualStates.PopupOpened, CommonVisualStates.PopupClosed};
    public static readonly string[] ValidationGroupStates = {CommonVisualStates.Valid, CommonVisualStates.InvalidFocused, CommonVisualStates.InvalidUnfocused, CommonVisualStates.HasErrors, CommonVisualStates.Empty};

    public static readonly string[] ExpandDirectionGroupStates =
    {
      CommonVisualStates.ExpandDown,
			CommonVisualStates.ExpandUp,
			CommonVisualStates.ExpandLeft,
      CommonVisualStates.ExpandRight
    };

    public static readonly string[] HasItemsGroupStates = {CommonVisualStates.HasItems, CommonVisualStates.NoItems};
    public static readonly string[] IncreaseGroupStates = {CommonVisualStates.IncreaseEnabled, CommonVisualStates.IncreaseDisabled};
    public static readonly string[] DecreaseGroupStates = {CommonVisualStates.DecreaseEnabled, CommonVisualStates.DecreaseDisabled};
    public static readonly string[] InteractionModeGroupStates = {CommonVisualStates.Edit, CommonVisualStates.Display};
    public static readonly string[] LockedGroupStates = {CommonVisualStates.Locked, CommonVisualStates.Unlocked};
    public static readonly string[] ActiveGroupStates = {CommonVisualStates.Active, CommonVisualStates.Inactive};
    public static readonly string[] WatermarkGroupStates = {CommonVisualStates.Unwatermarked, CommonVisualStates.Watermarked};
    public static readonly string[] CalendarButtonFocusGroupStates = {CommonVisualStates.CalendarButtonUnfocused, CommonVisualStates.CalendarButtonFocused};
    public static readonly string[] BusyStatusGroupStates = {CommonVisualStates.Busy, CommonVisualStates.Idle};
    public static readonly string[] VisibilityGroupStates = {CommonVisualStates.Visible, CommonVisualStates.Hidden};
    public static readonly string[] DayGroupStates = {CommonVisualStates.RegularDay, CommonVisualStates.Today};
    public static readonly string[] BlackoutDayGroupStates = {CommonVisualStates.NormalDay, CommonVisualStates.BlackoutDay};
    public static readonly string[] OpenGroupStates = {CommonVisualStates.Open, CommonVisualStates.Closed};
    public static readonly string[] SortGroupStates = {CommonVisualStates.Unsorted, CommonVisualStates.SortAscending, CommonVisualStates.SortDescending};

    #endregion

    #region Ctors

    static VisualGroups()
    {
	    GroupNamesArray = new List<string>
	    {
		    CommonGroupName,
		    CheckGroupName,
		    FocusGroupName,
		    SelectionGroupName,
		    ExpansionGroupName,
		    PopupGroupName,
		    ValidationGroupName,
		    ExpandDirectionGroupName,
		    HasItemsGroupName,
		    IncreaseGroupName,
		    DecreaseGroupName,
		    InteractionModeGroupName,
		    LockedGroupName,
		    ActiveGroupName,
		    WatermarkGroupName,
		    CalendarButtonFocusGroupName,
		    DayStatesGroupName,
		    BlackoutDayStatesGroupName,
				BusyStatusGroupName,
		    VisibilityGroupName,
		    OpenGroupName,
		    SortGroupName
	    };

   

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
	    GroupDictionary.Add(VisualGroupKind.Day, DayStatesGroupName);
	    GroupDictionary.Add(VisualGroupKind.BlackoutDay, BlackoutDayStatesGroupName);
			GroupDictionary.Add(VisualGroupKind.BusyStatus, BusyStatusGroupName);
      GroupDictionary.Add(VisualGroupKind.Visibility, VisibilityGroupName);
      GroupDictionary.Add(VisualGroupKind.Open, OpenGroupName);
      GroupDictionary.Add(VisualGroupKind.Sort, SortGroupName);

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
	    GroupStatesDictionary.Add(VisualGroupKind.Day, DayGroupStates);
	    GroupStatesDictionary.Add(VisualGroupKind.BlackoutDay, BlackoutDayGroupStates);
			GroupStatesDictionary.Add(VisualGroupKind.BusyStatus, BusyStatusGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Visibility, VisibilityGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Open, OpenGroupStates);
      GroupStatesDictionary.Add(VisualGroupKind.Sort, SortGroupStates);

	    GroupsArray = new List<ZaamlVisualStateGroup>(GroupNamesArray.Select(g => new ZaamlVisualStateGroup(g, GetStates(g))));
		}

    #endregion

    #region Properties



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

  //  internal static IEnumerable<string> EnumerateGroups(VisualStateGroupsDefinition groupsDefinition)
  //  {
		//	var groups = (int)groupsDefinition;

		//	for (var i = 0; i < GroupNamesArray.Count; i++)
		//	{
		//		if ((groups & i) != 0)
		//			yield return GroupNamesArray[i];
		//	}
		//}

	  internal static IEnumerable<ZaamlVisualStateGroup> EnumerateGroups(VisualStateGroupKind groupKind)
	  {
		  var groups = (int)groupKind;

		  var f = 1;

		  foreach (var group in GroupsArray)
		  {
			  if ((groups & f) != 0)
				  yield return group;

			  if (f > groups)
				  yield break;

			  f <<= 1;
		  }
	  }

		//internal static IEnumerable<VisualStateGroup> CreateVisualStateGroups(VisualStateGroupsDefinition groupsDefinition)
  //  {
  //    return EnumerateGroups(groupsDefinition).Select(BuildVisualStateGroup).SkipNull();
  //  }

    #endregion
  }

	internal sealed class VisualStateGroupDescription
	{
		public string GroupName { get; }

		public VisualStateGroupDescription(string groupName, IEnumerable<string> visualStates)
		{
			GroupName = groupName;
			VisualStates = visualStates.ToArray();
		}

		public string [] VisualStates { get; }
	}
}

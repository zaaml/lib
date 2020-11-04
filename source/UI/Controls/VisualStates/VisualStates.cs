namespace Zaaml.UI.Controls.VisualStates
{
  internal static class VisualStates
  {
    // Common States Group
    public const string Normal = "Normal";
    public const string ReadOnly = "ReadOnly";
    public const string MouseOver = "MouseOver";
    public const string Pressed = "Pressed";
    public const string Disabled = "Disabled";

    // Check States Group
    public const string Checked = "Checked";
    public const string Unchecked = "Unchecked";
	  public const string Indeterminate = "Indeterminate";

    // Focus States Group
    public const string Unfocused = "Unfocused";
    public const string Focused = "Focused";

    // Selection States Group
    public const string Selected = "Selected";
    public const string Unselected = "Unselected";
    public const string SelectedInactive = "SelectedInactive";

    // Expansion States Group
    public const string Expanded = "Expanded";
    public const string Collapsed = "Collapsed";

    // Popup States Group
    public const string PopupOpened = "PopupOpened";
    public const string PopupClosed = "PopupClosed";

    // Validation States Group
    public const string Valid = "Valid";
    public const string InvalidFocused = "InvalidFocused";
    public const string InvalidUnfocused = "InvalidUnfocused";
		public const string HasErrors = "HasErrors";
		public const string Empty = "Empty";

    // ExpandDirection States Group
    public const string ExpandDown = "ExpandDown";
    public const string ExpandUp = "ExpandUp";
    public const string ExpandLeft = "ExpandLeft";
    public const string ExpandRight = "ExpandRight";
   
    // HasItems States Group
    public const string HasItems = "HasItems";
    public const string NoItems = "NoItems";

    // Increase States Group
    public const string IncreaseEnabled = "IncreaseEnabled";
    public const string IncreaseDisabled = "IncreaseDisabled";

    // Decrease States Group
    public const string DecreaseEnabled = "DecreaseEnabled";
    public const string DecreaseDisabled = "DecreaseDisabled";

    // InteractionMode States Group
    public const string Edit = "Edit";
    public const string Display = "Display";

    // Locked States Group
    public const string Locked = "Locked";
    public const string Unlocked = "Unlocked";

    // Active States Group
    public const string Active = "Active";
    public const string Inactive = "Inactive";

    // Watermark States Group
    public const string Unwatermarked = "Unwatermarked";
    public const string Watermarked = "Watermarked";
    
    // CalendarButtonFocus States Group
    public const string CalendarButtonUnfocused = "CalendarButtonUnfocused";
    public const string CalendarButtonFocused = "CalendarButtonFocused";

    // BusyStatus States Group
    public const string Busy = "Busy";
    public const string Idle = "Idle";

    // Visibility States Group
    public const string Visible = "Visible";
    public const string Hidden = "Hidden";

		// Day States Group
	  public const string RegularDay = "RegularDay";
	  public const string Today = "Today";

		// Open States Group
		public const string Open = "Open";
		public const string Closed = "Closed";
	}
}

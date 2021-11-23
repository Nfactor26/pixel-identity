namespace Pixel.Identity.Shared.ViewModels
{
    public class SwitchItemViewModel
    {
        public string DisplayName { get; private set; }

        public string ItemValue { get; private set; }

        public bool IsSelected { get; set; }

        public SwitchItemViewModel(string itemValue, bool isSelected)
        {
            this.ItemValue = itemValue;
            this.IsSelected = isSelected;
            this.DisplayName = itemValue.Contains(":") ? itemValue.Split(":")[1] : itemValue;
        }

        public SwitchItemViewModel(string displayName, string itemValue, bool isSelected)
        {
            this.ItemValue = itemValue;
            this.IsSelected = isSelected;
            this.DisplayName = displayName;
        }
    }
}

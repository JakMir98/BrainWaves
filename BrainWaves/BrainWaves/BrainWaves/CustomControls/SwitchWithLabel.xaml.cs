using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SwitchWithLabel : ContentView
    {
        public SwitchWithLabel()
        {
            InitializeComponent();
        }

        #region Stack parent
        public static readonly BindableProperty CustomBackgroundColorProperty =
            BindableProperty.Create(
                nameof(BackgroundColorProp),
                typeof(Color),
                typeof(SwitchWithLabel),
                defaultValue: Color.Transparent,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: BackgroundColorPropertyChanged);

        private static void BackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.BackgroundColor = (Color)newValue;
        }

        public Color BackgroundColorProp
        {
            get => (Color)GetValue(CustomBackgroundColorProperty);
            set => SetValue(CustomBackgroundColorProperty, value);
        }

        public static readonly BindableProperty HorizontalOptionProperty = BindableProperty.Create(nameof(HorizontalOptionProperty),
            typeof(LayoutOptions),
            typeof(SwitchWithLabel),
            propertyChanged: HorizontalOptionPropertyChanged);

        private static void HorizontalOptionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.StackParent.HorizontalOptions = (LayoutOptions)newValue;
        }

        public LayoutOptions HorizontalOptionsProp
        {
            get => (LayoutOptions)GetValue(HorizontalOptionProperty);
            set => SetValue(HorizontalOptionProperty, value);
        }

        public static readonly BindableProperty VerticalOptionProperty = BindableProperty.Create(nameof(VerticalOptionProperty),
            typeof(LayoutOptions),
            typeof(SwitchWithLabel),
            propertyChanged: VerticalOptionPropertyChanged);

        private static void VerticalOptionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.StackParent.VerticalOptions = (LayoutOptions)newValue;
        }

        public LayoutOptions VerticalOptionsProp
        {
            get => (LayoutOptions)GetValue(VerticalOptionProperty);
            set => SetValue(VerticalOptionProperty, value);
        }
        #endregion

        #region LeftLabel
        public static readonly BindableProperty LeftTitleTextProperty =
            BindableProperty.Create(
                nameof(LeftTitleText),
                typeof(string),
                typeof(SwitchWithLabel),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftTitleTextPropertyChanged);

        private static void LeftTitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.LeftSwitchText.Text = newValue?.ToString();
        }

        public string LeftTitleText
        {
            get => GetValue(LeftTitleTextProperty)?.ToString();
            set => SetValue(LeftTitleTextProperty, value);
        }

        public static readonly BindableProperty LeftLabelTextColorProperty =
            BindableProperty.Create(
                nameof(LeftLabelTextColorProp),
                typeof(Color),
                typeof(SwitchWithLabel),
                defaultValue: Color.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftLabelTextColorPropertyChanged);

        private static void LeftLabelTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.LeftSwitchText.TextColor = (Color)newValue;
        }

        public Color LeftLabelTextColorProp
        {
            get => (Color)GetValue(LeftLabelTextColorProperty);
            set => SetValue(LeftLabelTextColorProperty, value);
        }

        public static readonly BindableProperty LeftLabelIsVisibleProperty =
            BindableProperty.Create(
                nameof(LeftLabelIsVisible),
                typeof(bool),
                typeof(SwitchWithLabel),
                defaultValue: true,
                propertyChanged: LeftLabelIsVisiblePropertyChanged);

        private static void LeftLabelIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.LeftSwitchText.IsVisible = (bool)newValue;
        }

        public bool LeftLabelIsVisible
        {
            get => (bool)GetValue(LeftLabelIsVisibleProperty);
            set => SetValue(LeftLabelIsVisibleProperty, value);
        }
        #endregion

        #region RightLabel
        public static readonly BindableProperty RightTitleTextProperty =
            BindableProperty.Create(
                nameof(RightTitleText),
                typeof(string),
                typeof(SwitchWithLabel),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightTitleTextPropertyChanged);

        private static void RightTitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.RightSwitchText.Text = newValue?.ToString();
        }

        public string RightTitleText
        {
            get => GetValue(RightTitleTextProperty)?.ToString();
            set => SetValue(RightTitleTextProperty, value);
        }

        public static readonly BindableProperty RightLabelTextColorProperty =
            BindableProperty.Create(
                nameof(RightLabelTextColorProp),
                typeof(Color),
                typeof(SwitchWithLabel),
                defaultValue: Color.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightLabelTextColorPropertyChanged);

        private static void RightLabelTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.RightSwitchText.TextColor = (Color)newValue;
        }

        public Color RightLabelTextColorProp
        {
            get => (Color)GetValue(RightLabelTextColorProperty);
            set => SetValue(RightLabelTextColorProperty, value);
        }

        public static readonly BindableProperty RightLabelIsVisibleProperty =
            BindableProperty.Create(
                nameof(RightLabelIsVisible),
                typeof(bool),
                typeof(SwitchWithLabel),
                defaultValue: false,
                propertyChanged: RightLabelIsVisiblePropertyChanged);

        private static void RightLabelIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.RightSwitchText.IsVisible = (bool)newValue;
        }

        public bool RightLabelIsVisible
        {
            get => (bool)GetValue(RightLabelIsVisibleProperty);
            set => SetValue(RightLabelIsVisibleProperty, value);
        }
        #endregion

        #region Switch
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked),
            typeof(bool),
            typeof(SwitchWithLabel),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsCheckedPropertyChanged);

        private static void IsCheckedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.SwitchToggle.IsToggled = (bool)newValue;
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        #endregion
    }
}
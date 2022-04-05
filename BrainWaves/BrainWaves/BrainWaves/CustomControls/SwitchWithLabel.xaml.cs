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

        public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(nameof(TitleText),
            typeof(string),
            typeof(SwitchWithLabel),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: TitleTextPropertyChanged);

        private static void TitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchWithLabel)bindable;
            control.SwitchText.Text = newValue?.ToString();
        }

        public string TitleText
        {
            get => GetValue(TitleTextProperty)?.ToString();
            set => SetValue(TitleTextProperty, value);
        }

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
    }
}
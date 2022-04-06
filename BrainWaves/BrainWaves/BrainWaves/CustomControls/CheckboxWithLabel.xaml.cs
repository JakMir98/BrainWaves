using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckboxWithLabel : ContentView
    {
        public CheckboxWithLabel()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CustomBackgroundColorProperty = 
            BindableProperty.Create(
                nameof(BackgroundColorProp),
                typeof(Color),
                typeof(CheckboxWithLabel),
                defaultValue: Color.Transparent,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: BackgroundColorPropertyChanged);

        private static void BackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckboxWithLabel)bindable;
            control.BackgroundColor = (Color)newValue;
        }

        public Color BackgroundColorProp
        {
            get => (Color)GetValue(CustomBackgroundColorProperty);
            set => SetValue(CustomBackgroundColorProperty, value);
        }

        public static readonly BindableProperty TitleTextProperty =
            BindableProperty.Create(
                nameof(TitleText),
                typeof(string),
                typeof(CheckboxWithLabel),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: TitleTextPropertyChanged);

        private static void TitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckboxWithLabel)bindable;
            control.SwitchText.Text = newValue?.ToString();
        }

        public string TitleText
        {
            get => GetValue(TitleTextProperty)?.ToString();
            set => SetValue(TitleTextProperty, value);
        }

        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked),
            typeof(bool),
            typeof(CheckboxWithLabel),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: IsCheckedPropertyChanged);

        private static void IsCheckedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckboxWithLabel)bindable;
            control.CheckboxToggle.IsChecked = (bool)newValue;
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public static readonly BindableProperty HorizontalOptionProperty = BindableProperty.Create(nameof(HorizontalOptionProperty),
            typeof(LayoutOptions),
            typeof(CheckboxWithLabel),
            propertyChanged: HorizontalOptionPropertyChanged);

        private static void HorizontalOptionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckboxWithLabel)bindable;
            control.StackParent.HorizontalOptions = (LayoutOptions)newValue;
        }

        public LayoutOptions HorizontalOptionsProp
        {
            get => (LayoutOptions)GetValue(HorizontalOptionProperty);
            set => SetValue(HorizontalOptionProperty, value);
        }

        public static readonly BindableProperty VerticalOptionProperty = BindableProperty.Create(nameof(VerticalOptionProperty),
            typeof(LayoutOptions),
            typeof(CheckboxWithLabel),
            propertyChanged: VerticalOptionPropertyChanged);

        private static void VerticalOptionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckboxWithLabel)bindable;
            control.StackParent.VerticalOptions = (LayoutOptions)newValue;
        }

        public LayoutOptions VerticalOptionsProp
        {
            get => (LayoutOptions)GetValue(VerticalOptionProperty);
            set => SetValue(VerticalOptionProperty, value);
        }
    }
}
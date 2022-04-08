using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ActivityIndicatorWithLabel : ContentView
    {
        public ActivityIndicatorWithLabel()
        {
            InitializeComponent();
        }

        #region Stack parent
        public static readonly BindableProperty CustomBackgroundColorProperty =
            BindableProperty.Create(
                nameof(BackgroundColorProp),
                typeof(Color),
                typeof(ActivityIndicatorWithLabel),
                defaultValue: Color.Transparent,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: BackgroundColorPropertyChanged);

        private static void BackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.BackgroundColor = (Color)newValue;
        }

        public Color BackgroundColorProp
        {
            get => (Color)GetValue(CustomBackgroundColorProperty);
            set => SetValue(CustomBackgroundColorProperty, value);
        }

        public static readonly BindableProperty HorizontalOptionProperty = 
            BindableProperty.Create(
                nameof(HorizontalOptionsProp),
                typeof(LayoutOptions),
                typeof(ActivityIndicatorWithLabel),
                propertyChanged: HorizontalOptionPropertyChanged);

        private static void HorizontalOptionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.StackParent.HorizontalOptions = (LayoutOptions)newValue;
        }

        public LayoutOptions HorizontalOptionsProp
        {
            get => (LayoutOptions)GetValue(HorizontalOptionProperty);
            set => SetValue(HorizontalOptionProperty, value);
        }

        public static readonly BindableProperty VerticalOptionProperty = 
            BindableProperty.Create(
                nameof(VerticalOptionsProp),
                typeof(LayoutOptions),
                typeof(ActivityIndicatorWithLabel),
                propertyChanged: VerticalOptionPropertyChanged);

        private static void VerticalOptionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.StackParent.VerticalOptions = (LayoutOptions)newValue;
        }

        public LayoutOptions VerticalOptionsProp
        {
            get => (LayoutOptions)GetValue(VerticalOptionProperty);
            set => SetValue(VerticalOptionProperty, value);
        }
        #endregion

        #region Labels
        public static readonly BindableProperty LabelTextColorProperty =
            BindableProperty.Create(
                nameof(LabelTextColorProp),
                typeof(Color),
                typeof(ActivityIndicatorWithLabel),
                defaultValue: Color.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LabelTextColorPropertyChanged);

        private static void LabelTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.LeftLabel.TextColor = (Color)newValue;
            control.RightLabel.TextColor = (Color)newValue;
        }

        public Color LabelTextColorProp
        {
            get => (Color)GetValue(LabelTextColorProperty);
            set => SetValue(LabelTextColorProperty, value);
        }
        #region Left label
        public static readonly BindableProperty LeftTitleTextProperty =
            BindableProperty.Create(
                nameof(LeftTitleText),
                typeof(string),
                typeof(ActivityIndicatorWithLabel),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftTitleTextPropertyChanged);

        private static void LeftTitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.LeftLabel.Text = newValue?.ToString();
        }

        public string LeftTitleText
        {
            get => GetValue(LeftTitleTextProperty)?.ToString();
            set => SetValue(LeftTitleTextProperty, value);
        }

        public static readonly BindableProperty LeftLabelIsVisibleProperty = 
            BindableProperty.Create(
                nameof(LeftLabelIsVisible),
                typeof(bool),
                typeof(ActivityIndicatorWithLabel),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftLabelIsVisiblePropertyChanged);

        private static void LeftLabelIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.LeftLabel.IsVisible = (bool)newValue;
        }

        public bool LeftLabelIsVisible
        {
            get => (bool)GetValue(LeftLabelIsVisibleProperty);
            set => SetValue(LeftLabelIsVisibleProperty, value);
        }
        #endregion

        #region Right label
        public static readonly BindableProperty RightTitleTextProperty =
            BindableProperty.Create(
                nameof(RightTitleText),
                typeof(string),
                typeof(ActivityIndicatorWithLabel),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightTitleTextPropertyChanged);

        private static void RightTitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.RightLabel.Text = newValue?.ToString();
        }

        public string RightTitleText
        {
            get => GetValue(RightTitleTextProperty)?.ToString();
            set => SetValue(RightTitleTextProperty, value);
        }

        public static readonly BindableProperty RightLabelIsVisibleProperty = 
            BindableProperty.Create(
                nameof(RightLabelIsVisible),
                typeof(bool),
                typeof(ActivityIndicatorWithLabel),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightLabelIsVisiblePropertyChanged);

        private static void RightLabelIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.RightLabel.IsVisible = (bool)newValue;
        }

        public bool RightLabelIsVisible
        {
            get => (bool)GetValue(RightLabelIsVisibleProperty);
            set => SetValue(RightLabelIsVisibleProperty, value);
        }
        #endregion

        #region Activity indicator
        public static readonly BindableProperty ActivityIndicatorColorProperty =
            BindableProperty.Create(
                nameof(ActivityIndicatorColorProp),
                typeof(Color),
                typeof(ActivityIndicatorWithLabel),
                defaultValue: Color.Gray,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ActivityIndicatorColorPropertyChanged);

        private static void ActivityIndicatorColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.ActivityIndicator.Color = (Color)newValue;
        }

        public Color ActivityIndicatorColorProp
        {
            get => (Color)GetValue(ActivityIndicatorColorProperty);
            set => SetValue(ActivityIndicatorColorProperty, value);
        }

        public static readonly BindableProperty ActivityIndicatorIsVisibleProperty = 
            BindableProperty.Create(
               nameof(ActivityIndicatorIsVisible),
               typeof(bool),
               typeof(ActivityIndicatorWithLabel),
               defaultValue: true,
               defaultBindingMode: BindingMode.OneWay,
               propertyChanged: ActivityIndicatorIsVisiblePropertyChanged);

        private static void ActivityIndicatorIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.ActivityIndicator.IsVisible = (bool)newValue;
        }

        public bool ActivityIndicatorIsVisible
        {
            get => (bool)GetValue(ActivityIndicatorIsVisibleProperty);
            set => SetValue(ActivityIndicatorIsVisibleProperty, value);
        }

        public static readonly BindableProperty ActivityIndicatorScaleProperty = 
            BindableProperty.Create(
               nameof(ActivityIndicatorScale),
               typeof(double),
               typeof(ActivityIndicatorWithLabel),
               defaultValue: 1.0,
               defaultBindingMode: BindingMode.OneWay,
               propertyChanged: ActivityIndicatorScalePropertyChanged);

        private static void ActivityIndicatorScalePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (ActivityIndicatorWithLabel)bindable;
            control.ActivityIndicator.Scale = (double)newValue;
        }

        public double ActivityIndicatorScale
        {
            get => (double)GetValue(ActivityIndicatorScaleProperty);
            set => SetValue(ActivityIndicatorScaleProperty, value);
        }
        #endregion
        #endregion
    }
}
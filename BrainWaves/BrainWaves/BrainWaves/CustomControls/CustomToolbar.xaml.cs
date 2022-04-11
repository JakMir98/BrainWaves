using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomToolbar : ContentView
    {
        private static Color DefaultBorderColor = Color.Silver;
        private static double DefaultBorderWidth = 0.5;
        private static int DefaultCornerRadius = 10;
        private static double DefaultLabelTextSize = 20.0;

        public CustomToolbar()
        {
            InitializeComponent();
        }

        #region Stack Parent
        public static readonly BindableProperty CustomBackgroundColorProperty =
            BindableProperty.Create(
                nameof(BackgroundColorProp),
                typeof(Color),
                typeof(CustomToolbar),
                defaultValue: Color.Transparent,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: BackgroundColorPropertyChanged);

        private static void BackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.BackgroundColor = (Color)newValue;
        }

        public Color BackgroundColorProp
        {
            get => (Color)GetValue(CustomBackgroundColorProperty);
            set => SetValue(CustomBackgroundColorProperty, value);
        }

        public static readonly BindableProperty ToolbarHeightProperty =
            BindableProperty.Create(
                nameof(ToolbarHeightProp),
                typeof(double),
                typeof(CustomToolbar),
                defaultValue: 50.0,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: ToolbarHeightPropertyChanged);

        private static void ToolbarHeightPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.HeightRequest = (double)newValue;
        }

        public double ToolbarHeightProp
        {
            get => (double)GetValue(ToolbarHeightProperty);
            set => SetValue(ToolbarHeightProperty, value);
        }
        #endregion

        #region Label 
        public static readonly BindableProperty TitleTextProperty =
            BindableProperty.Create(
                nameof(TitleText),
                typeof(string),
                typeof(CustomToolbar),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: TitleTextPropertyChanged);

        private static void TitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.ToolbarLabel.Text = newValue?.ToString();
        }

        public string TitleText
        {
            get => GetValue(TitleTextProperty)?.ToString();
            set => SetValue(TitleTextProperty, value);
        }

        public static readonly BindableProperty CustomTextColorProperty =
            BindableProperty.Create(
                nameof(CustomTextColorProp),
                typeof(Color),
                typeof(CustomToolbar),
                defaultValue: Color.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: CustomTextColorPropertyChanged);

        private static void CustomTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.ToolbarLabel.TextColor = (Color)newValue;
        }

        public Color CustomTextColorProp
        {
            get => (Color)GetValue(CustomTextColorProperty);
            set => SetValue(CustomTextColorProperty, value);
        }

        public static readonly BindableProperty LabelSizeProperty =
            BindableProperty.Create(
                nameof(LabelSizeProp),
                typeof(double),
                typeof(CustomToolbar),
                defaultBindingMode: BindingMode.OneWay,
                defaultValue: DefaultLabelTextSize,
                propertyChanged: LabelSizePropertyChanged);

        private static void LabelSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.ToolbarLabel.FontSize = (double)newValue;
        }

        public double LabelSizeProp
        {
            get => (double)GetValue(LabelSizeProperty);
            set => SetValue(LabelSizeProperty, value);
        }
        #endregion

        #region Left button
        public static readonly BindableProperty LeftButtonIsVisibleProperty = 
            BindableProperty.Create(nameof(LeftButtonIsVisibleProp),
                typeof(bool),
                typeof(CustomToolbar),
                defaultValue: true,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonIsVisiblePropertyChanged);

        private static void LeftButtonIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.IsVisible = (bool)newValue;
        }

        public bool LeftButtonIsVisibleProp
        {
            get => (bool)GetValue(LeftButtonIsVisibleProperty);
            set => SetValue(LeftButtonIsVisibleProperty, value);
        }

        public static readonly BindableProperty LeftButtonTitleTextProperty =
            BindableProperty.Create(
                nameof(LeftButtonTitleTextProp),
                typeof(string),
                typeof(CustomToolbar),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonTitleTextPropertyChanged);

        private static void LeftButtonTitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.Text = newValue?.ToString();
        }

        public string LeftButtonTitleTextProp
        {
            get => GetValue(LeftButtonTitleTextProperty)?.ToString();
            set => SetValue(LeftButtonTitleTextProperty, value);
        }

        public static readonly BindableProperty LeftButtonTextColorProperty =
            BindableProperty.Create(
                nameof(LeftButtonTextColorProp),
                typeof(Color),
                typeof(CustomToolbar),
                defaultValue: Color.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonTextColorPropertyChanged);

        private static void LeftButtonTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.TextColor = (Color)newValue;
        }

        public Color LeftButtonTextColorProp
        {
            get => (Color)GetValue(CustomTextColorProperty);
            set => SetValue(CustomTextColorProperty, value);
        }

        public static readonly BindableProperty LeftButtonBorderColorProperty =
            BindableProperty.Create(
                nameof(LeftButtonBorderColorProp),
                typeof(Color),
                typeof(CustomToolbar),
                defaultValue: DefaultBorderColor,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonBorderColorPropertyChanged);

        private static void LeftButtonBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.BorderColor = (Color)newValue;
        }

        public Color LeftButtonBorderColorProp
        {
            get => (Color)GetValue(LeftButtonBorderColorProperty);
            set => SetValue(LeftButtonBorderColorProperty, value);
        }

        public static readonly BindableProperty LeftButtonBorderWidthProperty =
            BindableProperty.Create(
                nameof(LeftButtonBorderWidthProp),
                typeof(double),
                typeof(CustomToolbar),
                defaultValue: DefaultBorderWidth,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonBorderWidthPropertyChanged);

        private static void LeftButtonBorderWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.BorderWidth = (double)newValue;
        }

        public double LeftButtonBorderWidthProp
        {
            get => (double)GetValue(LeftButtonBorderWidthProperty);
            set => SetValue(LeftButtonBorderWidthProperty, value);
        }

        public static readonly BindableProperty LeftButtonCornerRadiusProperty =
            BindableProperty.Create(
                nameof(LeftButtonCornerRadiusProp),
                typeof(int),
                typeof(CustomToolbar),
                defaultValue: DefaultCornerRadius,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonCornerRadiusPropertyChanged);

        private static void LeftButtonCornerRadiusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.CornerRadius = (int)newValue;
        }

        public int LeftButtonCornerRadiusProp
        {
            get => (int)GetValue(LeftButtonCornerRadiusProperty);
            set => SetValue(LeftButtonCornerRadiusProperty, value);
        }

        public static readonly BindableProperty LeftButtonCommandProperty =
           BindableProperty.Create(
               nameof(LeftButtonCommand),
               typeof(ICommand),
               typeof(CustomToolbar),
               null);

        public static readonly BindableProperty LeftButtonCommandParameterProperty =
           BindableProperty.Create(
               nameof(LeftButtonCommandParameter),
               typeof(object),
               typeof(CustomToolbar),
               null);

        public ICommand LeftButtonCommand
        {
            get { return (ICommand)GetValue(LeftButtonCommandProperty); }
            set { SetValue(LeftButtonCommandProperty, value); }
        }
        public object LeftButtonCommandParameter
        {
            get { return GetValue(LeftButtonCommandParameterProperty); }
            set { SetValue(LeftButtonCommandParameterProperty, value); }
        }

        public ICommand LeftButtonCheckCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (LeftButtonCommand == null)
                        return;
                    if (LeftButtonCommand.CanExecute(LeftButtonCommandParameter))
                        LeftButtonCommand.Execute(LeftButtonCommandParameter);
                });
            }
        }

        public static readonly BindableProperty LeftButtonTextSizeProperty =
            BindableProperty.Create(
                nameof(LeftButtonTextSizeProp),
                typeof(double),
                typeof(CustomToolbar),
                defaultValue: DefaultLabelTextSize,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonTextSizePropertyChanged);

        private static void LeftButtonTextSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.FontSize = (double)newValue;
        }

        public double LeftButtonTextSizeProp
        {
            get => (double)GetValue(LeftButtonTextSizeProperty);
            set => SetValue(LeftButtonTextSizeProperty, value);
        }
        #endregion

        #region Right button
        public static readonly BindableProperty RightButtonIsVisibleProperty =
            BindableProperty.Create(
                nameof(RightButtonIsVisibleProp),
                typeof(bool),
                typeof(CustomToolbar),
                defaultValue: true,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightButtonIsVisiblePropertyChanged);

        private static void RightButtonIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.RightButton.IsVisible = (bool)newValue;
        }

        public bool RightButtonIsVisibleProp
        {
            get => (bool)GetValue(RightButtonIsVisibleProperty);
            set => SetValue(RightButtonIsVisibleProperty, value);
        }

        public static readonly BindableProperty RightButtonTitleTextProperty =
            BindableProperty.Create(
                nameof(RightButtonTitleTextProp),
                typeof(string),
                typeof(CustomToolbar),
                defaultValue: string.Empty,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightButtonTitleTextPropertyChanged);

        private static void RightButtonTitleTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.RightButton.Text = newValue?.ToString();
        }

        public string RightButtonTitleTextProp
        {
            get => GetValue(RightButtonTitleTextProperty)?.ToString();
            set => SetValue(RightButtonTitleTextProperty, value);
        }

        public static readonly BindableProperty RightButtonTextColorProperty =
            BindableProperty.Create(
                nameof(RightButtonTextColorProp),
                typeof(Color),
                typeof(CustomToolbar),
                defaultValue: Color.White,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightButtonTextColorPropertyChanged);

        private static void RightButtonTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.RightButton.TextColor = (Color)newValue;
        }

        public Color RightButtonTextColorProp
        {
            get => (Color)GetValue(RightButtonTextColorProperty);
            set => SetValue(RightButtonTextColorProperty, value);
        }

        public static readonly BindableProperty RightButtonBorderColorProperty =
            BindableProperty.Create(
                nameof(RightButtonBorderColorProp),
                typeof(Color),
                typeof(CustomToolbar),
                defaultValue: DefaultBorderColor,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightButtonBorderColorPropertyChanged);

        private static void RightButtonBorderColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.RightButton.BorderColor = (Color)newValue;
        }

        public Color RightButtonBorderColorProp
        {
            get => (Color)GetValue(RightButtonBorderColorProperty);
            set => SetValue(RightButtonBorderColorProperty, value);
        }

        public static readonly BindableProperty RightButtonBorderWidthProperty =
            BindableProperty.Create(
                nameof(RightButtonBorderWidthProp),
                typeof(double),
                typeof(CustomToolbar),
                defaultValue: DefaultBorderWidth,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightButtonBorderWidthPropertyChanged);

        private static void RightButtonBorderWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.RightButton.BorderWidth = (double)newValue;
        }

        public double RightButtonBorderWidthProp
        {
            get => (double)GetValue(RightButtonBorderWidthProperty);
            set => SetValue(RightButtonBorderWidthProperty, value);
        }

        public static readonly BindableProperty RightButtonCornerRadiusProperty =
            BindableProperty.Create(
                nameof(RightButtonCornerRadiusProp),
                typeof(int),
                typeof(CustomToolbar),
                defaultValue: DefaultCornerRadius,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: RightButtonCornerRadiusPropertyChanged);

        private static void RightButtonCornerRadiusPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.RightButton.CornerRadius = (int)newValue;
        }

        public int RightButtonCornerRadiusProp
        {
            get => (int)GetValue(RightButtonCornerRadiusProperty);
            set => SetValue(RightButtonCornerRadiusProperty, value);
        }

        public static readonly BindableProperty RightButtonCommandProperty =
           BindableProperty.Create(
               nameof(RightButtonCommand),
               typeof(ICommand),
               typeof(CustomToolbar),
               null);

        public static readonly BindableProperty RightButtonCommandParameterProperty =
           BindableProperty.Create(
               nameof(RightButtonCommandParameter),
               typeof(object),
               typeof(CustomToolbar),
               null);

        public ICommand RightButtonCommand
        {
            get { return (ICommand)GetValue(RightButtonCommandProperty); }
            set { SetValue(RightButtonCommandProperty, value); }
        }
        public object RightButtonCommandParameter
        {
            get { return GetValue(RightButtonCommandParameterProperty); }
            set { SetValue(RightButtonCommandParameterProperty, value); }
        }

        public ICommand RightButtonCheckCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (RightButtonCommand == null)
                        return;
                    if (RightButtonCommand.CanExecute(RightButtonCommandParameter))
                        RightButtonCommand.Execute(RightButtonCommandParameter);
                });
            }
        }

        public static readonly BindableProperty RightButtonTextSizeProperty =
            BindableProperty.Create(
                nameof(RightButtonTextSizeProp),
                typeof(double),
                typeof(CustomToolbar),
                defaultBindingMode: BindingMode.OneWay,
                defaultValue: DefaultLabelTextSize,
                propertyChanged: RightButtonTextSizePropertyChanged);

        private static void RightButtonTextSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.RightButton.FontSize = (double)newValue;
        }

        public double RightButtonTextSizeProp
        {
            get => (double)GetValue(RightButtonTextSizeProperty);
            set => SetValue(RightButtonTextSizeProperty, value);
        }
        #endregion
    }
}
using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BrainWaves.CustomControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomToolbar : ContentView
    {
        public CustomToolbar()
        {
            InitializeComponent();
            /*
            RightButton.SetBinding(Image.SourceProperty, new Binding(nameof(RightButtonImageSourceProp), source: this));
            
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                RightButtonICommandProp = new Command(() =>
                {
                    RightButtonClickedEvent?.Invoke(this, EventArgs.Empty);

                    if(RightButtonICommandProp != null)
                    {
                        if (RightButtonICommandProp.CanExecute(RightButtonCommandParameter))
                            RightButtonICommandProp.Execute(RightButtonCommandParameter);
                    }
                })
            });
            */
        }

        #region Stack Parent
        public static readonly BindableProperty CustomBackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColorProp),
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

        public static readonly BindableProperty CustomTextColorProperty = BindableProperty.Create(nameof(CustomTextColorProp),
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
        #endregion

        #region Buttons
        public static void Execute(ICommand command)
        {
            if (command == null) return;
            if (command.CanExecute(null))
            {
                command.Execute(null);
            }
        }


        #region Left button
        public static readonly BindableProperty LeftButtonIsVisibleProperty = 
            BindableProperty.Create(nameof(LeftButtonIsVisible),
                typeof(bool),
                typeof(CustomToolbar),
                defaultValue: false,
                defaultBindingMode: BindingMode.OneWay,
                propertyChanged: LeftButtonIsVisiblePropertyChanged);

        private static void LeftButtonIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.FrameLeftButton.IsVisible = (bool)newValue;
        }

        public bool LeftButtonIsVisible
        {
            get => (bool)GetValue(LeftButtonIsVisibleProperty);
            set => SetValue(LeftButtonIsVisibleProperty, value);
        }

        public static readonly BindableProperty LeftButtonImageSourceProperty =
            BindableProperty.Create(nameof(LeftButtonImageSourceProp),
                typeof(string),
                typeof(CustomToolbar),
                propertyChanged: LeftButtonImageSourcePropertyChanged,
                defaultValue: null);

        private static void LeftButtonImageSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.ImageSource = (string)newValue;
        }

        public string LeftButtonImageSourceProp
        {
            get => GetValue(LeftButtonIsVisibleProperty).ToString();
            set => SetValue(LeftButtonIsVisibleProperty, value);
        }

        public static readonly BindableProperty LeftButtonICommandProperty =
            BindableProperty.Create(
                nameof(LeftButtonICommandProp),
                typeof(ICommand),
                typeof(CustomToolbar),
                propertyChanged: LeftButtonICommandPropertyChanged);

        private static void LeftButtonICommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.Command = (ICommand)newValue;
            control.LeftButtonImage.Command = (ICommand)newValue;
        }

        public ICommand LeftButtonICommandProp
        {
            get => (ICommand)GetValue(LeftButtonICommandProperty);
            set => SetValue(LeftButtonICommandProperty, value);
        }

        public static readonly BindableProperty LeftButtonTitleTextProperty =
            BindableProperty.Create(
                nameof(LeftButtonTitleText),
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

        public string LeftButtonTitleText
        {
            get => GetValue(TitleTextProperty)?.ToString();
            set => SetValue(TitleTextProperty, value);
        }

        #endregion

        #region Right button
        public static readonly BindableProperty RightButtonIsVisibleProperty = BindableProperty.Create(nameof(RightButtonIsVisible),
            typeof(bool),
            typeof(CustomToolbar),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay,
            propertyChanged: RightButtonIsVisiblePropertyChanged);

        private static void RightButtonIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.FrameRightButton.IsVisible = (bool)newValue;
        }

        public bool RightButtonIsVisible
        {
            get => (bool)GetValue(RightButtonIsVisibleProperty);
            set => SetValue(RightButtonIsVisibleProperty, value);
        }

        public static readonly BindableProperty RightButtonImageSourceProperty =
            BindableProperty.Create(
                nameof(RightButtonImageSourceProp),
                typeof(ImageSource),
                typeof(CustomToolbar),
                default(ImageSource),
                propertyChanged: RightButtonImageSourcePropertyChanged);

        private static void RightButtonImageSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CustomToolbar)bindable;
            control.LeftButton.ImageSource = (ImageSource)newValue;
        }

        public ImageSource RightButtonImageSourceProp
        {
            get => (ImageSource)GetValue(LeftButtonIsVisibleProperty);
            set => SetValue(LeftButtonIsVisibleProperty, value);
        }

        public static readonly BindableProperty RightButtonICommandProperty =
            BindableProperty.Create(
                nameof(RightButtonICommandProp),
                typeof(ICommand),
                typeof(CustomToolbar),
                defaultValue: null);

        public ICommand RightButtonICommandProp
        {
            get => (ICommand)GetValue(RightButtonICommandProperty);
            set => SetValue(RightButtonICommandProperty, value);
        }

        public static readonly BindableProperty RightByttonCommandParameterProperty =
            BindableProperty.Create(
                nameof(RightButtonCommandParameter),
                typeof(object),
                typeof(CustomToolbar));

        public object RightButtonCommandParameter
        {
            get { return GetValue(RightByttonCommandParameterProperty); }
            set { SetValue(RightByttonCommandParameterProperty, value); }
        }

        public event EventHandler RightButtonClickedEvent;


        public static readonly BindableProperty RightButtonTitleTextProperty =
            BindableProperty.Create(
                nameof(RightButtonTitleText),
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

        public string RightButtonTitleText
        {
            get => GetValue(TitleTextProperty)?.ToString();
            set => SetValue(TitleTextProperty, value);
        }

        #endregion
        #endregion


        public static readonly BindableProperty CommandProperty =
           BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(CustomToolbar), null);
        public static readonly BindableProperty CommandParameterProperty =
           BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(CustomToolbar), null);
        public event EventHandler CheckedChanged;

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand CheckCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (Command == null)
                        return;
                    if (Command.CanExecute(CommandParameter))
                        Command.Execute(CommandParameter);
                });
            }
        }
    }
}
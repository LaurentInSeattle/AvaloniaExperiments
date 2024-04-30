namespace Lyt.Avalonia.Controls; 

public partial class SvgIcon : UserControl
{
    // TODO 
    // private const string DefaultSvgIcon = "DefaultSvgIcon";

    private DrawingImage? drawingImage;

    public SvgIcon() => this.InitializeComponent();

    public void UpdateImage()
    {
        if (this.drawingImage is null )
        {
            return;
        }

        if ( this.drawingImage.Drawing is DrawingGroup drawingGroup)
        {
            this.ProcessDrawingGroup(drawingGroup);
            this.image.Source = this.drawingImage;
        }
    }

    private void ProcessDrawingGroup(DrawingGroup drawingGroup)
    {
        if (drawingGroup.Children != null)
        {
            foreach (var child in drawingGroup.Children)
            {
                if (child is DrawingGroup childDrawingGroup)
                {
                    this.ProcessDrawingGroup(childDrawingGroup);
                }

                if (child is GeometryDrawing geometryDrawing)
                {
                    this.ProcessGeometryDrawing(geometryDrawing);
                }
            }
        }
    }

    private void ProcessGeometryDrawing(GeometryDrawing geometryDrawing)
    {
        if (geometryDrawing.Brush != null)
        {
            // Brush not null: we need to fill 
            geometryDrawing.Brush = this.Foreground;
        }

        if (geometryDrawing.Pen is Pen pen)
        {
            // If the pen is null, no stroke, no need to do anything 
            if (pen.Brush is SolidColorBrush)
            {
                pen.Brush = this.Foreground;
            }
            else if (pen.Brush is LinearGradientBrush lnearGradientBrush)
            {
                foreach (var stop in lnearGradientBrush.GradientStops)
                {
                    stop.Color = this.Foreground.Color;
                }
            }
            else
            {
                if (pen.Brush is not null)
                {
                    Debug.WriteLine("Unsupported pen brush: " + pen.Brush.GetType().Name);
                } 
                else
                {
                    Debug.WriteLine("No brush ??? ");
                }

                if ( Debugger.IsAttached ) {  Debugger.Break(); }
            }

            pen.Thickness = this.StrokeThickness;
        }
        else
        {
            geometryDrawing.Pen = new Pen() { Thickness = this.StrokeThickness, Brush = this.Foreground };
        }
    }

    #region Styled Properties

    #region Styled Property Source

    /// <summary> Source Styled Property </summary>
    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register< SvgIcon , string>(
            nameof(Source), 
            defaultValue:string.Empty, 
            inherits: false, 
            defaultBindingMode:BindingMode.OneWay, 
            validate: null, 
            coerce: CoerceSource, 
            enableDataValidation: false);

    /// <summary> Gets or sets the Source property.</summary>
    public string Source
    {
        get => this.GetValue(SourceProperty);
        set => this.SetValue(SourceProperty, value);
    }

    private static string CoerceSource(AvaloniaObject sender, string newSource)
    {
        bool valid = !string.IsNullOrWhiteSpace(newSource) ;
        if ( valid && (sender is SvgIcon svgIcon))
        {
            string source = string.Concat("icon_", newSource, "DrawingImage");
            if (Utilities.TryFindResource<DrawingImage>(source, out svgIcon.drawingImage) ||
                Utilities.TryFindResource<DrawingImage>(newSource, out svgIcon.drawingImage))
            {
                svgIcon.UpdateImage();
                return newSource;
            }
        }

        return string.Empty;
    }

    #endregion Styled Property Source

    #region Styled Property Wadding

    /// <summary> Wadding Styled Property </summary>
    public static readonly StyledProperty<Thickness> WaddingProperty =
        AvaloniaProperty.Register<SvgIcon, Thickness>(
            nameof(Wadding),
            defaultValue: new Thickness(0),
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceWadding,
            enableDataValidation: false);

    /// <summary> Gets or sets the Wadding property.</summary>
    public Thickness Wadding
    {
        get => (Thickness)this.GetValue(WaddingProperty);
        set => this.SetValue(WaddingProperty, value);
    }

    private static Thickness CoerceWadding(AvaloniaObject sender, Thickness newWadding)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.viewBox.Margin = newWadding;
        }

        return newWadding;
    }

    #endregion Styled  Property Wadding

    #region Styled  Property Height

    /// <summary> Height Styled Property </summary>
    public static readonly new StyledProperty<double> HeightProperty =
        AvaloniaProperty.Register<SvgIcon, double>(
            nameof(Height),
            defaultValue: 32.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceHeight,
            enableDataValidation: false);


    /// <summary> Gets or sets the Height property.</summary>
    public new double Height
    {
        get => this.GetValue(HeightProperty);
        set => this.SetValue(HeightProperty, value);
    }

    /// <summary> Coerces the Height value. </summary>
    private static double CoerceHeight(AvaloniaObject sender, double newHeight)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.grid.Height = newHeight;
        }

        return newHeight;
    }

    #endregion Styled Property Height

    #region Styled  Property Width

    /// <summary> Width Styled Property </summary>
    public static readonly new StyledProperty<double> WidthProperty =
        AvaloniaProperty.Register<SvgIcon, double>(
            nameof(Width),
            defaultValue: 32.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceWidth,
            enableDataValidation: false);


    /// <summary> Gets or sets the Width property.</summary>
    public new double Width
    {
        get => this.GetValue(WidthProperty);
        set => this.SetValue(WidthProperty, value);
    }

    /// <summary> Coerces the Width value. </summary>
    private static double CoerceWidth(AvaloniaObject sender, double newWidth)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.grid.Width = newWidth;
        }

        return newWidth;
    }

    #endregion Styled Property Width

    #region Styled Property Background

    /// <summary> Background Styled Property </summary>
    public static new readonly StyledProperty<Brush> BackgroundProperty =
        AvaloniaProperty.Register<SvgIcon, Brush>(
            nameof(Background),
            defaultValue: new SolidColorBrush(Colors.Aquamarine, 1.0),
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceBackground,
            enableDataValidation: false);

    /// <summary> Gets or sets the Background property.</summary>
    public new Brush Background
    {
        get => this.GetValue(BackgroundProperty);
        set => this.SetValue(BackgroundProperty, value);
    }

    /// <summary> Coerces the Background value. </summary>
    private static Brush CoerceBackground(AvaloniaObject sender, Brush newBackground)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.grid.Background = newBackground;
        }

        return newBackground;
    }

    #endregion Styled Property Background

    #region Styled Property Foreground

    /// <summary> Foreground Styled Property </summary>
    public static new readonly StyledProperty<SolidColorBrush> ForegroundProperty =
        AvaloniaProperty.Register<SvgIcon, SolidColorBrush>(
            nameof(Foreground),
            defaultValue: new SolidColorBrush(Colors.Aquamarine, 1.0),
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceForeground,
            enableDataValidation: false);

    /// <summary> Gets or sets the Foreground property.</summary>
    public new SolidColorBrush Foreground
    {
        get => this.GetValue(ForegroundProperty);
        set => this.SetValue(ForegroundProperty, value);
    }

    /// <summary> Coerces the Foreground value. </summary>
    private static SolidColorBrush CoerceForeground(AvaloniaObject sender, SolidColorBrush newForeground)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.Foreground = newForeground;
            svgIcon.UpdateImage();
        }

        return newForeground;
    }

    #endregion Styled Property Foreground

    #region Styled Property StrokeThickness

    /// <summary> StrokeThickness Styled Property </summary>
    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<SvgIcon, double>(
            nameof(StrokeThickness),
            defaultValue: 2.0,
            inherits: false,
            defaultBindingMode: BindingMode.OneWay,
            validate: null,
            coerce: CoerceStrokeThickness,
            enableDataValidation: false);

    /// <summary> Gets or sets the StrokeThickness property.</summary>
    public double StrokeThickness
    {
        get => this.GetValue(StrokeThicknessProperty);
        set => this.SetValue(StrokeThicknessProperty, value);
    }

    /// <summary> Coerces the StrokeThickness value. </summary>
    private static double CoerceStrokeThickness(AvaloniaObject sender , double newStrokeThickness)
    {
        if (sender is SvgIcon svgIcon)
        {
            svgIcon.StrokeThickness = newStrokeThickness;
            svgIcon.UpdateImage();
        } 

        return newStrokeThickness;
    }

    #endregion Styled Property StrokeThickness

    #endregion Styled Properties
}

using Avalonia.Controls.Primitives;

namespace Lyt.Avalonia.Controls.BadgeControl;

public partial class Badge : UserControl
{
    ContentControl? contentControl; 
    
    public Badge() 
    {
        this.InitializeComponent();
        this.ApplyTemplate();
        this.Loaded += this.OnLoaded; ;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (this.contentControl is not null)
        {
            this.contentControl.Content = this.Content;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.contentControl = e.NameScope.Find<ContentControl>("contentControl");
    }
}

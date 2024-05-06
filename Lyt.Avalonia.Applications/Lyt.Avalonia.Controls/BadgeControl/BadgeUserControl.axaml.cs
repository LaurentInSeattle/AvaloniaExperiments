namespace Lyt.Avalonia.Controls.BadgeControl;

public partial class BadgeUserControl : UserControl
{
    private ContentControl? contentControl; 
    
    public BadgeUserControl() 
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

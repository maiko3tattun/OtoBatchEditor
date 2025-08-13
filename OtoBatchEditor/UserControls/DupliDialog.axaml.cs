using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using DialogHostAvalonia;
using OtoBatchEditor.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OtoBatchEditor;

public partial class DupliDialog : UserControl
{
    public bool Cancel { get; set; } = true;

    public DupliDialog()
    {
        InitializeComponent();
    }

    public DupliDialog(List<DuplicateItemGroup> groups)
    {
        InitializeComponent();
        DataContext = new DupliDialogViewModel(new ObservableCollection<DuplicateItemGroup>(groups));
    }

    private void OKClick(object? sender, RoutedEventArgs e)
    {
        Cancel = false;
        DialogHost.GetDialogSession("dialog")?.Close(false);
    }

    private void CancelClick(object? sender, RoutedEventArgs e)
    {
        DialogHost.GetDialogSession("dialog")?.Close(false);
    }

    private void PanelPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is StackPanel panel && panel.DataContext is DuplicateItem item)
        {
            if (ToolTip.GetTip(panel) != null)
            {
                return;
            }
            var textBlock = new TextBlock
            {
                Text = $"{item.DirectorName}: {item.Oto}",
                TextAlignment = Avalonia.Media.TextAlignment.Center
            };

            textBlock.Measure(Size.Infinity);
            double desiredWidth = textBlock.DesiredSize.Width * 1.2;
            textBlock.Width = desiredWidth;

            var tooltip = new ToolTip
            {
                Content = textBlock,
                Width = desiredWidth + 20
            };
            ToolTip.SetTip(panel, tooltip);
        }
    }
}

public class DupliDialogViewModel : ViewModelBase
{
    public ObservableCollection<DuplicateItemGroup> Groups { get; set; }

    public DupliDialogViewModel(ObservableCollection<DuplicateItemGroup> groups)
    {
        Groups = groups;
    }
}
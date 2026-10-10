using System.Windows;
using System.Windows.Controls;
using Negocio;

namespace Presentacion.Vistas;

public partial class ResumenView : UserControl
{
    private readonly ProductoNegocio _negocio = new();

    public ResumenView()
    {
        InitializeComponent();
        IsVisibleChanged += (_, e) => { if ((bool)e.NewValue) Cargar(); };
    }

    private void Cargar()
    {
        try { grid.ItemsSource = _negocio.ResumenPorCategoria(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning); }
    }

    private void Actualizar_Click(object s, RoutedEventArgs e) => Cargar();
}

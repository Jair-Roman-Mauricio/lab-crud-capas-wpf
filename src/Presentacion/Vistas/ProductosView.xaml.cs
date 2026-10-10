using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Entidades;
using Negocio;

namespace Presentacion.Vistas;

public partial class ProductosView : UserControl
{
    private readonly ProductoNegocio _negocio = new();
    private int _id;
    private bool _papelera;

    public ProductosView()
    {
        InitializeComponent();
        Loaded += (_, _) => Cargar();
    }

    private void Cargar()
    {
        try { grid.ItemsSource = _papelera ? _negocio.ListarEliminados(txtBuscar.Text) : _negocio.Buscar(txtBuscar.Text); }
        catch (Exception ex) { Error(ex); }
    }

    private void Buscar_Changed(object s, TextChangedEventArgs e) { if (IsLoaded) Cargar(); }

    private void Grid_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
        if (grid.SelectedItem is not Producto p) return;
        _id = p.IdProducto;
        txtNombre.Text = p.Nombre; txtDescripcion.Text = p.Descripcion;
        txtPrecio.Text = p.Precio.ToString(CultureInfo.CurrentCulture); txtStock.Text = p.Stock.ToString();
    }

    private void Guardar_Click(object s, RoutedEventArgs e)
    {
        try
        {
            if (!decimal.TryParse(txtPrecio.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var precio))
                throw new ArgumentException("El precio no es un número válido.");
            if (!int.TryParse(txtStock.Text, out var stock))
                throw new ArgumentException("El stock debe ser un número entero.");
            _negocio.Guardar(new Producto
            {
                IdProducto = _id, Nombre = txtNombre.Text, Descripcion = txtDescripcion.Text, Precio = precio, Stock = stock
            });
            Limpiar(); Cargar();
        }
        catch (Exception ex) { Error(ex); }
    }

    private void Eliminar_Click(object s, RoutedEventArgs e)
    {
        if (_id == 0) { MessageBox.Show("Seleccione un producto."); return; }
        if (MessageBox.Show("¿Eliminar el producto seleccionado?", "Confirmar", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        try { _negocio.Eliminar(_id); Limpiar(); Cargar(); }
        catch (Exception ex) { Error(ex); }
    }

    private void Papelera_Click(object s, RoutedEventArgs e)
    {
        _papelera = btnPapelera.IsChecked == true;
        btnPapelera.Content = _papelera ? "Ver activos" : "Ver eliminados";
        lblModo.Text = _papelera ? "Registros eliminados" : "";
        colFecha.Visibility = _papelera ? Visibility.Visible : Visibility.Collapsed;
        var normal = _papelera ? Visibility.Collapsed : Visibility.Visible;
        btnGuardar.Visibility = btnNuevo.Visibility = btnEliminar.Visibility = normal;
        btnRestaurar.Visibility = _papelera ? Visibility.Visible : Visibility.Collapsed;
        Limpiar(); Cargar();
    }

    private void Restaurar_Click(object s, RoutedEventArgs e)
    {
        if (_id == 0) { MessageBox.Show("Seleccione un producto eliminado."); return; }
        try { _negocio.Restaurar(_id); Limpiar(); Cargar(); }
        catch (Exception ex) { Error(ex); }
    }

    private void Nuevo_Click(object s, RoutedEventArgs e) => Limpiar();

    private void Limpiar()
    {
        _id = 0; grid.SelectedItem = null;
        txtNombre.Clear(); txtDescripcion.Clear(); txtPrecio.Clear(); txtStock.Clear();
    }

    private static void Error(Exception ex) => MessageBox.Show(ex.Message, "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
}

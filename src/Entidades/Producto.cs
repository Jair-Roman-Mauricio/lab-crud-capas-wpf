namespace Entidades;

public class Producto
{
    public int IdProducto { get; set; }
    public string Nombre { get; set; } = "";
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool Estado { get; set; } = true;
    public DateTime? FechaEliminacion { get; set; }
}

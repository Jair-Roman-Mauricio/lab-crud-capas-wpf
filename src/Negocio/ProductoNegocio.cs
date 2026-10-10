using Datos;
using Entidades;

namespace Negocio;

public class ProductoNegocio
{
    private readonly ProductoDatos _datos = new();

    public List<Producto> Listar() => _datos.Listar();

    public List<Producto> Buscar(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? _datos.Listar() : _datos.Buscar(texto.Trim());

    public List<Producto> ListarEliminados(string? texto) =>
        _datos.ListarEliminados(string.IsNullOrWhiteSpace(texto) ? null : texto.Trim());

    public List<ResumenCategoria> ResumenPorCategoria() => _datos.ResumenPorCategoria();

    public void Restaurar(int id)
    {
        if (!_datos.Restaurar(id)) throw new InvalidOperationException("El producto no existe o no está eliminado.");
    }

    public int Guardar(Producto p)
    {
        p.Nombre = p.Nombre.Trim();
        if (p.Nombre.Length == 0) throw new ArgumentException("El nombre es obligatorio.");
        if (p.Precio < 0) throw new ArgumentException("El precio no puede ser negativo.");
        if (p.Stock < 0) throw new ArgumentException("El stock no puede ser negativo.");
        if (p.IdCategoria <= 0) throw new ArgumentException("Seleccione una categoría.");
        if (p.IdProducto == 0) return _datos.Insertar(p);
        if (!_datos.Actualizar(p)) throw new InvalidOperationException("El producto no existe o fue eliminado.");
        return p.IdProducto;
    }

    public void Eliminar(int id)
    {
        if (!_datos.Eliminar(id)) throw new InvalidOperationException("El producto no existe o ya fue eliminado.");
    }
}

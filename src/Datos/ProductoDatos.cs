using Entidades;
using Microsoft.Data.SqlClient;

namespace Datos;

public class ProductoDatos
{
    public List<Producto> Listar() => Leer("dbo.sp_Producto_Listar", null);

    public List<Producto> Buscar(string texto) => Leer("dbo.sp_Producto_Buscar", c => c.Parameters.AddWithValue("@Texto", texto));

    public List<Producto> ListarEliminados(string? texto) => Leer("dbo.sp_Producto_ListarEliminados",
        c => c.Parameters.AddWithValue("@Texto", (object?)texto ?? DBNull.Value));

    public bool Restaurar(int id) =>
        Escalar("dbo.sp_Producto_Restaurar", c => c.Parameters.AddWithValue("@IdProducto", id)) > 0;

    public int Insertar(Producto p) => Escalar("dbo.sp_Producto_Insertar", c => Llenar(c, p));

    public bool Actualizar(Producto p) => Escalar("dbo.sp_Producto_Actualizar", c =>
    {
        c.Parameters.AddWithValue("@IdProducto", p.IdProducto);
        Llenar(c, p);
    }) > 0;

    public bool Eliminar(int id) =>
        Escalar("dbo.sp_Producto_Eliminar", c => c.Parameters.AddWithValue("@IdProducto", id)) > 0;

    public List<ResumenCategoria> ResumenPorCategoria()
    {
        var lista = new List<ResumenCategoria>();
        using var cn = Conexion.Crear();
        using var cmd = Conexion.Sp(cn, "dbo.sp_Producto_ResumenPorCategoria");
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
            lista.Add(new ResumenCategoria
            {
                Categoria = dr.GetString(0),
                CantidadProductos = dr.GetInt32(1),
                StockTotal = dr.GetInt32(2),
                ValorInventario = dr.GetDecimal(3)
            });
        return lista;
    }

    private static void Llenar(SqlCommand c, Producto p)
    {
        c.Parameters.AddWithValue("@Nombre", p.Nombre);
        c.Parameters.AddWithValue("@Descripcion", (object?)p.Descripcion ?? DBNull.Value);
        c.Parameters.AddWithValue("@Precio", p.Precio);
        c.Parameters.AddWithValue("@Stock", p.Stock);
        c.Parameters.AddWithValue("@IdCategoria", p.IdCategoria);
    }

    private static int Escalar(string sp, Action<SqlCommand> parametros)
    {
        using var cn = Conexion.Crear();
        using var cmd = Conexion.Sp(cn, sp);
        parametros(cmd);
        cn.Open();
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static List<Producto> Leer(string sp, Action<SqlCommand>? parametros)
    {
        var lista = new List<Producto>();
        using var cn = Conexion.Crear();
        using var cmd = Conexion.Sp(cn, sp);
        parametros?.Invoke(cmd);
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
            lista.Add(new Producto
            {
                IdProducto = dr.GetInt32(0),
                Nombre = dr.GetString(1),
                Descripcion = dr.IsDBNull(2) ? null : dr.GetString(2),
                Precio = dr.GetDecimal(3),
                Stock = dr.GetInt32(4),
                Estado = dr.GetBoolean(5),
                IdCategoria = dr.GetInt32(6),
                Categoria = dr.GetString(7),
                FechaEliminacion = dr.IsDBNull(8) ? null : dr.GetDateTime(8)
            });
        return lista;
    }
}

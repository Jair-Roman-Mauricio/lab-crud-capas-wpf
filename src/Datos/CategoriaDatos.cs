using Entidades;

namespace Datos;

public class CategoriaDatos
{
    public List<Categoria> Listar()
    {
        var lista = new List<Categoria>();
        using var cn = Conexion.Crear();
        using var cmd = Conexion.Sp(cn, "dbo.sp_Categoria_Listar");
        cn.Open();
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
            lista.Add(new Categoria { IdCategoria = dr.GetInt32(0), Nombre = dr.GetString(1) });
        return lista;
    }
}

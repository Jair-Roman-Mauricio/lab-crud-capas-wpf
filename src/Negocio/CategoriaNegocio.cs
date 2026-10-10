using Datos;
using Entidades;

namespace Negocio;

public class CategoriaNegocio
{
    private readonly CategoriaDatos _datos = new();

    public List<Categoria> Listar() => _datos.Listar();
}

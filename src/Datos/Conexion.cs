using Microsoft.Data.SqlClient;

namespace Datos;

public static class Conexion
{
    public static string Cadena { get; set; } =
        Environment.GetEnvironmentVariable("LABCRUD_CONN")
        ?? "Server=localhost;Database=LabCrud;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection Crear() => new(Cadena);

    // Crea un comando de procedimiento almacenado.
    public static SqlCommand Sp(SqlConnection cn, string nombre) =>
        new(nombre, cn) { CommandType = System.Data.CommandType.StoredProcedure };
}

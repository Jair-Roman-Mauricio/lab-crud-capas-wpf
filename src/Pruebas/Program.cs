using Entidades;
using Negocio;

// Prueba de integración de Negocio -> Datos -> SQL Server (procedimientos almacenados).
void Ok(bool c, string m) { Console.WriteLine((c ? "OK   " : "FALLO ") + m); if (!c) Environment.ExitCode = 1; }

var prn = new ProductoNegocio();
var pid = prn.Guardar(new Producto { Nombre = "Laptop", Descripcion = "14 pulgadas", Precio = 2500.50m, Stock = 5 });
Ok(pid > 0, "Producto: insertar");
Ok(prn.Buscar("lap").Any(x => x.IdProducto == pid), "Producto: buscar");
var q = prn.Listar().First(x => x.IdProducto == pid); q.Stock = 9; prn.Guardar(q);
Ok(prn.Listar().First(x => x.IdProducto == pid).Stock == 9, "Producto: actualizar");
prn.Eliminar(pid);
Ok(!prn.Listar().Any(x => x.IdProducto == pid), "Producto: eliminación lógica (no aparece)");
Ok(prn.ListarEliminados(null).Any(x => x.IdProducto == pid && x.FechaEliminacion != null), "Producto: aparece en eliminados con fecha");
prn.Restaurar(pid);
Ok(prn.Listar().Any(x => x.IdProducto == pid) && !prn.ListarEliminados(null).Any(x => x.IdProducto == pid), "Producto: restaurar (vuelve a activos)");
prn.Eliminar(pid);

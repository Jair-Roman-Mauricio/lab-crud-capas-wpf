using Entidades;
using Negocio;

void Ok(bool c, string m) { Console.WriteLine((c ? "OK   " : "FALLO ") + m); if (!c) Environment.ExitCode = 1; }

var categorias = new CategoriaNegocio().Listar();
Ok(categorias.Count >= 4, "Categoría: listar");
var idCat = categorias.First(c => c.Nombre == "Cómputo").IdCategoria;

var prn = new ProductoNegocio();
var pid = prn.Guardar(new Producto { Nombre = "Laptop", Descripcion = "14 pulgadas", Precio = 2500.50m, Stock = 5, IdCategoria = idCat });
Ok(pid > 0, "Producto: insertar");
Ok(prn.Listar().First(x => x.IdProducto == pid).Categoria == "Cómputo", "Producto: listar con INNER JOIN (nombre de categoría)");
Ok(prn.Buscar("lap").Any(x => x.IdProducto == pid), "Producto: buscar");
Ok(prn.Buscar("cómputo").Any(x => x.IdProducto == pid), "Producto: buscar por categoría");
var q = prn.Listar().First(x => x.IdProducto == pid); q.Stock = 9; prn.Guardar(q);
Ok(prn.Listar().First(x => x.IdProducto == pid).Stock == 9, "Producto: actualizar");
var r = prn.ResumenPorCategoria().First(x => x.Categoria == "Cómputo");
Ok(r.CantidadProductos >= 1 && r.StockTotal >= 9 && r.ValorInventario > 0, "Resumen: GROUP BY por categoría");
try { prn.Guardar(new Producto { Nombre = "X", Precio = 1, Stock = 1 }); Ok(false, "Producto: categoría obligatoria"); }
catch (ArgumentException) { Ok(true, "Producto: categoría obligatoria"); }
prn.Eliminar(pid);
Ok(!prn.Listar().Any(x => x.IdProducto == pid), "Producto: eliminación lógica (no aparece)");
Ok(prn.ListarEliminados(null).Any(x => x.IdProducto == pid && x.FechaEliminacion != null), "Producto: aparece en eliminados con fecha");
prn.Restaurar(pid);
Ok(prn.Listar().Any(x => x.IdProducto == pid) && !prn.ListarEliminados(null).Any(x => x.IdProducto == pid), "Producto: restaurar (vuelve a activos)");
prn.Eliminar(pid);

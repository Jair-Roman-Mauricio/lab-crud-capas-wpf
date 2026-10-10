-- Lab CRUD en capas: Producto (Sección B)
-- Eliminación lógica: columna Estado (1 = activo, 0 = eliminado)
IF DB_ID('LabCrud') IS NULL CREATE DATABASE LabCrud;
GO
USE LabCrud;
GO
IF OBJECT_ID('dbo.Producto') IS NULL
CREATE TABLE dbo.Producto (
    IdProducto  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(250) NULL,
    Precio      DECIMAL(10,2) NOT NULL CHECK (Precio >= 0),
    Stock       INT NOT NULL CHECK (Stock >= 0),
    Estado      BIT NOT NULL DEFAULT 1,          -- flag: 1 = activo, 0 = eliminado lógicamente
    FechaEliminacion DATETIME NULL                 -- flag auxiliar: cuándo se eliminó (NULL si está activo)
);
IF COL_LENGTH('dbo.Producto','FechaEliminacion') IS NULL
    ALTER TABLE dbo.Producto ADD FechaEliminacion DATETIME NULL;
GO
-- ===== PRODUCTO =====
CREATE OR ALTER PROCEDURE dbo.sp_Producto_Listar AS
    SELECT IdProducto,Nombre,Descripcion,Precio,Stock,Estado FROM dbo.Producto WHERE Estado=1 ORDER BY Nombre;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Producto_Buscar @Texto NVARCHAR(100) AS
    SELECT IdProducto,Nombre,Descripcion,Precio,Stock,Estado FROM dbo.Producto
    WHERE Estado=1 AND (Nombre LIKE '%'+@Texto+'%' OR Descripcion LIKE '%'+@Texto+'%') ORDER BY Nombre;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Producto_Insertar
    @Nombre NVARCHAR(100),@Descripcion NVARCHAR(250),@Precio DECIMAL(10,2),@Stock INT AS
    INSERT dbo.Producto(Nombre,Descripcion,Precio,Stock) VALUES(@Nombre,@Descripcion,@Precio,@Stock);
    SELECT CAST(SCOPE_IDENTITY() AS INT);
GO
CREATE OR ALTER PROCEDURE dbo.sp_Producto_Actualizar
    @IdProducto INT,@Nombre NVARCHAR(100),@Descripcion NVARCHAR(250),@Precio DECIMAL(10,2),@Stock INT AS
    UPDATE dbo.Producto SET Nombre=@Nombre,Descripcion=@Descripcion,Precio=@Precio,Stock=@Stock
    WHERE IdProducto=@IdProducto AND Estado=1;
    SELECT @@ROWCOUNT;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Producto_Eliminar @IdProducto INT AS
    UPDATE dbo.Producto SET Estado=0, FechaEliminacion=GETDATE() WHERE IdProducto=@IdProducto AND Estado=1;  -- eliminación lógica
    SELECT @@ROWCOUNT;
GO
-- ===== PAPELERA (registros eliminados lógicamente) =====
CREATE OR ALTER PROCEDURE dbo.sp_Producto_ListarEliminados @Texto NVARCHAR(100) = NULL AS
    SELECT IdProducto,Nombre,Descripcion,Precio,Stock,Estado,FechaEliminacion FROM dbo.Producto
    WHERE Estado=0 AND (@Texto IS NULL OR Nombre LIKE '%'+@Texto+'%' OR Descripcion LIKE '%'+@Texto+'%')
    ORDER BY FechaEliminacion DESC;
GO
CREATE OR ALTER PROCEDURE dbo.sp_Producto_Restaurar @IdProducto INT AS
    UPDATE dbo.Producto SET Estado=1, FechaEliminacion=NULL WHERE IdProducto=@IdProducto AND Estado=0;  -- revierte la eliminación
    SELECT @@ROWCOUNT;
GO

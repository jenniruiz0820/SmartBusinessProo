# SmartBusinessPro: Contexto de datos con EF Core

Aplicación Windows Forms (.NET 8) que se conecta a SQL Server mediante
Entity Framework Core y consulta los productos de un inventario.

## Tecnologías
- C# / .NET 8 / Windows Forms
- Entity Framework Core 8 (SqlServer)
- SQL Server Express

## Estructura
- `Models/`: entidades `Producto` y `Categoria`
- `Data/SmartBusinessContext.cs`: contexto centralizado
- `App.config`: cadena de conexión
- `Form1.cs`: interfaz y consulta de productos

## Configuración
1. Ejecutar el script SQL para crear la base de datos y las tablas.
2. Ajustar `Server` y `Database` en `App.config`.
3. Ejecutar el proyecto.

## Funciones
- Prueba de conexión con mensajes de error claros
- Consulta de productos con su categoría
- Buscador en vivo y resumen del inventario

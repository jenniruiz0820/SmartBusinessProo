using SmartBusinessProo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartBusinessProo.Models;

public class Producto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Existencia { get; set; }

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;
}
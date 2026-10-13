using System;
using System.Collections.Generic;
using System.Text;

namespace SmartBusinessProo.Models;

public class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
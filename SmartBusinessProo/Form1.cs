using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SmartBusinessProo.Data;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace SmartBusinessPro;

public partial class Form1 : Form
{
    // ───────── Paleta de colores ─────────
    private static readonly Color AzulOscuro = Color.FromArgb(31, 58, 95);
    private static readonly Color AzulClaro = Color.FromArgb(52, 120, 198);
    private static readonly Color Verde = Color.FromArgb(40, 167, 110);
    private static readonly Color Rojo = Color.FromArgb(214, 69, 65);
    private static readonly Color Naranja = Color.FromArgb(240, 150, 40);
    private static readonly Color Fondo = Color.FromArgb(244, 246, 248);
    private static readonly Color TextoGris = Color.FromArgb(100, 110, 125);

    private const int StockBajo = 20;

    // ───────── Controles ─────────
    private PanelDegradado pnlEncabezado = null!;
    private Panel pnlTarjetas = null!;
    private Panel pnlBotones = null!;
    private Panel pnlEstado = null!;
    private Panel pnlGrid = null!;
    private Button btnProbarConexion = null!;
    private Button btnConsultar = null!;
    private TextBox txtBuscar = null!;
    private DataGridView dgvProductos = null!;
    private Label lblEstado = null!;
    private Label lblTotalProductos = null!;
    private Label lblValorInventario = null!;
    private Label lblStockBajo = null!;

    // Datos en memoria (para el buscador)
    private List<ProductoVista> _productos = new();

    public Form1()
    {
        InitializeComponent();
        ConstruirInterfaz();
    }

    // ═══════════════ CONSTRUCCIÓN DE LA INTERFAZ ═══════════════
    private void ConstruirInterfaz()
    {
        Text = "SmartBusinessPro - Inventario";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1050, 640);
        MinimumSize = new Size(900, 560);
        Font = new Font("Segoe UI", 10f);
        BackColor = Fondo;

        ConstruirEncabezado();
        ConstruirTarjetas();
        ConstruirBarraBotones();
        ConstruirGrid();
        ConstruirBarraEstado();

        // El orden importa por el Dock: primero el Fill, al final lo más alto.
        Controls.Add(pnlGrid);
        Controls.Add(pnlEstado);
        Controls.Add(pnlBotones);
        Controls.Add(pnlTarjetas);
        Controls.Add(pnlEncabezado);

        EstablecerEstado("Listo. Presione \"Probar conexión\" para comenzar.", TextoGris);
    }

    private void ConstruirEncabezado()
    {
        pnlEncabezado = new PanelDegradado
        {
            Dock = DockStyle.Top,
            Height = 90,
            Color1 = AzulOscuro,
            Color2 = AzulClaro
        };

        var lblIcono = new Label
        {
            Text = "📦",
            Font = new Font("Segoe UI Emoji", 28f),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(18, 14)
        };

        var lblTitulo = new Label
        {
            Text = "SmartBusinessPro",
            Font = new Font("Segoe UI", 20f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(110, 10)
        };

        var lblSubtitulo = new Label
        {
            Text = "Sistema de inventario  |  Consulta de productos",
            Font = new Font("Segoe UI", 10f),
            ForeColor = Color.FromArgb(210, 225, 245),
            BackColor = Color.Transparent,
            AutoSize = true,
            Location = new Point(113, 52)
        };

        pnlEncabezado.Controls.Add(lblIcono);
        pnlEncabezado.Controls.Add(lblTitulo);
        pnlEncabezado.Controls.Add(lblSubtitulo);
    }

    private void ConstruirTarjetas()
    {
        pnlTarjetas = new Panel
        {
            Dock = DockStyle.Top,
            Height = 115,
            Padding = new Padding(20, 18, 20, 5),
            BackColor = Fondo
        };

        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            BackColor = Fondo
        };
        for (int i = 0; i < 3; i++)
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

        var t1 = CrearTarjeta("PRODUCTOS REGISTRADOS", AzulClaro, out lblTotalProductos);
        var t2 = CrearTarjeta("VALOR DEL INVENTARIO", Verde, out lblValorInventario);
        var t3 = CrearTarjeta($"STOCK BAJO (menos de {StockBajo})", Naranja, out lblStockBajo);
        t3.Margin = new Padding(0);

        tabla.Controls.Add(t1, 0, 0);
        tabla.Controls.Add(t2, 1, 0);
        tabla.Controls.Add(t3, 2, 0);
        pnlTarjetas.Controls.Add(tabla);
    }

    private Panel CrearTarjeta(string titulo, Color acento, out Label valor)
    {
        var card = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 15, 0)
        };

        var barra = new Panel { Dock = DockStyle.Left, Width = 6, BackColor = acento };

        var lblTit = new Label
        {
            Text = titulo,
            AutoSize = true,
            ForeColor = TextoGris,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            Location = new Point(22, 16)
        };

        valor = new Label
        {
            Text = "—",
            AutoSize = true,
            ForeColor = AzulOscuro,
            Font = new Font("Segoe UI Semibold", 22f),
            Location = new Point(20, 40)
        };

        card.Controls.Add(valor);
        card.Controls.Add(lblTit);
        card.Controls.Add(barra);
        return card;
    }

    private void ConstruirBarraBotones()
    {
        pnlBotones = new Panel
        {
            Dock = DockStyle.Top,
            Height = 68,
            BackColor = Fondo
        };

        btnProbarConexion = CrearBoton("🔌  Probar conexión", AzulClaro,
            new Point(20, 14), new Size(185, 42));
        btnProbarConexion.Click += btnProbarConexion_Click;

        btnConsultar = CrearBoton("📋  Consultar productos", Verde,
            new Point(218, 14), new Size(215, 42));
        btnConsultar.Click += btnConsultar_Click;

        txtBuscar = new TextBox
        {
            Width = 280,
            Font = new Font("Segoe UI", 11f),
            BorderStyle = BorderStyle.FixedSingle,
            PlaceholderText = "Buscar producto o categoría..."
        };
        txtBuscar.Location = new Point(pnlBotones.Width - txtBuscar.Width - 20, 19);
        txtBuscar.TextChanged += (s, e) => AplicarFiltro();

        // Mantener el buscador pegado a la derecha
        pnlBotones.Resize += (s, e) =>
            txtBuscar.Left = pnlBotones.Width - txtBuscar.Width - 20;

        pnlBotones.Controls.Add(btnProbarConexion);
        pnlBotones.Controls.Add(btnConsultar);
        pnlBotones.Controls.Add(txtBuscar);
    }

    private static Button CrearBoton(string texto, Color color, Point ubicacion, Size tamano)
    {
        var boton = new Button
        {
            Text = texto,
            Location = ubicacion,
            Size = tamano,
            FlatStyle = FlatStyle.Flat,
            BackColor = color,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 10f),
            Cursor = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color, 0.2f);
        boton.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color, 0.1f);
        return boton;
    }

    private void ConstruirGrid()
    {
        pnlGrid = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20, 5, 20, 15),
            BackColor = Fondo
        };

        dgvProductos = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            GridColor = Color.FromArgb(230, 233, 238),
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
            EnableHeadersVisualStyles = false,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            ColumnHeadersHeight = 42,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            ReadOnly = true,
            MultiSelect = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        dgvProductos.RowTemplate.Height = 38;

        dgvProductos.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = AzulOscuro,
            ForeColor = Color.White,
            Font = new Font("Segoe UI Semibold", 10f),
            Alignment = DataGridViewContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0),
            SelectionBackColor = AzulOscuro,
            SelectionForeColor = Color.White
        };

        dgvProductos.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White,
            ForeColor = Color.FromArgb(40, 45, 55),
            Font = new Font("Segoe UI", 10f),
            Padding = new Padding(10, 0, 10, 0),
            SelectionBackColor = Color.FromArgb(214, 230, 250),
            SelectionForeColor = Color.FromArgb(20, 30, 50)
        };

        dgvProductos.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(245, 248, 252)
        };

        // Existencia en rojo (stock bajo) o verde (stock suficiente)
        var fuenteExistencia = new Font("Segoe UI Semibold", 10f);
        dgvProductos.CellFormatting += (s, e) =>
        {
            if (e.RowIndex < 0 || e.Value is not int existencia) return;
            if (dgvProductos.Columns[e.ColumnIndex].DataPropertyName != "Existencia") return;

            var color = existencia < StockBajo ? Rojo : Verde;
            e.CellStyle!.ForeColor = color;
            e.CellStyle.SelectionForeColor = color;
            e.CellStyle.Font = fuenteExistencia;
        };

        pnlGrid.Controls.Add(dgvProductos);
    }

    private void ConstruirBarraEstado()
    {
        pnlEstado = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 40,
            BackColor = Color.White
        };

        var linea = new Panel
        {
            Dock = DockStyle.Top,
            Height = 1,
            BackColor = Color.FromArgb(225, 229, 235)
        };

        lblEstado = new Label
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f),
            Location = new Point(20, 10)
        };

        pnlEstado.Controls.Add(lblEstado);
        pnlEstado.Controls.Add(linea);
    }

    // ═══════════════ LÓGICA ═══════════════
    private async void btnProbarConexion_Click(object? sender, EventArgs e)
    {
        SetOcupado(true, "Conectando con el servidor...");
        try
        {
            using var db = new SmartBusinessContext();

            // OpenConnectionAsync lanza la SqlException real si algo falla,
            // lo que permite mostrar mensajes claros (CanConnectAsync solo devuelve false).
            await db.Database.OpenConnectionAsync();
            var conexion = db.Database.GetDbConnection();

            EstablecerEstado(
                $"Conexión exitosa  |  Servidor: {conexion.DataSource}  |  Base de datos: {conexion.Database}",
                Verde);

            await db.Database.CloseConnectionAsync();
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
        finally
        {
            SetOcupado(false);
        }
    }

    private async void btnConsultar_Click(object? sender, EventArgs e)
    {
        SetOcupado(true, "Consultando productos...");
        try
        {
            using var db = new SmartBusinessContext();

            _productos = await db.Productos
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new ProductoVista(
                    p.Nombre, p.Precio, p.Existencia, p.Categoria.Nombre))
                .ToListAsync();

            AplicarFiltro();
            ActualizarTarjetas();
            EstablecerEstado($"{_productos.Count} productos recuperados correctamente.", Verde);
        }
        catch (Exception ex)
        {
            MostrarError(ex);
        }
        finally
        {
            SetOcupado(false);
        }
    }

    private void AplicarFiltro()
    {
        string texto = txtBuscar.Text.Trim();

        var lista = string.IsNullOrEmpty(texto)
            ? _productos
            : _productos.Where(p =>
                p.Producto.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                p.Categoria.Contains(texto, StringComparison.OrdinalIgnoreCase)).ToList();

        dgvProductos.DataSource = lista;
        FormatearColumnas();
    }

    private void FormatearColumnas()
    {
        if (dgvProductos.Columns.Count == 0) return;

        dgvProductos.Columns["Producto"]!.HeaderText = "Producto";
        dgvProductos.Columns["Producto"]!.FillWeight = 45;

        var precio = dgvProductos.Columns["Precio"]!;
        precio.HeaderText = "Precio";
        precio.DefaultCellStyle.Format = "C2";
        precio.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        precio.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        precio.FillWeight = 15;

        var existencia = dgvProductos.Columns["Existencia"]!;
        existencia.HeaderText = "Existencia";
        existencia.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        existencia.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        existencia.FillWeight = 15;

        dgvProductos.Columns["Categoria"]!.HeaderText = "Categoría";
        dgvProductos.Columns["Categoria"]!.FillWeight = 25;

        dgvProductos.ClearSelection();
    }

    private void ActualizarTarjetas()
    {
        lblTotalProductos.Text = _productos.Count.ToString();
        lblValorInventario.Text = _productos.Sum(p => p.Precio * p.Existencia).ToString("C2");
        lblStockBajo.Text = _productos.Count(p => p.Existencia < StockBajo).ToString();
    }

    private void SetOcupado(bool ocupado, string? mensaje = null)
    {
        btnProbarConexion.Enabled = !ocupado;
        btnConsultar.Enabled = !ocupado;
        UseWaitCursor = ocupado;
        if (ocupado && mensaje != null)
            EstablecerEstado(mensaje, AzulClaro);
    }

    private void EstablecerEstado(string texto, Color color)
    {
        lblEstado.Text = "●  " + texto;
        lblEstado.ForeColor = color;
    }

    private void MostrarError(Exception ex)
    {
        // EF Core puede envolver la SqlException; buscamos la original.
        var sqlEx = ex as SqlException ?? ex.InnerException as SqlException;

        string mensaje = sqlEx?.Number switch
        {
            53 or -1 or 2 or 1326 =>
                "No se encontró el servidor SQL. Verifique el nombre del servidor/instancia y que el servicio esté iniciado.",
            4060 =>
                "El servidor existe, pero la base de datos no existe o el usuario no tiene acceso a ella.",
            18456 =>
                "Error de autenticación: usuario o contraseña incorrectos.",
            -2 =>
                "Tiempo de espera agotado al conectar con el servidor.",
            null when ex is InvalidOperationException =>
                ex.Message,
            _ => $"Error inesperado: {ex.Message}"
        };

        EstablecerEstado("Error de conexión: " + mensaje, Rojo);
        MessageBox.Show(mensaje, "SmartBusinessPro",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}

// ═══════════════ CLASES AUXILIARES ═══════════════

/// <summary>Fila que se muestra en el DataGridView.</summary>
public record ProductoVista(string Producto, decimal Precio, int Existencia, string Categoria);

/// <summary>Panel con fondo en degradado horizontal.</summary>
public class PanelDegradado : Panel
{
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color Color1 { get; set; } = Color.Black;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color Color2 { get; set; } = Color.Black;

    public PanelDegradado()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (Width <= 0 || Height <= 0) return;
        using var pincel = new LinearGradientBrush(
            ClientRectangle, Color1, Color2, LinearGradientMode.Horizontal);
        e.Graphics.FillRectangle(pincel, ClientRectangle);
    }
}
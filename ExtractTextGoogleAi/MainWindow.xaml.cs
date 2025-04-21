using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using ExtractTextGoogleAi.Services;
using System.Windows.Controls;
using ExtractTextGoogleAi.Models;
using System.Text;

namespace ExtractTextGoogleAi
{
    public partial class MainWindow : Window
    {
        private string _imagePath = string.Empty; // Initialize to avoid nullability warning
        private GoogleVisionService _visionService;
        private GeminiAiService _geminiService = new GeminiAiService();
        private List<ProductoExtraido> _productos;

        private bool _modoEdicion = false;

        public MainWindow()
        {
            InitializeComponent();
            _visionService = new GoogleVisionService();
            ActualizarEstadosDeBotones();
        }

        private void ActualizarEstadosDeBotones()
        {
            bool hayImagen = imgPreview.Source != null;
            bool hayDatos = _productos != null && _productos.Any();

            btnUpload.IsEnabled = !hayImagen;
            btnCopy.IsEnabled = hayDatos;
            btnExport.IsEnabled = hayDatos;
            btnClear.IsEnabled = hayImagen || hayDatos;
            btnEditarGuardar.IsEnabled = hayDatos;
        }

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog
            {
                Title = "Seleccionar imagen",
                Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
            };

            if (openFile.ShowDialog() == true)
            {
                _imagePath = openFile.FileName;
                BitmapImage bitmap = new BitmapImage(new Uri(_imagePath));
                imgPreview.Source = bitmap;

                // OCR con Google Vision
                var result = await _visionService.ExtractTextWithBoundsAsync(_imagePath);

                // Dibujar los rectángulos sobre las palabras
                DrawBoundingBoxes(result.Boxes, bitmap);

                // 🔥 Enviar el texto extraído a Gemini AI
                try
                {
                    string filteredData = await _geminiService.GetFilteredProductDataAsync(result.FullText);

                    // ✅ Mostrar el resultado filtrado directamente en el TextBox
                    //txtExtractedText.Text = filteredData;
                    Console.WriteLine("Texto procesado por Gemini:\n" + filteredData);

                    _productos = ParsearTextoFiltrado(filteredData);
                    dataGridResultados.ItemsSource = _productos;
                }
                catch (Exception ex)
                {
                    //txtExtractedText.Text = $"Error al comunicarse con Gemini: {ex.Message}";
                    MessageBox.Show($"Error al comunicarse con Gemini: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            ActualizarEstadosDeBotones();
        }

        private void BtnEditarGuardar_Click(object sender, RoutedEventArgs e)
        {
            _modoEdicion = !_modoEdicion;

            if (_modoEdicion)
            {
                btnEditarGuardar.Content = "💾 Guardar";
                dataGridResultados.IsReadOnly = false;
            }
            else
            {
                btnEditarGuardar.Content = "✏️ Editar";
                dataGridResultados.CommitEdit(); // Guardar cambios locales
                dataGridResultados.IsReadOnly = true;
                MessageBox.Show("Cambios guardados correctamente.", "Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DrawBoundingBoxes(List<OcrResult.OcrBoundingBox> boxes, BitmapImage image)
        {
            overlayCanvas.Children.Clear();

            double scaleX = imgPreview.ActualWidth / image.PixelWidth;
            double scaleY = imgPreview.ActualHeight / image.PixelHeight;

            foreach (var box in boxes)
            {
                if (box.Vertices.Count < 4) continue;

                var rect = new System.Windows.Shapes.Rectangle
                {
                    Stroke = System.Windows.Media.Brushes.Red,
                    StrokeThickness = 1,
                    Fill = System.Windows.Media.Brushes.Transparent
                };

                var x = Math.Min(box.Vertices[0].X, box.Vertices[2].X) * scaleX;
                var y = Math.Min(box.Vertices[0].Y, box.Vertices[2].Y) * scaleY;
                var width = Math.Abs(box.Vertices[0].X - box.Vertices[2].X) * scaleX;
                var height = Math.Abs(box.Vertices[0].Y - box.Vertices[2].Y) * scaleY;

                Canvas.SetLeft(rect, x);
                Canvas.SetTop(rect, y);
                rect.Width = width;
                rect.Height = height;

                overlayCanvas.Children.Add(rect);
            }
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (_productos != null && _productos.Any())
            {
                var texto = new StringBuilder();
                foreach (var p in _productos)
                {
                    texto.AppendLine($"Código: {p.Codigo}, Descripción: {p.Descripcion}, Cantidad: {p.Cantidad}, Precio: {p.PrecioUnitario}");
                }

                Clipboard.SetText(texto.ToString());
                MessageBox.Show("Texto copiado desde los resultados a la tabla.", "Copiado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            imgPreview.Source = null;
            overlayCanvas.Children.Clear();
            _imagePath = string.Empty;
            _productos = new List<ProductoExtraido>();
            dataGridResultados.ItemsSource = null;
            ActualizarEstadosDeBotones();
        }

        private List<ProductoExtraido> ParsearTextoFiltrado(string texto)
        {
            var productos = new List<ProductoExtraido>();
            var lineas = texto.Split('\n');
            ProductoExtraido? producto = null;

            foreach (var linea in lineas)
            {
                if (linea.Contains("Código"))
                {
                    // Si ya hay un producto en progreso, lo agregamos
                    if (producto != null)
                    {
                        productos.Add(producto);
                    }

                    producto = new ProductoExtraido
                    {
                        Codigo = linea.Split(':')[1].Trim()
                    };
                }
                else if (linea.Contains("Descripción") && producto != null)
                {
                    producto.Descripcion = linea.Split(':')[1].Trim();
                }
                else if (linea.Contains("Cantidad") && producto != null)
                {
                    producto.Cantidad = linea.Split(':')[1].Trim();
                }
                else if (linea.Contains("Precio") && producto != null)
                {
                    producto.PrecioUnitario = linea.Split(':')[1].Trim();
                }
            }

            // Agregamos el último producto (si existe)
            if (producto != null)
            {
                productos.Add(producto);
            }

            return productos;
        }


        private void BtnExportarCSV_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "productos_extraidos.csv"
            };

            if (saveDialog.ShowDialog() == true)
            {
                ExportarCSV(_productos, saveDialog.FileName);
                MessageBox.Show("Exportación completada.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportarCSV(List<ProductoExtraido> productos, string path)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Código,Descripción,Cantidad,Precio Unitario");

            foreach (var p in productos)
            {
                sb.AppendLine($"{p.Codigo},{p.Descripcion},{p.Cantidad},{p.PrecioUnitario}");
            }

            File.WriteAllText(path, sb.ToString());
        }
    }
}

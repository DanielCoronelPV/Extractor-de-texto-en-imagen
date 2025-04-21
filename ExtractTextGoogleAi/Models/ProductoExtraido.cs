using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractTextGoogleAi.Models
{
    internal class ProductoExtraido
    {
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Cantidad { get; set; } = string.Empty;
        public string PrecioUnitario { get; set; } = string.Empty;
    }
}

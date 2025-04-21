using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Vision.V1;

namespace ExtractTextGoogleAi.Services
{
    public class OcrResult
    {
        public string FullText { get; set; } = string.Empty; // Initialize to avoid null issues  
        public List<OcrBoundingBox> Boxes { get; set; } = new List<OcrBoundingBox>(); // Initialize to avoid null issues  
   
        public class OcrBoundingBox
        {
            public string Text { get; set; } = string.Empty; // Initialize to avoid null issues  
            public IList<Vertex> Vertices { get; set; } = new List<Vertex>(); // Initialize to avoid null issues  
        }
    }
}

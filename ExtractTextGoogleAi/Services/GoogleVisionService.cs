using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Vision.V1;
using System.IO;
using ExtractTextGoogleAi.Services;
using static ExtractTextGoogleAi.Services.OcrResult;

namespace ExtractTextGoogleAi.Services
{
    public class GoogleVisionService
    {
        private readonly ImageAnnotatorClient _client;

        public GoogleVisionService()
        {
            // Ruta al archivo de credenciales (ajusta según tu ubicación)
            string credentialPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "google-credentials.json");
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credentialPath);

            _client = ImageAnnotatorClient.Create();
        }

        public async Task<string> ExtractTextFromImage(string imagePath)
        {
            try
            {
                var image = await Image.FromFileAsync(imagePath);
                var response = await _client.DetectTextAsync(image);

                if (response.Count == 0)
                    return "No se encontró texto en la imagen.";

                return string.Join("\n", response);
            }
            catch (Exception ex)
            {
                return $"Error al procesar la imagen: {ex.Message}";
            }
        }

        public async Task<OcrResult> ExtractTextWithBoundsAsync(string imagePath)
        {
            var image = await Image.FromFileAsync(imagePath);
            var response = await _client.DetectDocumentTextAsync(image);

            if (response == null || response.Pages.Count == 0)
                return new OcrResult { FullText = "No se encontró texto", Boxes = new List<OcrBoundingBox>() };

            var fullText = response.Text;
            var boxes = new List<OcrBoundingBox>();

            foreach (var page in response.Pages)
            {
                foreach (var block in page.Blocks)
                {
                    foreach (var paragraph in block.Paragraphs)
                    {
                        foreach (var word in paragraph.Words)
                        {
                            var wordText = string.Join("", word.Symbols.Select(s => s.Text));
                            boxes.Add(new OcrBoundingBox
                            {
                                Text = wordText,
                                Vertices = word.BoundingBox.Vertices
                            });
                        }
                    }
                }
            }

            return new OcrResult
            {
                FullText = fullText,
                Boxes = boxes
            };

        }
    }
}

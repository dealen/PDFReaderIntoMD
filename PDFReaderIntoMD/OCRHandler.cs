using Newtonsoft.Json;
using Spectre.Console;
using System.Net.Http.Headers;
using System.Text;

namespace PDFReaderIntoMD
{
    internal class OCRHandler
    {
        private readonly string _apiKey;

        public OCRHandler(string apiKey)
        {
            _apiKey = apiKey;
        }

        internal async Task<OCRUploadResponse?> UploadAFile(string filePath)
        {
            var url = "https://api.mistral.ai/v1/files";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            using var form = new MultipartFormDataContent
            {
                { new StringContent("ocr"), "purpose" }
            };
            using var fileStream = new FileStream(filePath, FileMode.Open);
            using var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            form.Add(fileContent, "file", Path.GetFileName(filePath));

            HttpResponseMessage response = await client.PostAsync(url, form);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var ocrUploadResponse = JsonConvert.DeserializeObject<OCRUploadResponse>(responseBody);
                return ocrUploadResponse;
            }
            else
            {
                AnsiConsole.MarkupLine($"An error occurred while uploading the file: {response.ReasonPhrase}");
                return null;
            }
        }

        internal async Task<OCRFileInfoResponse?> GetUrlOfUploadedFile(string fileId)
        {
            var url = $"https://api.mistral.ai/v1/files/{fileId}/url?expiry=24";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                try
                {
                    return JsonConvert.DeserializeObject<OCRFileInfoResponse>(responseBody);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"An error occurred while deserializing the response: {ex.Message}");
                    return null;
                }
            }
            else
            {
                AnsiConsole.MarkupLine($"An error occurred while getting the URL of the uploaded file: {response.ReasonPhrase}");
                return null;
            }
        }

        internal async Task ConvertPDFToMD(string documentUrl, string outputPath)
        {
            var apiUrl = "https://api.mistral.ai/v1/ocr";
            var model = "mistral-ocr-latest";

            var client = new HttpClient();
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            try
            {
                var requestBody = new
                {
                    model = model,
                    document = new
                    {
                        type = "document_url",
                        document_url = documentUrl
                    },
                    include_image_base64 = true
                };

                var ocrRequestJson = JsonConvert.SerializeObject(requestBody);
                var ocrContent = new StringContent(ocrRequestJson, Encoding.UTF8, "application/json");

                HttpResponseMessage ocrResponse = await client.PostAsync(apiUrl, ocrContent);
                ocrResponse.EnsureSuccessStatusCode();

                string ocrResponseBody = await ocrResponse.Content.ReadAsStringAsync();
                var ocrDocument = JsonConvert.DeserializeObject<OCRDocumentResponse>(ocrResponseBody);

                AnsiConsole.MarkupLine($"OCR Request successful with pages count: [bold]{ocrDocument.Pages.Count}[/]");

                var outputContent = new StringBuilder();
                foreach (var page in ocrDocument.Pages)
                {
                    outputContent.AppendLine(page.Markdown);
                }

                // Save the OCR response to the output file
                File.WriteAllText(outputPath, outputContent.ToString());
                AnsiConsole.MarkupLine($"OCR output saved to [bold]{outputPath}[/]");
            }
            catch (HttpRequestException e)
            {
                AnsiConsole.MarkupLine($"An error occurred while uploading the file: {e.Message}");
                AnsiConsole.MarkupLine($"Please check the file path and try again.");
            }
        }
    }
}

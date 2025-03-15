using Newtonsoft.Json;

namespace PDFReaderIntoMD
{
    internal class OCRRequest
    {
        [JsonProperty("model")]
        public string Model { get; set; }
        [JsonProperty("document")]
        public Document Document { get; set; }
        [JsonProperty("include_text_details")]
        public bool IncludeImageBase64 { get; set; }
    }

    internal class Document
    {
        [JsonProperty("document_id")]
        public string Type { get; set; }
        [JsonProperty("document_url")]
        public string DocumentUrl { get; set; }
    }
}

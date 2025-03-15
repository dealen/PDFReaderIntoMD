using Newtonsoft.Json;

namespace PDFReaderIntoMD
{
    internal class OCRUploadResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("bytes")]
        public int Bytes { get; set; }
        [JsonProperty("created_at")]
        public int CreatedAt { get; set; }
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("purpose")]
        public string Purpose { get; set; }
        [JsonProperty("sample_type")]
        public string SampleType { get; set; }
        [JsonProperty("num_lines")]
        public int? NumLines { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
    }
}

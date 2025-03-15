using Newtonsoft.Json;

public class OCRDocumentResponse
{
    [JsonProperty("pages")]
    public List<Page> Pages { get; set; }

    [JsonProperty("model")]
    public string Model { get; set; }

    [JsonProperty("usage_info")]
    public UsageInfo UsageInfo { get; set; }
}

public class Page
{
    [JsonProperty("index")]
    public int Index { get; set; }

    [JsonProperty("markdown")]
    public string Markdown { get; set; }

    [JsonProperty("images")]
    public List<Image> Images { get; set; }

    [JsonProperty("dimensions")]
    public Dimensions Dimensions { get; set; }
}

public class Image
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("top_left_x")]
    public int TopLeftX { get; set; }

    [JsonProperty("top_left_y")]
    public int TopLeftY { get; set; }

    [JsonProperty("bottom_right_x")]
    public int BottomRightX { get; set; }

    [JsonProperty("bottom_right_y")]
    public int BottomRightY { get; set; }

    [JsonProperty("image_base64")]
    public string ImageBase64 { get; set; }
}

public class Dimensions
{
    [JsonProperty("dpi")]
    public int Dpi { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }
}

public class UsageInfo
{
    [JsonProperty("pages_processed")]
    public int PagesProcessed { get; set; }

    [JsonProperty("doc_size_bytes")]
    public int DocSizeBytes { get; set; }
}

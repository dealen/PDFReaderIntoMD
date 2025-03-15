using PDFReaderIntoMD;
using Spectre.Console;

AnsiConsole.Markup("Hello, [bold green]world[/]!");

var apiKey = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter your Mistral API key:")
                .PromptStyle("red")
                .Secret()
                .InvalidChoiceMessage("[red]Please enter a valid API key.[/]")
        );

var ocrHandler = new OCRHandler(apiKey);

var isSingleFile = AnsiConsole.Confirm("Do you want to convert a single PDF file to Markdown?");
AnsiConsole.MarkupLine($"You chose: [bold]{(isSingleFile ? "Yes" : "No")}[/]");
if (!isSingleFile)
{
    var direcotryPath = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter the directory path containing the PDF files:")
            .PromptStyle("blue")
            .Validate(filesInDirectory =>
            {
                // Validate that the directory exists
                if (Directory.Exists(filesInDirectory))
                {
                    return ValidationResult.Success();
                }
                else
                {
                    return ValidationResult.Error("[red]The directory does not exist.[/]");
                }
            })
            .InvalidChoiceMessage("[red]Please enter a valid directory path containing the PDF files.[/]"));

    AnsiConsole.MarkupLine($"You entered: [bold]{direcotryPath}[/]");

    var outputDirectoryPath = AnsiConsole.Prompt(
        new TextPrompt<string>("Enter the output directory path for the Markdown files:")
            .PromptStyle("blue")
            .Validate(outputDirectory =>
            {
                // Validate that the output directory exists
                if (Directory.Exists(outputDirectory))
                {
                    return ValidationResult.Success();
                }
                else
                {
                    return ValidationResult.Error("[red]The output directory does not exist.[/]");
                }
            })
            .InvalidChoiceMessage("[red]Please enter a valid output directory path for the Markdown files.[/]"));

    AnsiConsole.MarkupLine($"You entered: [bold]{outputDirectoryPath}[/]");

    var files = Directory.GetFiles(direcotryPath, "*.pdf");

    foreach (var file in files)
    {
        if (!File.Exists(file))
        {
            continue;
        }

        if (!file.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        AnsiConsole.MarkupLine($"Converting file: [bold]{file}[/]");

        await ConvertPdfToMarkdown(ocrHandler, file);
    }

    return;
}

var filePath = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter the file path to the PDF file:")
                .Validate(filePath =>
                {
                    // Validate that the file path ends with .pdf
                    if (filePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                    {
                        return ValidationResult.Success();
                    }
                    else
                    {
                        return ValidationResult.Error("[red]The file path must end with .pdf[/]");
                    }
                })
                .InvalidChoiceMessage("[red]Please enter a valid file path to a PDF file.[/]")
        );

AnsiConsole.MarkupLine($"You entered: [bold]{filePath}[/]");

await ConvertPdfToMarkdown(ocrHandler, filePath);

static async Task ConvertPdfToMarkdown(OCRHandler ocrHandler, string filePath)
{
    var outputPath = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter the output path for the Markdown file:")
                    .PromptStyle("blue")
                    .Validate(outputPath =>
                    {
                        // Validate that the output path ends with .md
                        if (outputPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                        {
                            return ValidationResult.Success();
                        }
                        else
                        {
                            return ValidationResult.Error("[red]The output path must end with .md[/]");
                        }
                    })
                    .InvalidChoiceMessage("[red]Please enter a valid output path for the Markdown file.[/]")
            );

    AnsiConsole.MarkupLine($"You entered: [bold]{outputPath}[/]");

    var ocrUploadResponse = await ocrHandler.UploadAFile(filePath);

    if (ocrUploadResponse != null)
    {
        AnsiConsole.MarkupLine($"File uploaded successfully. File ID: [bold]{ocrUploadResponse.Id}[/]");
        var uploadInfo = await ocrHandler.GetUrlOfUploadedFile(ocrUploadResponse.Id);
        AnsiConsole.MarkupLine($"URL of the uploaded file: [bold]{uploadInfo.Url}[/]");

        AnsiConsole.MarkupLine("Converting the PDF file to Markdown...");

        await ocrHandler.ConvertPDFToMD(uploadInfo.Url, outputPath);
    }
    else
    {
        AnsiConsole.MarkupLine("[red]An error occurred while uploading the file.[/]");
    }
}

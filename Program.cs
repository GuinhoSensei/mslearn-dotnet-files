using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles, out StringBuilder salesDetails);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

// Create the sales summary report file
var salesSummaryFilePath = Path.Combine(salesTotalDir, "SalesSummaryReport.txt");
File.WriteAllText(salesSummaryFilePath, GenerateSalesSummary(salesTotal, salesDetails));

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles, out StringBuilder salesDetails)
{
    double salesTotal = 0;
    salesDetails = new StringBuilder();

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        double fileTotal = data?.Total ?? 0;
        salesTotal += fileTotal;

        // Append details for the current file
        salesDetails.AppendLine($" {Path.GetFileName(file)}: {fileTotal.ToString("C")}");
    }

    return salesTotal;
}

string GenerateSalesSummary(double totalSales, StringBuilder details)
{
    StringBuilder reportBuilder = new StringBuilder();
    reportBuilder.AppendLine("Sales Summary");
    reportBuilder.AppendLine("----------------------------");
    reportBuilder.AppendLine($" Total Sales: {totalSales.ToString("C")}");
    reportBuilder.AppendLine();
    reportBuilder.AppendLine(" Details:");
    reportBuilder.Append(details.ToString());

    return reportBuilder.ToString();
}

record SalesData(double Total);

// using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
// using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using System.Globalization;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

CultureInfo.CurrentCulture = new CultureInfo("en-US");

var titleRegex = new Regex(@"Title:\s*(?<title>[^\r\n]+)", RegexOptions.IgnoreCase);
var dateRegex = new Regex(@"Date:\s*(?<date>\b\d{1,4}[/.-]\d{1,2}[/.-]\d{2,4}\b)", RegexOptions.IgnoreCase);
var timeRegex = new Regex(@"Time:\s*(?<time>\b\d{1,2}:\d{2}(?::\d{2})?\s*(?:[AP]\.?M\.?)?\b)", RegexOptions.IgnoreCase);


var files = Directory.EnumerateFiles(@"C:\Users\user\Downloads\Tickets\Tickets", "*.pdf");

using(StreamWriter sw = new StreamWriter("C:\\Users\\user\\Downloads\\Tickets\\Tickets\\TicketsInfo.txt"))
{
    foreach (var file in files)
    {
        var tickets = getTicketInfo(file);
        foreach (var ticket in tickets)
        {
            /*var ticketDate = $"{ticket.Date} {ticket.Time}";
            var date = DateTime.Parse(ticketDate, CultureInfo.InvariantCulture);
            Console.WriteLine(ticket);
            Console.WriteLine($"Ticket date formatted: {date.Month}/{date.Day}/{date.Year} hour?:{date.Hour}:{date.Minute}");*/

            sw.WriteLine(printTicket(ticket));

        }
    }
}

Console.WriteLine("All the tickets has been read.");


var ticketsInfo = getTicketInfo(@"C:\Users\user\Downloads\Tickets\Tickets\Tickets1.pdf");
Console.ReadKey();

List<Ticket> getTicketInfo(string pdfPath)
{
    var tickets = new List<Ticket>();
    using (PdfDocument document = PdfDocument.Open(pdfPath))
    {
        foreach (Page page in document.GetPages())
        {
            string text = ContentOrderTextExtractor.GetText(page);
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Ticket currentTicket = null;

            foreach (var line in lines)
            {
                var titleMatch = titleRegex.Match(line);

                if (titleMatch.Success)
                {
                    if (currentTicket != null) tickets.Add(currentTicket);

                    currentTicket = new Ticket
                    {
                        Title = titleMatch.Groups["title"].Value.Trim()
                    };
                    continue;
                }

                if (currentTicket == null) continue;

                var dateMatch = dateRegex.Match(line);
                if (dateMatch.Success)
                {
                    currentTicket.Date = dateMatch.Groups["date"].Value.Trim();
                    continue;
                }

                var timeMatch = timeRegex.Match(line);
                if (timeMatch.Success)
                {
                    currentTicket.Time = timeMatch.Groups["time"].Value.Trim();
                    continue;
                }
            }

            if (currentTicket != null) tickets.Add(currentTicket);

            //IEnumerable<Word> words = page.GetWords(NearestNeighbourWordExtractor.Instance);

            /*foreach(var word in words)
            {
                Console.WriteLine(word);
            }*/

        }
    }
    return tickets;
}

string printTicket(Ticket ticket)
{
    var ticketDate = $"{ticket.Date} {ticket.Time}";
    var date = DateTime.Parse(ticketDate, CultureInfo.InvariantCulture);
    return $"{ticket.Title,-30} | {date.Month}/{date.Day}/{date.Year} | {date.Hour}:{date.Minute}";
}

public class Ticket
{
    public string Title { get; set; }
    public string Date { get; set; }
    public string Time { get; set; }

    public override string? ToString()
    {
        return $"Title: {Title}\n Date: {Date}\n Time: {Time}";
    }
}


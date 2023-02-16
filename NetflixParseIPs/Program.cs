using HtmlAgilityPack;
using System.Net;
using System.Text;

namespace NetflixParseIPs
{
    class Program
    {
        static void Main(string[] args)
        {
            IPParser parser = new();
            StringBuilder result = new();
            HtmlWeb web = new()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36"
            };

            HtmlDocument doc = web.Load("https://ipinfo.io/AS2906");

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//div[@id='ipv4-data']/table");
            HtmlNodeCollection rows = table.SelectNodes("tbody/tr");

            foreach (HtmlNode row in rows)
            {
                string netblock = row.SelectSingleNode("td[1]/a").InnerText.Trim();
                string company = row.SelectSingleNode("td[2]/span").InnerText.Trim();

                result.AppendLine(parser.ParseIP(netblock, "COMPANY : " + company));
            }

            string remoteAddress = parser.GetRemoteAddress("https://www.netflix.com/msl/playapi/cadmium/event/1?");

            doc = web.Load($"https://ipinfo.io/{remoteAddress}");

            HtmlNode ipRangeNode = doc.DocumentNode.SelectSingleNode("//tr[3]/td[2]//a");
            
            result.AppendLine(parser.ParseIP(ipRangeNode.InnerHtml, "Netflix Watch History API - Caution!!! AWS IP address ranges"));

            Console.WriteLine(result);

            File.WriteAllText("result.txt", result.ToString());
        }
    }

    public class IPParser
    {
        private readonly Dictionary<string, string> _prefixToNetmask;

        public IPParser()
        {
            _prefixToNetmask = new Dictionary<string, string>()
            {
                ["/30"] = "255.255.255.252",
                ["/29"] = "255.255.255.248",
                ["/28"] = "255.255.255.240",
                ["/27"] = "255.255.255.224",
                ["/26"] = "255.255.255.192",
                ["/25"] = "255.255.255.128",
                ["/24"] = "255.255.255.0",
                ["/23"] = "255.255.254.0",
                ["/22"] = "255.255.252.0",
                ["/21"] = "255.255.248.0",
                ["/20"] = "255.255.240.0",
                ["/19"] = "255.255.224.0",
                ["/18"] = "255.255.192.0",
                ["/17"] = "255.255.128.0",
                ["/16"] = "255.255.0.0",
                ["/15"] = "255.254.0.0",
                ["/14"] = "255.252.0.0",
                ["/13"] = "255.248.0.0",
                ["/12"] = "255.240.0.0",
                ["/11"] = "255.224.0.0",
                ["/10"] = "255.192.0.0",
                ["/9"] = "255.128.0.0",
                ["/8"] = "255.0.0.0"
            };
        }

        public string ParseIP(string ipAddress, string annotate)
        {
            string prefix = ipAddress.Split('/')[1];
            string netmask = _prefixToNetmask[$"/{prefix}"];

            return $"route {ipAddress.Split('/')[0]} {netmask} # {annotate}";
        }

        public string GetRemoteAddress(string url)
        {
            Uri myUri = new Uri(url);
            return Dns.GetHostAddresses(myUri.Host)[0].ToString();
        }
    }
}

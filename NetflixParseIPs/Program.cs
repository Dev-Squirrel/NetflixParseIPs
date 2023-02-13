using HtmlAgilityPack;

namespace NetflixParseIPs
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> Legend = new()
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

            HtmlWeb web = new()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36"
            };

            HtmlDocument doc = web.Load("https://ipinfo.io/AS2906");

            string result = "";

            HtmlNode table = doc.DocumentNode.SelectSingleNode("//div[@id='ipv4-data']/table");
            HtmlNodeCollection rows = table.SelectNodes("tbody/tr");

            foreach (HtmlNode row in rows)
            {
                string netblock = row.SelectSingleNode("td[1]/a").InnerText.Trim();
                string company = row.SelectSingleNode("td[2]/span").InnerText.Trim();
                string numOfIPs = row.SelectSingleNode("td[3]").InnerText.Trim();

                //Console.WriteLine("Netblock: " + netblock);
                //Console.WriteLine("Company: " + company);
                //Console.WriteLine("Num of IPs: " + numOfIPs);
                //Console.WriteLine("--------------------");

                result += ParseIP(netblock, company, numOfIPs, Legend) + Environment.NewLine;
            }

            Console.WriteLine(result);

            File.WriteAllText("result.txt", result);
        }

        static string ParseIP(string ipAddress, string company, string numOfIPs, Dictionary<string, string> Legend)
        {
            string prefix = ipAddress.Split('/')[1];
            string netmask = Legend[$"/{prefix}"];

            return $"route {ipAddress.Split('/')[0]} {netmask} # COMPANY : {company} / NUM OF IPS : {numOfIPs}";
        }
    }
}

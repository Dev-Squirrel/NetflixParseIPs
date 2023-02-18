using HtmlAgilityPack;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NetflixParseIPs
{
    class Program
    {
        static void Main(string[] args)
        {
            IPParser parser = new();
            StringBuilder result = new();
            HtmlDocument doc;
            HtmlWeb web = new()
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36"
            };

            string[] networks = { "AS2906", "AS40027", "AS55095", "AS394406" };

            foreach (string network in networks)
            {
                result.AppendLine($"# Netflix : https://ipinfo.io/{network}");

                doc = web.Load($"https://ipinfo.io/{network}");

                HtmlNode table = doc.DocumentNode.SelectSingleNode("//div[@id='ipv4-data']/table");

                foreach (HtmlNode row in table.SelectNodes("tbody/tr"))
                {
                    string netblock = row.SelectSingleNode("td[1]/a").InnerText.Trim();
                    string company = row.SelectSingleNode("td[2]/span").InnerText.Trim();

                    result.AppendLine(parser.ParseIP(netblock, "COMPANY : " + company));
                }
            }

            result.AppendLine($"# AWS (us-west-2) EC2 : https://ip-ranges.amazonaws.com/ip-ranges.json");

            using (HttpClient client = new())
            {
                List<string> ipPrefixes = new();

                string awsRangesJson = client.GetStringAsync("https://ip-ranges.amazonaws.com/ip-ranges.json").Result;
                JsonElement jsonData = JsonSerializer.Deserialize<JsonElement>(awsRangesJson);

                foreach (JsonElement item in jsonData.GetProperty("prefixes").EnumerateArray())
                {
                    if (item.GetProperty("network_border_group").GetString() == "us-west-2" && item.GetProperty("service").GetString() == "EC2")
                    {
                        string? ipAddress = item.GetProperty("ip_prefix").GetString();

                        if (!string.IsNullOrWhiteSpace(ipAddress))
                        {
                            ipPrefixes.Add(parser.ParseIP(ipAddress, "COMPANY : AWS"));
                        }
                    }
                }

                ipPrefixes.Sort(parser.CompareIPAddresses);

                foreach (string ipPrefix in ipPrefixes)
                {
                    result.AppendLine(ipPrefix);
                }
            }

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
                ["/32"] = "255.255.255.255",
                ["/31"] = "255.255.255.254",
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

        public int CompareIPAddresses(string a, string b)
        {
            string[] aComponents = a.Split(' ');
            string[] bComponents = b.Split(' ');

            string[] aIpComponents = aComponents[1].Split('.');
            string[] bIpComponents = bComponents[1].Split('.');

            for (int i = 0; i < 4; i++)
            {
                int aComponent = int.Parse(aIpComponents[i]);
                int bComponent = int.Parse(bIpComponents[i]);

                if (aComponent != bComponent)
                {
                    return aComponent.CompareTo(bComponent);
                }
            }

            return 0;
        }
    }
}

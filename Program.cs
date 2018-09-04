using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace calculadora_ip
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator("192.168.0.1", 24);
            Console.WriteLine("A rede possui um número máximo de {0} hosts.", calc.CalcHosts());
            Console.WriteLine("A rede pode ter um número máximo de {0} subredes.", calc.CalcSubnet());
            string sameNetworkResponse = calc.SameNetwork("192.168.0.255", 16) ? "Os hosts pertencem à mesma rede" : "Os hosts não pertecem à mesma rede";
            Console.WriteLine(sameNetworkResponse);
            Console.ReadKey();
        }

        class Calculator
        {
            private string ip { get; set; }
            private string[] binaryIp { get; set; }

            private int netmask { get; set; }
            private string[] binaryMask { get; set; }

            public Calculator(string ip, int netmask)
            {
                this.ip = ip;
                binaryIp = parseIp(ip);
                this.netmask = netmask;
                binaryMask = parseMask(netmask);
            }

            public string[] parseIp(string ip)
            {
                string parsedIp = "";
                string[] octetIp = ip.Split('.');

                for(int i=0; i < octetIp.Length; i++)
                {
                    parsedIp += Convert.ToString(int.Parse(octetIp[i]), 2).PadRight(8, '0');
                    if (i < octetIp.Length - 1) parsedIp += ".";
                }
                return parsedIp.Split('.');
            }

            public string[] parseMask(int mask)
            {
                string parsedMask = "";
                for(int i=0; i < mask; i++)
                {
                    parsedMask += "1";
                }
                parsedMask = parsedMask.PadRight(32, '0');
                parsedMask = Regex.Replace(parsedMask, ".{8}", "$0.");
                parsedMask = parsedMask.Remove(parsedMask.Length - 1, 1);
                return parsedMask.Split('.');
            }

            public int CalcHosts()
            {
                return Convert.ToInt32(Math.Pow(2, 32 - netmask)) - 2;
            }

            public int CalcSubnet()
            {
                return Convert.ToInt32(Math.Pow(2, netmask)) / 4;
            }

            public bool SameNetwork(string ipToCompare, int maskToCompare)
            {
                string networkId1 = "";
                string networkId2 = "";
                string[] binaryIp2 = parseIp(ipToCompare);
                string[] binaryMask2 = parseMask(maskToCompare);


                for (int i=0; i<binaryIp.Length; i++)
                {
                    networkId1 += (Convert.ToInt32(binaryIp[i], 2) & Convert.ToInt32(binaryMask[i], 2)).ToString();
                    networkId2 += (Convert.ToInt32(binaryIp2[i], 2) & Convert.ToInt32(binaryMask2[i], 2)).ToString();
                    if (i < binaryIp.Length - 1)
                    {
                        networkId1 += '.';
                        networkId2 += '.';
                    }
                }
                int shorterMask = netmask < maskToCompare ? netmask : maskToCompare;
                return string.Join("", binaryIp).Substring(0, shorterMask) == string.Join("", binaryIp2).Substring(0, shorterMask) ? true : false;
            }

            private string ToBinary(string entry)
            {
                return Convert.ToString(int.Parse(entry), 2);
            }

            private List<string> CheckClass(int ip)
            {
                List<string> ipClass = new List<string>();

                if (ip >= 1 && ip <= 126)
                {
                    ipClass.Add("A");
                    ipClass.Add("8");
                } else
                if (ip >= 128 && ip <= 191)
                {
                    ipClass.Add("B");
                    ipClass.Add("16");
                } else
                if (ip >= 192 && ip <= 223)
                {
                    ipClass.Add("C");
                    ipClass.Add("24");
                }
                return ipClass;
            }

            static string IPV4ToBin(string input)
            {
                return String.Join(".", (input.Split('.').Select(x => Convert.ToString(Int32.Parse(x), 2).PadLeft(8, '0'))).ToArray());
            }
            
        }
    }
}

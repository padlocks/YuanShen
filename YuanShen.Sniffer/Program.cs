using PacketDotNet;
using SharpPcap;
using System;
using System.IO;
using System.Threading;
using YuanShen.Core.Data;

namespace YuanShen.Sniffer
{

    class Program
    {
        private static readonly String UnixTime = DateTime.Now.GetUnixTime().ToString();
        private static int packetCount = 0;
        static void Main(string[] args)
        {
            Console.WriteLine($"YuanShen_Sniffer using SharpPcap {SharpPcap.Version.VersionString}");

            var devices = CaptureDeviceList.Instance;

            if (devices.Count < 1)
            {
                Console.WriteLine("No devices were found on this machine");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("The following devices are available on this machine:");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine();

            var i = 0;

            foreach (var dev in devices)
            {
                Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Description);
                i++;
            }

            Console.WriteLine();
            Console.Write("-- Please choose a device to capture: ");
            i = Int32.Parse(Console.ReadLine() ?? throw new InvalidOperationException());

            var device = devices[i];

            var readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            device.Filter = "udp portrange 22100-22102";

            try
            {
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dumps", UnixTime)))
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dumps", UnixTime));
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }

            device.OnPacketArrival += Device_OnPacketArrival;
            device.StartCapture();

            Console.WriteLine();
            Console.WriteLine($"-- Listening on {device.Name}, hit 'ctrl-c' to stop...");

            while (true)
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key == ConsoleKey.C && keyInfo.Modifiers == ConsoleModifiers.Control)
                {
                    Thread.Sleep(500);
                    break;
                }
            }

            Console.WriteLine("-- Capture stopped");

            Console.WriteLine(device.Statistics.ToString());

            device.Close();

            Console.ReadLine();
        }

        private static void Device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            try
            {
                var rawPacket = Packet.ParsePacket(LinkLayers.Ethernet, e.Packet.Data).Extract<EthernetPacket>();
                var ipPacket = Packet.ParsePacket(LinkLayers.Ethernet, e.Packet.Data).Extract<IPv4Packet>();
                var udpPacket = ipPacket.Extract<UdpPacket>();

                EPacketDirection direction = EPacketDirection.In;

                if (udpPacket.DestinationPort >= 22100 && udpPacket.DestinationPort <= 22102)
                    direction = EPacketDirection.Out;

                Console.WriteLine($"New {direction}_{packetCount.ToString("D5")} packet with {udpPacket.PayloadData.Length} bytes");

                File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dumps", UnixTime, $"{packetCount.ToString("D5")}_{direction}.bin"), udpPacket.PayloadData);

                packetCount++;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
// See https://aka.ms/new-console-template for more information
using ConsoleApp1;

using TL;

//await new Telegram().Start();
var aa = await new BaseScan().GetInfoByAddress("0x4A8b69837106FCFABb1824EE8Bb5219478b16A47");
var ee = await new CryptoFilter().Time(aa.result, "0x4A8b69837106FCFABb1824EE8Bb5219478b16A47");

Console.WriteLine("Hello, World!");



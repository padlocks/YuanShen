using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YuanShen.Core.Data;

namespace YuanShen.Core.Net
{
    public class GenshinPacket
    {
		public uint ConversationId { get; set; }
		public byte Command { get; set; }
		public byte FragmentCount { get; set; }
		public ushort WindowSize { get; set; }
		public uint TimeStamp { get; set; }
		public uint SerialNumber { get; set; }
		public uint UnAckSerialNumber { get; set; }
		public byte[] Data { get; set; }

		public GenshinPacket() { }

		public GenshinPacket(Stream buffer)
		{
			BinaryReader reader = new BinaryReader(buffer);

			reader.ReadBytes(4);
			ConversationId = reader.ReadUInt32();
			Command = reader.ReadByte();
			FragmentCount = reader.ReadByte();
			WindowSize = reader.ReadUInt16();
			TimeStamp = reader.ReadUInt32();
			SerialNumber = reader.ReadUInt32();
			UnAckSerialNumber = reader.ReadUInt32();
			Data = reader.ReadBytes(reader.ReadInt32());
		}

		public string Dump()
		{
			return	$"ConversationId : {ConversationId} " +
					$"Command : {Command} " +
					$"FragmentCount : {FragmentCount} " +
					$"WindowSize : {WindowSize} " +
					$"TimeStamp : {TimeStamp} " +
					$"SerialNumber : {SerialNumber} " +
					$"UnAckSerialNumber : {UnAckSerialNumber} " +
					$"\n\n" +
					Data.HexDump(16);
		}
	}
}

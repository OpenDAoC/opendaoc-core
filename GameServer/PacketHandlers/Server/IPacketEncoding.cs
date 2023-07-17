using System;

namespace DOL.GS.PacketHandler
{
	public enum EEncryptionState
	{
		NotEncrypted = 0,
		RSAEncrypted = 1,
		PseudoRC4Encrypted = 2
	}

	public interface IPacketEncoding
	{
		EEncryptionState EncryptionState { get; set; }
		byte[] DecryptPacket(byte[] content, int offset, bool udpPacket);
		byte[] EncryptPacket(byte[] content, int offset, bool udpPacket);
		byte[] SBox { get; set; }
	}
}

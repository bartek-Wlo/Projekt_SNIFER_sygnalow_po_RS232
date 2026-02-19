namespace GUIdoObslugiSnifferAnd;

public partial class Form1
{
    //  CRC-16/MCRF4XX 
    public ushort CalculateCRC16MCRF4XX(byte[] data)
    {
        ushort crc = 0xFFFF;
        for (int i = 0; i < data.Length; i++)
        {
            crc ^= data[i];
            for (int j = 0; j < 8; j++)
            {
                if ((crc & 0x0001) != 0)
                {
                    crc = (ushort)((crc >> 1) ^ 0x8408);
                }
                else
                {
                    crc >>= 1;
                }
            }
        }
        return crc;
    }
}
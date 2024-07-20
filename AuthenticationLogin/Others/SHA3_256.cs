using Org.BouncyCastle.Crypto.Digests;
using System.Text;

namespace AuthenticationLogin.Others;

public class SHA3_256
{
    public static string ComputeSha3_256Hash(byte[] data)
    {
        var sha3 = new Sha3Digest(256);

        sha3.BlockUpdate(data, 0, data.Length);
        byte[] result = new byte[sha3.GetDigestSize()];

        sha3.DoFinal(result, 0);

        StringBuilder sb = new(result.Length * 2);

        foreach (byte b in result)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}

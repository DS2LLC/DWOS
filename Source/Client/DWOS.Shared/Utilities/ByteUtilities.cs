namespace DWOS.Shared.Utilities
{
    public static class ByteUtilities
    {
        public static bool AreEqual(byte[] a, byte[] b)
        {
            if (a == b)
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            for (var i = 0; i < a.Length; i++)
            {
                var aItem = a[i];
                var bItem = b[i];

                if (aItem != bItem)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

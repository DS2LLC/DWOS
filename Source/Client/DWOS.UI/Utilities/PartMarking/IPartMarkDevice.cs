using System;

namespace DWOS.UI.Utilities.PartMarking
{
    public interface IPartMarkDevice
    {
        Action<string, bool> ReceivedMessages { get; set; }
        Action<string> SentMessages { get; set; }
        PartMarkSettings Settings { get; }

        /// <summary>
        ///     Opens the connection to the part marker.
        /// </summary>
        /// <returns> </returns>
        void Open();
        /// <summary>
        ///     Closes the connection to the part marker.
        /// </summary>
        void Close();
        void ClearBuffer();
        void SetFont(int lineCount, MultiStroke stroke, PartMarkFontSize fontSize);
        void SetCustomFont(string text);
        void WriteText(params string[] lines);
    }
}

namespace DWOS.UI.Utilities.PartMarking
{
    public class VideoJetPartMarkDevice : VideoJetDeviceBase
    {
        #region Methods

        public VideoJetPartMarkDevice(PartMarkSettings settings) : base(settings)
        {
        }

        public override void WriteText(params string[] lines)
        {
            int lineCount = lines.Length;

            int index = 0;
            string sb = "";

            while (true)
            {
                if (lineCount >= 1)
                {
                    if (lines[0].Length > index)
                        sb += lines[0][index];
                    else
                        sb += HexConverter.ConvertCode(BLANK_SPACE);
                }

                if (lineCount >= 2)
                {
                    if (lines[1].Length > index)
                        sb += lines[1][index];
                    else
                        sb += HexConverter.ConvertCode(BLANK_SPACE);
                }

                if (lineCount >= 3)
                {
                    if (lines[2].Length > index)
                        sb += lines[2][index];
                    else
                        sb += HexConverter.ConvertCode(BLANK_SPACE);
                }

                if (lineCount >= 4)
                {
                    if (lines[3].Length > index)
                        sb += lines[3][index];
                    else
                        sb += HexConverter.ConvertCode(BLANK_SPACE);
                }

                index++;

                bool moreToPrint = false;

                foreach (string line in lines)
                {
                    if (line.Length > index)
                        moreToPrint = true;
                }

                if (!moreToPrint)
                    break;
            }
            
            SendMessage("Send Text", sb + HexConverter.ConvertCode(END_OF_LINE), false);
        }

        #endregion
    }
}

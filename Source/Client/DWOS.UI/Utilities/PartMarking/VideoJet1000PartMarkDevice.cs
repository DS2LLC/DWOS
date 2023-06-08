namespace DWOS.UI.Utilities.PartMarking
{
    public class VideoJet1000PartMarkDevice : VideoJetDeviceBase
    {
        #region Methods

        public VideoJet1000PartMarkDevice(PartMarkSettings settings) : base(settings)
        {
        }

        public override void WriteText(params string[] lines)
        {
            SendMessage("Send Text", string.Join("\t", lines) + HexConverter.ConvertCode(END_OF_LINE), false);
        }

        #endregion
    }
}

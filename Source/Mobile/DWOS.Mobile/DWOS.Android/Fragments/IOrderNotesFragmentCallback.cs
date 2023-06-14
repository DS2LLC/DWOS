using DWOS.Services.Messages;

namespace DWOS.Android
{
    public interface IOrderNotesFragmentCallback
    {
        void OnNoteSelected(OrderNote note);
    }
}
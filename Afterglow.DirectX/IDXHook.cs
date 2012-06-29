using Afterglow.DirectX.ScreenshotInterface;

namespace Afterglow.DirectX
{
    internal interface IDXHook
    {
        ScreenshotInterface.ScreenshotInterface Interface
        {
            get;
            set;
        }

        bool ShowOverlay
        {
            get;
            set;
        }

        ScreenshotRequest Request
        {
            get;
            set;
        }

        void Hook();

        void Cleanup();
    }
}

using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace d2runner
{
    // Dummy IKeyboardMouseEvents to reproduce breakpoing slowdown when hooking input events
    class DummyKeyboardMouseEvents : IKeyboardMouseEvents
    {
        public event System.Windows.Forms.KeyEventHandler KeyDown;
        public event KeyPressEventHandler KeyPress;
        public event EventHandler<KeyDownTxtEventArgs> KeyDownTxt;
        public event System.Windows.Forms.KeyEventHandler KeyUp;
        public event System.Windows.Forms.MouseEventHandler MouseMove;
        public event EventHandler<MouseEventExtArgs> MouseMoveExt;
        public event System.Windows.Forms.MouseEventHandler MouseClick;
        public event System.Windows.Forms.MouseEventHandler MouseDown;
        public event EventHandler<MouseEventExtArgs> MouseDownExt;
        public event System.Windows.Forms.MouseEventHandler MouseUp;
        public event EventHandler<MouseEventExtArgs> MouseUpExt;
        public event System.Windows.Forms.MouseEventHandler MouseWheel;
        public event System.Windows.Forms.MouseEventHandler MouseHWheel;
        public event EventHandler<MouseEventExtArgs> MouseWheelExt;
        public event EventHandler<MouseEventExtArgs> MouseHWheelExt;
        public event System.Windows.Forms.MouseEventHandler MouseDoubleClick;
        public event System.Windows.Forms.MouseEventHandler MouseDragStarted;
        public event EventHandler<MouseEventExtArgs> MouseDragStartedExt;
        public event System.Windows.Forms.MouseEventHandler MouseDragFinished;
        public event EventHandler<MouseEventExtArgs> MouseDragFinishedExt;

        public void Dispose()
        {
        }
    }
}

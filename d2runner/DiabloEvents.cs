using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;

namespace d2runner
{
    class Foo : IKeyboardMouseEvents
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


    public static class DiabloEvents
    {
        private static IKeyboardMouseEvents evts;
        public static IObservable<DateTimeOffset> HellGameStarted { get; private set; }
        public static IObservable<DateTimeOffset> RunEnded { get; private set; }

        private static bool IsPlayButton(Point p) => p.X > 863 && p.X < 1270 && p.Y > 1241 && p.Y < 1340;
        private static bool IsHellButton(Point p) => p.X > 1122 && p.X < 1436 && p.Y > 741 && p.Y < 820;
        private static bool IsSaveExitButton(Point p) => p.X > 1059 && p.X < 1503 && p.Y > 593 && p.Y < 676;

        static DiabloEvents()
        {
            evts = Hook.GlobalEvents();
            //evts = new Foo();

            // start a run
            HellGameStarted = evts.GetClickPosition().Buffer(2, 1).Where(x => IsPlayButton(x[0]) && IsHellButton(x[1])).Select(x => DateTimeOffset.Now);

            int escCount = 0;
            bool listening = false;
            evts.GetKeys().Where(key => key == Key.Escape).Subscribe(x =>
            {
                escCount++;
                listening = true;
                int ec = escCount;
                Observable.Timer(TimeSpan.FromSeconds(2)).Where(a => ec == escCount).Subscribe(x => listening = false);
            });
            RunEnded = evts.GetClickPosition().Where(x => listening && IsSaveExitButton(x)).Select(x => DateTimeOffset.Now);
        }
    }
}

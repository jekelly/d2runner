using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;

namespace d2runner
{
    public static class DiabloEvents
    {
        private static IKeyboardMouseEvents evts;
        public static IObservable<DateTimeOffset> HellGameStarted { get; private set; }
        public static IObservable<DateTimeOffset> RunEnded { get; private set; }
        public static IObservable<DateTimeOffset> AbandonLastRun { get; private set; }

        private static bool IsPlayButton(Point p) => p.X > 863 && p.X < 1270 && p.Y > 1241 && p.Y < 1340;
        private static bool IsHellButton(Point p) => p.X > 1122 && p.X < 1436 && p.Y > 741 && p.Y < 820;
        private static bool IsSaveExitButton(Point p) => p.X > 1059 && p.X < 1503 && p.Y > 593 && p.Y < 676;

        static DiabloEvents()
        {
            evts = Hook.GlobalEvents();
            //evts = new DummyKeyboardMouseEvents();

            // start a run
            var clickStart = evts
                .GetClickPosition()
                .Buffer(2, 1)
                .Do(b => Debug.WriteLine($"Click: {b[1].X}, {b[1].Y}"))
                .Where(x => IsPlayButton(x[0]) && IsHellButton(x[1]))
                .Select(x => DateTimeOffset.Now);
            var keyStart = evts
                .GetKeys()
                .Do(b => Debug.WriteLine($"Keys pressed: {b.Modifiers}+{b.KeyCode}"))
                .Where(k => k.Modifiers == System.Windows.Forms.Keys.Alt && k.KeyCode == System.Windows.Forms.Keys.R)
                .Select(x => DateTimeOffset.Now);
            HellGameStarted = clickStart.Merge(keyStart);

            int escCount = 0;
            bool listening = false;
            evts.GetKeys().Where(key => key.KeyCode == Keys.Escape).Subscribe(x =>
            {
                escCount++;
                listening = true;
                int ec = escCount;
                Observable.Timer(TimeSpan.FromSeconds(2)).Where(a => ec == escCount).Subscribe(x => listening = false);
            });
            var clickEnd = evts.GetClickPosition().Where(x => listening && IsSaveExitButton(x)).Select(x => DateTimeOffset.Now);
            var keyEnd = evts.
                GetKeys()
                .Do(b => Debug.WriteLine($"Keys pressed: {b.Modifiers}+{b.KeyCode}"))
                .Where(k => k.Modifiers == System.Windows.Forms.Keys.Alt && k.KeyCode == System.Windows.Forms.Keys.Q)
                .Select(x => DateTimeOffset.Now);
            RunEnded = clickEnd.Merge(keyEnd);

            var keyAbandon = evts.
                GetKeys()
                .Do(b => Debug.WriteLine($"Keys pressed: {b.Modifiers}+{b.KeyCode}"))
                .Where(k => k.Modifiers == System.Windows.Forms.Keys.Alt && k.KeyCode == System.Windows.Forms.Keys.A)
                .Select(x => DateTimeOffset.Now);
            AbandonLastRun = keyAbandon;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Gma.System.MouseKeyHook;
using ReactiveUI;
using Splat;

namespace d2runner
{
    public static class KeyboardMouseEventsExtensions
    {
        public static IObservable<Point> GetClickPosition(this IMouseEvents evt)
        {
            return Observable.FromEventPattern(evt, nameof(evt.MouseDownExt))
                    .Select(oe =>
                    {
                        var x = (MouseEventExtArgs)oe.EventArgs;
                        return new Point(x.Point.X, x.Point.Y);
                    })
                    .DistinctUntilChanged();
        }

        public static IObservable<KeyEventArgsExt> GetKeys(this IKeyboardEvents evt)
        {
            return Observable.FromEventPattern(evt, nameof(evt.KeyUp))
                .Select(oe => oe.EventArgs)
                .Cast<KeyEventArgsExt>();
                //.Select(e => new KeyEventArgs(
                //.Select(e => KeyInterop.KeyFromVirtualKey((int)e.KeyCode));
        }
    }

    public record class Run
    {
        public Run(int id)
        {
            this.Id = id;
            this.Start = DateTimeOffset.Now;
        }

        public int Id { get; }
        public DateTimeOffset Start { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? End { get; set; }
        public bool IsRunning => this.End is null;

        public override string ToString()
        {
            return $"Run: {this.Id}";
            //return $"Run {this.Id}: {((this.End ?? DateTimeOffset.Now) - this.Start).TotalSeconds} seconds {(this.End is null ? "(still running)" : "")}";
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<MainWindowViewModel>));
        }
    }
}

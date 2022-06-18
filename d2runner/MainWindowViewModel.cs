using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;

namespace d2runner
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly RunTracker runTracker = new RunTracker();

        private ObservableAsPropertyHelper<TimeSpan> currentRunElapsed;
        public TimeSpan CurrentRunElapsed => this.currentRunElapsed.Value;

        private readonly ObservableAsPropertyHelper<Run> currentRun;
        public Run CurrentRun => this.currentRun.Value;

        public ReactiveCommand<Unit, Unit> AbandonRunCommand { get; }

        public MainWindowViewModel()
        {
            var start = DateTimeOffset.Now;

            var tick = Observable.Interval(TimeSpan.FromMilliseconds(100));

            var o = tick.CombineLatest(this.runTracker.RunUpdate)
                .Select(x => (x.Second.End ?? DateTimeOffset.Now) - x.Second.Start)
                .ObserveOnDispatcher()
                .ToProperty(this, x => x.CurrentRunElapsed, out this.currentRunElapsed);

            this.runTracker.RunUpdate.ObserveOnDispatcher().ToProperty(this, x => x.CurrentRun, out this.currentRun);

            this.AbandonRunCommand = ReactiveCommand.Create(() => this.runTracker.AbandonRun());
        }
    }
}

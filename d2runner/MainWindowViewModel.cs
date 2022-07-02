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

        public ReactiveCommand<Unit, Unit> StartStopCommand { get; }
        public ReactiveCommand<Unit, Unit> AbandonRunCommand { get; }

        private readonly ObservableAsPropertyHelper<string> playPauseIcon;
        public string PlayPauseIcon => this.playPauseIcon.Value;

        public MainWindowViewModel()
        {
            var start = DateTimeOffset.Now;

            var tick = Observable.Interval(TimeSpan.FromMilliseconds(100));

            var o = tick.CombineLatest(this.runTracker.RunUpdate)
                .Where(x => x.Second is not null)
                .Select(x => (x.Second.End ?? DateTimeOffset.Now) - x.Second.Start)
                .ObserveOnDispatcher()
                .ToProperty(this, x => x.CurrentRunElapsed, out this.currentRunElapsed);

            var runUpdate = this.runTracker.RunUpdate.ObserveOnDispatcher();
            runUpdate.Select(x => IsRunActive(x) ? char.ConvertFromUtf32(0xe20d) : char.ConvertFromUtf32(0xe102)).ToProperty(this, x => x.PlayPauseIcon, out this.playPauseIcon);
            runUpdate.ToProperty(this, x => x.CurrentRun, out this.currentRun);
            //runUpdate.Select(x => x is null ? char. "&#xe103;" : "&#xe102;").ToProperty(this, x => x.PlayPauseIcon, out this.playPauseIcon);

            this.StartStopCommand = ReactiveCommand.Create(this.StartOrStop);
            this.AbandonRunCommand = ReactiveCommand.Create(() => this.runTracker.AbandonRun());
            DiabloEvents.AbandonLastRun.Select(dto => Unit.Default).InvokeCommand(this.AbandonRunCommand);
        }

        private static bool IsRunActive(Run run) => run?.IsRunning ?? false;

        private void StartOrStop()
        {
            if (IsRunActive(this.CurrentRun))
            {
                this.runTracker.StopRun();
            }
            else
            {
                this.runTracker.StartRun();
            }
        }
    }
}

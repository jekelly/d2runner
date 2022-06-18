using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace d2runner
{
    public class RunRepository
    {
        private readonly List<Run> runs;

        public RunRepository()
        {
            this.runs = new List<Run>();
        }

        public Run? LastRun { get => this.runs.LastOrDefault(); }

        public void Add(Run run)
        {
            this.runs.Add(run);
        }
    }

    public class RunTracker
    {
        private readonly RunRepository runs = new RunRepository();
        private readonly Subject<Run> runSubject;
        private Run? activeRun;
        private int idCounter = 0;

        public IObservable<Run> RunUpdate => this.runSubject;

        public RunTracker()
        {
            DiabloEvents.HellGameStarted.Subscribe(this.StartRun);
            DiabloEvents.RunEnded.Subscribe(this.EndRun);
            this.runSubject = new Subject<Run>();
        }

        private void StartRun(DateTimeOffset startTime)
        {
            if (this.activeRun is not null && this.activeRun.End is null)
            {
                this.EndRun(DateTimeOffset.Now); // should we abandon the run in this case?
            }

            this.activeRun = new Run(this.idCounter++);
            this.runSubject.OnNext(this.activeRun);
        }

        public void AbandonRun()
        {
            if (this.activeRun is null)
                return;

            this.activeRun = null;
            this.runSubject.OnNext(this.runs.LastRun);
            this.idCounter--;
        }

        private void EndRun(DateTimeOffset endTime)
        {
            if (this.activeRun is null)
                return;

            this.activeRun.End = endTime;
            this.runSubject.OnNext(this.activeRun);
            this.runs.Add(this.activeRun);
        }
    }
}

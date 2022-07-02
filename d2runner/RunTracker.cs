using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;

namespace d2runner
{
    public class RunRepository
    {
        private readonly SourceCache<Run, int> runCache;

        public IObservableCache<Run, int> Runs { get; }

        public RunRepository()
        {
            this.runCache = new SourceCache<Run, int>(r => r.Id);
            this.Runs = this.runCache.AsObservableCache();

            this.runCache
                .Connect()
                .Select(cs =>
                {
                    var id = this.runCache.Keys.Any() ? this.runCache.Keys.Max() : -1;
                    return id >= 0 ? this.runCache.Lookup(id).Value : null;
                })
                .Subscribe(maxRun => this.LastRun = maxRun);
        }

        public Run? LastRun { get; protected set; }

        public virtual void Add(Run run)
        {
            this.runCache.AddOrUpdate(run);
            this.LastRun = run;
        }

        public virtual void Remove(Run run)
        {
            this.runCache.Remove(run);
        }

        public virtual int NextId() => 0;
    }

    public class RunTracker
    {
        private readonly RunRepository runs = new SqlRunRepository();
        private readonly BehaviorSubject<Run> runSubject;
        private Run? activeRun;
        private int idCounter;

        public IObservable<Run> RunUpdate => this.runSubject;

        public RunTracker()
        {
            DiabloEvents.HellGameStarted.Subscribe(this.StartRun);
            DiabloEvents.RunEnded.Subscribe(this.EndRun);
            this.runSubject = new BehaviorSubject<Run>(this.runs.LastRun);
            this.idCounter = this.runs.NextId();
        }

        public void StartRun() => this.StartRun(DateTimeOffset.Now);
        public void StopRun() => this.EndRun(DateTimeOffset.Now);

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
                this.activeRun = this.runs.LastRun;

            this.runs.Remove(this.activeRun);
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

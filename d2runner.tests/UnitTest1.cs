using DynamicData.Kernel;

namespace d2runner.tests
{
    public class RunRepositoryTests
    {
        private readonly RunRepository sut = new RunRepository();
        private static readonly Random r = new Random();

        [Fact]
        public void NewRepository_IsEmpty()
        {
            this.sut.Runs.KeyValues.Should().BeEmpty();
        }

        [Fact]
        public void NewRun_IsInCache()
        {
            var test = Test();
            this.sut.Add(test);
            this.sut.Runs.Lookup(test.Id).Should().NotBe(Optional<Run>.None);
        }

        [Fact]
        public void AddAndRemoveRun_LeavesCacheEmpty()
        {
            var test = Test();

            this.sut.Add(test);
            this.sut.Remove(test);
            this.sut.Runs.Lookup(test.Id).Should().Be(Optional<Run>.None);
        }

        private static Run Test()
        {
            return new Run(r.Next());
        }
    }
}
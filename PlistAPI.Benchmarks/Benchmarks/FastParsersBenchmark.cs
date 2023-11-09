using BenchmarkDotNet.Attributes;

namespace PlistAPI.Benchmarks.Benchmarks
{
    [MemoryDiagnoser]
    public class FastParsersBenchmark
    {
        public int IntValues = int.MaxValue;

        public float FlaotValues = int.MinValue;

        [Benchmark]
        public void DefaultIntParser()
            // slow int parser
            => int.Parse(IntValues.ToString());

        [Benchmark]
        public void FastIntParser()
            // fast int parser
            => FastParsers.ToInt(IntValues.ToString());

        [Benchmark]
        public void DefaultFloatParser()
            // slow float parser
            => float.Parse(FlaotValues.ToString());

        [Benchmark]
        public void FastFloatParser()
            // fast float parser
            => FastParsers.ToFloat(FlaotValues.ToString());
    }
}

using System.Threading;

namespace OpenFTTH.TileDataExtractor
{
    public class Counter
    {
        private long _counter = 0;

        public long GetNext()
        {
            return Interlocked.Increment(ref _counter);
        }
    }
}

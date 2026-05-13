namespace NMWPFControls.Core
{
    public class RealtimeAverage
    {
        private readonly double[] _vals;
        private readonly int _sz;
        private int _i = 0;

        public double Sum { get; private set; }
        public double Mean { get; private set; }

        public RealtimeAverage(int size)
        {
            _sz = size;
            _vals = new double[size];
            for (int i = 0; i < _sz; i++) { _vals[i] = 0; }
        }

        public double AddVal(double val)
        {
            Sum = Sum - _vals[_i] + val;
            _vals[_i] = val;
            _i = (_i + 1) % _sz;

            Mean = Sum / _sz;
            return Mean;
        }
    }
}

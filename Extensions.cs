using System.Numerics;

namespace adventOfCode;

public static class Extensions
{
    extension<T>(IEnumerable<T> array) where T : INumber<T>
    {
        public T Mul()
        {
            return array.Aggregate(T.MultiplicativeIdentity, (current, t) => current * t);
        }
    }
}
using System.Collections.Frozen;
using Xunit;
using PriceChange = (int D1, int D2, int D3, int D4);

namespace adventOfCode._2024.Day22;

public class Day22
{
    [Fact]
    public void First()
    {
        var buyerSecrets = InputHelper.GetInputLines().Select(long.Parse).ToArray();
        for (var i = 0; i < 2000; i++)
        {
            for (var j = 0; j < buyerSecrets.Length; j++)
            {
                buyerSecrets[j] = CalculateSecret(buyerSecrets[j]);
            }
        }

        Assert.Equal(13004408787, buyerSecrets.Sum());
    }
    
    [Fact]
    public void Second()
    {
        var buyerSecrets = InputHelper.GetInputLines().Select(long.Parse).ToArray();
        var buyersPriceChanges = new FrozenDictionary<PriceChange, int>[buyerSecrets.Length];

        Span<int> prices = new int[2000];

        foreach (var (index, buyerSecret) in buyerSecrets.Index())
        {
            prices[0] = LastDigit(buyerSecret);
            var previous = buyerSecret;
            for (var i = 1; i < prices.Length; i++)
            {
                var secret = CalculateSecret(previous);
                var price = LastDigit(secret);

                (previous, prices[i]) = (secret, price);
            }

            buyersPriceChanges[index] = CalculatePriceChanges(prices);
        }

        var alreadyTriedPriceChanges = new HashSet<PriceChange>();
        var finalResult = 0;
        for (var i = 0; i < buyerSecrets.Length; i++)
        {
            var biggestForPriceChange = 0;
            foreach (var priceChange in buyersPriceChanges[i])
            {
                if (!alreadyTriedPriceChanges.Add(priceChange.Key)) continue;

                var result = buyersPriceChanges.AsParallel().Select(x => x.GetValueOrDefault(priceChange.Key, 0)).Sum();

                if (biggestForPriceChange < result) biggestForPriceChange = result;
            }

            if (finalResult < biggestForPriceChange)
            {
                finalResult = biggestForPriceChange;
            }
        }

        Assert.Equal(1455, finalResult);
    }
    
    private static FrozenDictionary<PriceChange, int> CalculatePriceChanges(Span<int> prices)
    {
        Dictionary<PriceChange, int> result = [];
        for (var i = 4; i < prices.Length; i++)
        {
            var (s4, s3, s2, s1, s0) = (prices[i], prices[i - 1], prices[i - 2], prices[i - 3], prices[i - 4]);
            var (d4, d3, d2, d1) = (s4 - s3, s3 - s2, s2 - s1, s1 - s0);

            result.TryAdd(new PriceChange(d1, d2, d3, d4), s4);
        }
        
        return result.ToFrozenDictionary();
    }

    private static int LastDigit(long value) => (int)(value % 10);
    private static long CalculateSecret(long s)
    {
        // 16777216 = 2^24 => 2^24 - 1 = 0xFFFFFF
        // n % 2^m = n & 2^m - 1
        s = (s ^ (s << 6)) & 0xFFFFFF;
        s = (s ^ (s >> 5)) & 0xFFFFFF;
        return (s ^ (s << 11)) & 0xFFFFFF;
    }
}
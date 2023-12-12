namespace SalaTools.Core;

using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

public interface IMemoizable
{
    void Clear();
    void Clear<T>();
}

/// <summary>
/// A stupid yet simple and effective memoization (caching) for small number of items. It uses generic
/// arguments to build a key and caches the generic result. 
/// 
/// Below is an example on how to use this on a class that returns arbitrary parameters. It derives from a
/// non-memoizing base class and overrides it(or extends, whichever way you like) with memoizing.
/// <code>
/// public class ParametersMemoizing : Parameters
/// {
///     private readonly ArgumentBasedMemoize _mem = new();
/// 
///     public ParametersMemoizing(Session session) : base(session) { }
/// 
///     public override Parameter GetParameter(string code) =>
///         _mem.GetMemoized(code, base.GetParameter);
/// 
///     public override IEnumerable&lt;Parameter&gt; GetParameters(string code, int foo) =>
///         _mem.GetMemoized(code, foo, base.GetParameters);
/// 
///     public void ClearCache() => _mem.Clear();
/// }
/// </code>
/// </summary>
public class ArgumentBasedMemoize : IMemoizable
{
    // Why Lazy<T>: https://andrewlock.net/making-getoradd-on-concurrentdictionary-thread-safe-using-lazy/
    private readonly ConcurrentDictionary<(string, Type, object), Lazy<object>> _mem = new();

    // Simple enough
    public TResult GetMemoized<TResult>(Func<TResult> f, [CallerMemberName] string memberName = null) =>
        (TResult)_mem.GetOrAdd((memberName, typeof(TResult), null), p => new(() => f()))
                     .Value;

    // Ok now we're getting somewhere
    public TResult GetMemoized<TResult, T1>(T1 p1,
                                            Func<T1, TResult> f,
                                            [CallerMemberName] string memberName = null) =>
        (TResult)_mem.GetOrAdd((memberName, typeof(TResult), p1), p => new(() => f((T1)p.Item3)))
                     .Value;

    // Yes, yes...
    public TResult GetMemoized<TResult, T1, T2>(T1 p1, T2 p2, Func<T1, T2, TResult> f, [CallerMemberName] string memberName = null) =>
        (TResult)_mem.GetOrAdd((memberName, typeof(TResult), (p1, p2)),
                         p => new(() =>
                         {
                             var t = (ValueTuple<T1, T2>)p.Item3;
                             return f(t.Item1, t.Item2);
                         }))
                     .Value;

    // Jack Nicholson nod gif
    public TResult GetMemoized<TResult, T1, T2, T3>(T1 p1,
                                                    T2 p2,
                                                    T3 p3,
                                                    Func<T1, T2, T3, TResult> f,
                                                    [CallerMemberName] string memberName = null) =>
        (TResult)_mem.GetOrAdd((memberName, typeof(TResult), (p1, p2, p3)),
                         p => new(() =>
                         {
                             var t = (ValueTuple<T1, T2, T3>)p.Item3;
                             return f(t.Item1, t.Item2, t.Item3);
                         }))
                     .Value;

    public TResult GetMemoized<TResult, T1, T2, T3, T4>(T1 p1,
                                                        T2 p2,
                                                        T3 p3,
                                                        T4 p4,
                                                        Func<T1, T2, T3, T4, TResult> f,
                                                        [CallerMemberName] string memberName = null) =>
        (TResult)_mem.GetOrAdd((memberName, typeof(TResult), (p1, p2, p3, p4)),
                         p => new(() =>
                         {
                             var t = (ValueTuple<T1, T2, T3, T4>)p.Item3;
                             return f(t.Item1, t.Item2, t.Item3, t.Item4);
                         }))
                     .Value;

    public void Clear() => _mem.Clear();

    public void Clear<T>()
    {
        // Remove all memoizations where mentioned type is used
        var remove = _mem.Keys.Where(k => k.Item2 == typeof(T));
        foreach (var key in remove)
            _mem.TryRemove(key, out _);
    }
}
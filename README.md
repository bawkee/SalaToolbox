# SalaTools 🧰
Bojan Sala's dotNet toolbox of various extensions and utilities that can be used in any app, library or service.

[![NuGet](https://img.shields.io/nuget/v/SalaTools.Core)](https://www.nuget.org/packages/SalaTools.Core/1.0.0)

Some of the tools included:

- ArgumentBasedMemoize - Easy memoization of any class without a hassle, extremely lightweight and flexible. Great for small apps and services with limited lifetime.
- ILogging - An interface that makes your life much easier if you like using Microsoft's logging abstractions. I use this everywhere.
- StableArray - Immutable, comparable, interoperable array with computed hashcodes for each of its elements. Useful for caching, indexing, etc. Works pretty much like a tuple.
- String extensions such as CompactWhitespace, IsNullOrWhiteSpace, NullIfEmptyOrWhiteSpace, WildCard filter and such. Something you'd use often for client-facing services.
- Type extensions (no reflection) such as IsNullableType, IsNumericType, IsNullableOrSpecificType, ChangeType, etc.
- Uri extensions allowing you to easily build query parameters.

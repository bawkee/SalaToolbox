namespace SalaTools.Core;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

/// <summary>
/// Looks out of place but these ugly-looking extensions are needed all too often in UI code.
/// </summary>
public static class TypeExtensions
{
    public static bool IsNullableType(this Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public static readonly HashSet<Type> BuiltInConvertibleTypes = new()
    {
        typeof(byte),
        typeof(DBNull),
        typeof(bool),
        typeof(char),
        typeof(DateTime),
        typeof(string),
        typeof(sbyte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(decimal),
        typeof(double),
        typeof(float)
    };

    public static bool IsBuiltInConvertibleType(this Type type) =>
        BuiltInConvertibleTypes.Contains(type) ||
        BuiltInConvertibleTypes.Contains(Nullable.GetUnderlyingType(type));

    public static readonly HashSet<Type> NumericTypes = new()
    {
        typeof(byte),
        typeof(sbyte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(decimal),
        typeof(double),
        typeof(float),
        typeof(BigInteger)
    };

    public static bool IsNumericType(this Type type) =>
        NumericTypes.Contains(type) ||
        NumericTypes.Contains(Nullable.GetUnderlyingType(type));

    private static readonly HashSet<Type> FloatingTypes = new()
    {
        typeof(decimal),
        typeof(double),
        typeof(float)
    };

    public static bool IsFloatingNumber(this Type type) =>
        FloatingTypes.Contains(type) ||
        FloatingTypes.Contains(Nullable.GetUnderlyingType(type));

    public static bool IsNullableOrSpecificType(this Type type, Type specificType) =>
        Nullable.GetUnderlyingType(type) == specificType || type == specificType;

    public static bool IsNullableOrSpecificType<T>(this Type type) =>
        type.IsNullableOrSpecificType(typeof(T));

    /// <summary>
    /// Converts one type to another by combining multiple techniques.
    /// </summary>
    public static T ChangeType<T>(this object value) =>
        (T)value.ChangeType(typeof(T));

    /// <summary>
    /// Converts one type to another by combining multiple techniques.
    /// </summary>
    public static object ChangeType(this object value, Type targetType)
    {
        if (targetType.IsNullableType())
        {
            if (value == null)
                return default; // No-brainer
            targetType = Nullable.GetUnderlyingType(targetType);
        }

        if (value == null)
        {
            if (targetType == typeof(string) || !targetType.IsValueType)
                return null;
            throw new InvalidCastException("Can't convert a null value to a non-nullable type.");
        }

        // If the value is a IConvertible type then we'll go with that. The IConvertible is an all or nothing
        // affair because you have no way of knowing whether something can be converted or not (other than catching
        // exceptions). Although, with standard built-in types at least we know that they can only convert between
        // themselves.
        if (value is IConvertible convertible)
        {
            if (convertible.GetTypeCode() != TypeCode.Object && // This is one of the standard convertible types
                !targetType.IsBuiltInConvertibleType()) // Target type is not one of the standard types neither
            {
                // Using IConvertible here is guaranteed to not work, so we'll try TypeConverter
                if (TypeDescriptor.GetConverter(targetType) is { } converterFrom && converterFrom.IsValid(value))
                    return converterFrom.ConvertFrom(value); // Guaranteed to work
                // Resort to explicit cast.
                return value;
            }

            // Use the standard converter mechanism.
            return Convert.ChangeType(value, targetType);
        }

        // This isn't an IConvertible so try other things
        var sourceType = value.GetType();

        // See if we're on the same thing (nullable and non-nullable are assignable)
        if (targetType == sourceType || sourceType.IsAssignableFrom(targetType))
            return value;

        // Try the TypeConverter
        if (TypeDescriptor.GetConverter(sourceType) is { } converterTo && converterTo.CanConvertTo(targetType))
            return converterTo.ConvertTo(value, targetType);

        // Final resort
        return value;
    }

    /// <summary>
    /// Returns an array with a single item containing the provided <paramref name="obj"/> value or null if the
    /// <paramref name="obj"/> is null.
    /// Used when array is expected but a single value is available.
    /// </summary>
    public static T[] ExpandToArray<T>(this T obj) => obj == null ? null : new[] { obj };

    public static decimal Round(this decimal d, int decimals = 0) =>
        decimal.Round(d, decimals);

    public static decimal? Round(this decimal? d, int decimals = 0) =>
        d.HasValue ? decimal.Round(d.Value, decimals) : null;


}
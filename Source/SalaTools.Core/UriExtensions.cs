namespace SalaTools.Core;

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;

// These utility classes make the absolute disgrace of a .Net API for building uri query
// parameters at least somewhat acceptable. 
// This one deals with UriBuilder class

public class UriBuilderQuery
{
    private readonly UriBuilder _builder;

    public UriBuilderQuery(UriBuilder builder)
    {
        _builder = builder;
        Query = HttpUtility.ParseQueryString(_builder.Query);
    }

    public NameValueCollection Query { get; set; }

    public UriBuilderQuery AddQuery(string name, object value)
    {
        Query[name] = value.ToString();
        return this;
    }

    public UriBuilder BuildQuery()
    {
        _builder.Query = ToString();
        return _builder;
    }

    public override string ToString() => Query.ToString();
}

public static class UriBuilderExtensions
{
    public static UriBuilderQuery AddQuery(this UriBuilder builder, string name, object value) =>
        new UriBuilderQuery(builder).AddQuery(name, value);
}

public class UriQuery
{
    private readonly Uri _uri;

    public UriQuery(Uri uri)
    {
        _uri = uri;
        Query = HttpUtility.ParseQueryString(_uri.IsAbsoluteUri ? _uri.Query : "");
    }

    public NameValueCollection Query { get; set; }

    public UriQuery AddQuery(string name, object value)
    {
        Query[name] = Convert.ToString(value, CultureInfo.InvariantCulture);
        return this;
    }

    public Uri BuildQuery()
    {
        var actualUri = $"{_uri}?{ToString()}";
        return _uri.IsAbsoluteUri ? new Uri(actualUri) : new Uri(actualUri, UriKind.Relative);
    }

    public override string ToString() => Query.ToString();
}

public static class UriExtensions
{
    public static UriQuery AddQuery(this Uri uri, string name, object value) =>
        new UriQuery(uri).AddQuery(name, value);
}
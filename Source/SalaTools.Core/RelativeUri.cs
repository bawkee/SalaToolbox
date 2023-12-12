﻿namespace SalaTools.Core;

using System;

public class RelativeUri : Uri
{
    public RelativeUri(string uriString)
        : base(uriString, UriKind.Relative)
    {
        // Stupid I know but after 3000 uses it begins to make a difference, like,
        // how often do you use an absolute Uri? 
    }
}
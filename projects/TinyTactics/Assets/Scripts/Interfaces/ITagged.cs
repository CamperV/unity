using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface ITagged
{
    List<string> tags { get; set; }
    bool HasTagMatch(params string[] tagsToCheck);
}
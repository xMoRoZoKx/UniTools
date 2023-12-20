using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class UnityDateTime
{
    [SerializeField] private int year = 2023;
    [SerializeField, Range(1, 12)] private int month = 1;
    [SerializeField, Range(1, 31)] private int day = 1;
    [SerializeField, Range(1, 24)] private int hour = 1;
    [SerializeField, Range(0, 59)] private int minute = 0;
    [SerializeField, Range(0, 59)] private int second = 0;

    public DateTimeOffset GetDateTimeOffset() => new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero);
}

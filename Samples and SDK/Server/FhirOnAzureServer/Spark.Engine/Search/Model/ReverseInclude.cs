﻿#region Information

// Solution:  Spark
// Spark.Engine
// File:  ReverseInclude.cs
// 
// Created: 07/12/2017 : 10:35 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:01 PM

#endregion

namespace FhirOnAzure.Engine.Search.Model
{
    using System;
    using System.Text.RegularExpressions;

    public class ReverseInclude
    {
        private static readonly Regex pattern = new Regex(@"(?<resourcetype>[^\.]+)\.(?<searchpath>.*)");

        public string ResourceType { get; set; }
        public string SearchPath { get; set; }

        /// <summary>
        ///     Expected format: ResourceType.searchParameter[.searchParameter]*
        /// </summary>
        /// <param name="reverseInclude"></param>
        /// <returns>
        ///     ReverseInclude instance with ResourceType is everything before the first dot, and SearchPath everything after
        ///     it.
        /// </returns>
        public static ReverseInclude Parse(string reverseInclude)
        {
            //_revinclude should have the following format: ResourceType.searchParameter[.searchParameter]*
            //so we simply split in on the first dot.
            if (reverseInclude == null)
                throw new ArgumentNullException("reverseInclude cannot be null");
            var result = new ReverseInclude();
            var match = pattern.Match(reverseInclude);
            if (match.Groups.Count < 2)
                throw new ArgumentException(string.Format(
                    "reverseInclude '{0}' does not adhere to the format 'ResourceType.searchParameter[.searchParameter]*'",
                    reverseInclude));

            result.ResourceType = match.Groups["resourcetype"].Captures[0].Value;
            result.SearchPath = match.Groups["searchpath"].Captures[0].Value;

            return result;
        }
    }
}
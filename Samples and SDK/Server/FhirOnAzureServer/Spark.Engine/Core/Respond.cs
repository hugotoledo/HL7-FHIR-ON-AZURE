﻿#region Information

// Solution:  Spark
// Spark.Engine
// File:  Respond.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 2:12 PM

#endregion

namespace FhirOnAzure.Engine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Extensions;
    using Hl7.Fhir.Model;

    // This class serves instances of "Response"
    public static class Respond
    {
        public static FhirResponse NotImplemented => WithError(HttpStatusCode.NotImplemented);

        public static FhirResponse Success => new FhirResponse(HttpStatusCode.OK);

        public static FhirResponse WithError(HttpStatusCode code)
        {
            return new FhirResponse(code, Key.Null, null);
        }

        public static FhirResponse WithCode(HttpStatusCode code)
        {
            return new FhirResponse(code, null);
        }

        public static FhirResponse WithCode(int code)
        {
            return new FhirResponse((HttpStatusCode) code, null);
        }

        public static FhirResponse WithError(HttpStatusCode code, string message, params object[] args)
        {
            var outcome = new OperationOutcome();
            outcome.AddError(string.Format(message, args));
            return new FhirResponse(code, outcome);
        }

        public static FhirResponse WithResource(int code, Resource resource)
        {
            return new FhirResponse((HttpStatusCode) code, resource);
        }

        public static FhirResponse WithResource(Resource resource)
        {
            return new FhirResponse(HttpStatusCode.OK, resource);
        }

        public static FhirResponse WithResource(Key key, Resource resource)
        {
            return new FhirResponse(HttpStatusCode.OK, key, resource);
        }

        public static FhirResponse WithResource(HttpStatusCode code, Key key, Resource resource)
        {
            return new FhirResponse(code, key, resource);
        }

        public static FhirResponse WithEntry(HttpStatusCode code, Entry entry)
        {
            return new FhirResponse(code, entry.Key, entry.Resource);
        }

        //public static FhirResponse WithBundle(IEnumerable<Resource> resources)
        //{
        //    Bundle bundle = new Bundle();
        //    bundle.Append(resources);
        //    return new FhirResponse(HttpStatusCode.OK, bundle);
        //}

        public static FhirResponse WithBundle(Bundle bundle)
        {
            return new FhirResponse(HttpStatusCode.OK, bundle);
        }

        public static FhirResponse WithBundle(IEnumerable<Entry> entries, Uri serviceBase)
        {
            var bundle = new Bundle();
            bundle.Append(entries);
            return WithBundle(bundle);
        }

        public static FhirResponse WithMeta(Meta meta)
        {
            var parameters = new Parameters();
            parameters.Add(typeof(Meta).Name, meta);
            return WithResource(parameters);
        }

        public static FhirResponse WithMeta(Entry entry)
        {
            if (entry.Resource != null && entry.Resource.Meta != null)
                return WithMeta(entry.Resource.Meta);
            return WithError(HttpStatusCode.InternalServerError,
                "Could not retrieve meta. Meta was not present on the resource");
        }

        public static FhirResponse WithKey(HttpStatusCode code, IKey key)
        {
            return new FhirResponse(code, key, null);
        }

        public static FhirResponse WithResource(HttpStatusCode code, Entry entry)
        {
            return new FhirResponse(code, entry.Key, entry.Resource);
        }

        public static FhirResponse WithResource(Entry entry)
        {
            return new FhirResponse(HttpStatusCode.OK, entry.Key, entry.Resource);
        }

        public static FhirResponse NotFound(IKey key)
        {
            if (key.VersionId == null)
                return WithError(HttpStatusCode.NotFound, "No {0} resource with id {1} was found.", key.TypeName,
                    key.ResourceId);
            return WithError(HttpStatusCode.NotFound,
                "There is no {0} resource with id {1}, or there is no version {2}", key.TypeName, key.ResourceId,
                key.VersionId);
            // For security reasons (leakage): keep message in sync with Error.NotFound(key)
        }

        public static FhirResponse Gone(Entry entry)
        {
            var message = string.Format(
                "A {0} resource with id {1} existed, but was deleted on {2} (version {3}).",
                entry.Key.TypeName,
                entry.Key.ResourceId,
                entry.When,
                entry.Key.ToRelativeUri());

            return WithError(HttpStatusCode.Gone, message);
        }
    }
}
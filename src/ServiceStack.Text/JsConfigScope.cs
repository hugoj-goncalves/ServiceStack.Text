﻿using System;
using System.Collections.Generic;
using ServiceStack.Text.Json;
using ServiceStack.Text.Jsv;
using ServiceStack.Text.Common;

namespace ServiceStack.Text
{
    public sealed class JsConfigScope : Config, IDisposable
    {
        bool disposed;
        readonly JsConfigScope parent;

        [ThreadStatic]
        private static JsConfigScope head;

        internal JsConfigScope()
        {
            PclExport.Instance.BeginThreadAffinity();

            parent = head;
            head = this;
        }

        internal static JsConfigScope Current => head;

        public static void DisposeCurrent()
        {
            head?.Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                head = parent;

                PclExport.Instance.EndThreadAffinity();
            }
        }
    }
    
    public class Config
    {
        private static Config instance;
        internal static Config Instance => instance ?? (instance = new Config(Defaults));
        internal static bool HasInit = false;

        public static Config AssertNotInit() => HasInit
            ? throw new NotSupportedException("JsConfig can't be mutated after JsConfig.Init(). Use BeginScope() or CreateScope() to use custom config after Init().")
            : Instance;

        public static void Init() => HasInit = true;

        internal static void Reset() => Instance.Populate(Defaults);

        public Config()
        {
            Populate(Instance);
        }

        private Config(Config config)
        {
            if (config != null) //Defaults=null, instance=Defaults
                Populate(config);
        }

        public bool ConvertObjectTypesIntoStringDictionary { get; set; }
        public bool TryToParsePrimitiveTypeValues { get; set; }
        public bool TryToParseNumericType { get; set; }
        public bool TryParseIntoBestFit { get; set; }
        public ParseAsType ParsePrimitiveFloatingPointTypes { get; set; }
        public ParseAsType ParsePrimitiveIntegerTypes { get; set; }
        public bool ExcludeDefaultValues { get; set; }
        public bool IncludeNullValues { get; set; }
        public bool IncludeNullValuesInDictionaries { get; set; }
        public bool IncludeDefaultEnums { get; set; }
        public bool TreatEnumAsInteger { get; set; }
        public bool ExcludeTypeInfo { get; set; }
        public bool IncludeTypeInfo { get; set; }
        public string TypeAttr { get; set; }
        public string DateTimeFormat { get; set; }
        internal string JsonTypeAttrInObject { get; set; }
        internal string JsvTypeAttrInObject { get; set; }
        public Func<Type, string> TypeWriter { get; set; }
        public Func<string, Type> TypeFinder { get; set; }
        public Func<string, object> ParsePrimitiveFn { get; set; }
        public DateHandler DateHandler { get; set; }
        public TimeSpanHandler TimeSpanHandler { get; set; }
        public PropertyConvention PropertyConvention { get; set; }
        public bool EmitCamelCaseNames { get; set; }
        public bool EmitLowercaseUnderscoreNames { get; set; }
        public bool ThrowOnError { get; set; }
        public bool SkipDateTimeConversion { get; set; }
        public bool AlwaysUseUtc { get; set; }
        public bool AssumeUtc { get; set; }
        public bool AppendUtcOffset { get; set; }
        public bool PreferInterfaces { get; set; }
        public bool IncludePublicFields { get; set; }
        public int MaxDepth { get; set; }
        public DeserializationErrorDelegate OnDeserializationError { get; set; }
        public EmptyCtorFactoryDelegate ModelFactory { get; set; }
        public string[] ExcludePropertyReferences { get; set; }
        public HashSet<Type> ExcludeTypes { get; set; }
        public bool EscapeUnicode { get; set; }
        public bool EscapeHtmlChars { get; set; }

        public static Config Defaults => new Config(null) {
            ConvertObjectTypesIntoStringDictionary = false,
            TryToParsePrimitiveTypeValues = false,
            TryToParseNumericType = false,
            TryParseIntoBestFit = false,
            ParsePrimitiveFloatingPointTypes = ParseAsType.Decimal,
            ParsePrimitiveIntegerTypes = ParseAsType.Byte | ParseAsType.SByte | ParseAsType.Int16 | ParseAsType.UInt16 |
                                         ParseAsType.Int32 | ParseAsType.UInt32 | ParseAsType.Int64 | ParseAsType.UInt64,
            ExcludeDefaultValues = false,
            ExcludePropertyReferences = null,
            IncludeNullValues = false,
            IncludeNullValuesInDictionaries = false,
            IncludeDefaultEnums = true,
            TreatEnumAsInteger = false,
            ExcludeTypeInfo = false,
            IncludeTypeInfo = false,
            TypeAttr = JsWriter.TypeAttr,
            DateTimeFormat = null,
            JsonTypeAttrInObject = JsonTypeSerializer.GetTypeAttrInObject(JsWriter.TypeAttr),
            JsvTypeAttrInObject = JsvTypeSerializer.GetTypeAttrInObject(JsWriter.TypeAttr),
            TypeWriter = AssemblyUtils.WriteType,
            TypeFinder = AssemblyUtils.FindType,
            ParsePrimitiveFn = null,
            DateHandler = Text.DateHandler.TimestampOffset,
            TimeSpanHandler = Text.TimeSpanHandler.DurationFormat,
            EmitCamelCaseNames = false,
            EmitLowercaseUnderscoreNames = false,
            PropertyConvention = Text.PropertyConvention.Strict,
            ThrowOnError = Env.StrictMode,
            SkipDateTimeConversion = false,
            AlwaysUseUtc = false,
            AssumeUtc = false,
            AppendUtcOffset = false,
            EscapeUnicode = false,
            EscapeHtmlChars = false,
            PreferInterfaces = false,
            IncludePublicFields = false,
            MaxDepth = 50,
            OnDeserializationError = null,
            ModelFactory = ReflectionExtensions.GetConstructorMethodToCache,
            ExcludeTypes = new HashSet<Type> { typeof(System.IO.Stream) },
        };

        public Config Populate(Config config)
        {
            ConvertObjectTypesIntoStringDictionary = config.ConvertObjectTypesIntoStringDictionary;
            TryToParsePrimitiveTypeValues = config.TryToParsePrimitiveTypeValues;
            TryToParseNumericType = config.TryToParseNumericType;
            TryParseIntoBestFit = config.TryParseIntoBestFit;
            ParsePrimitiveFloatingPointTypes = config.ParsePrimitiveFloatingPointTypes;
            ParsePrimitiveIntegerTypes = config.ParsePrimitiveIntegerTypes;
            ExcludeDefaultValues = config.ExcludeDefaultValues;
            ExcludePropertyReferences = config.ExcludePropertyReferences;
            IncludeNullValues = config.IncludeNullValues;
            IncludeNullValuesInDictionaries = config.IncludeNullValuesInDictionaries;
            IncludeDefaultEnums = config.IncludeDefaultEnums;
            TreatEnumAsInteger = config.TreatEnumAsInteger;
            ExcludeTypeInfo = config.ExcludeTypeInfo;
            IncludeTypeInfo = config.IncludeTypeInfo;
            TypeAttr = config.TypeAttr;
            DateTimeFormat = config.DateTimeFormat;
            JsonTypeAttrInObject = config.JsonTypeAttrInObject;
            JsvTypeAttrInObject = config.JsvTypeAttrInObject;
            TypeWriter = config.TypeWriter;
            TypeFinder = config.TypeFinder;
            ParsePrimitiveFn = config.ParsePrimitiveFn;
            DateHandler = config.DateHandler;
            TimeSpanHandler = config.TimeSpanHandler;
            EmitCamelCaseNames = config.EmitCamelCaseNames;
            EmitLowercaseUnderscoreNames = config.EmitLowercaseUnderscoreNames;
            PropertyConvention = config.PropertyConvention;
            ThrowOnError = config.ThrowOnError;
            SkipDateTimeConversion = config.SkipDateTimeConversion;
            AlwaysUseUtc = config.AlwaysUseUtc;
            AssumeUtc = config.AssumeUtc;
            AppendUtcOffset = config.AppendUtcOffset;
            EscapeUnicode = config.EscapeUnicode;
            EscapeHtmlChars = config.EscapeHtmlChars;
            PreferInterfaces = config.PreferInterfaces;
            IncludePublicFields = config.IncludePublicFields;
            MaxDepth = config.MaxDepth;
            OnDeserializationError = config.OnDeserializationError;
            ModelFactory = config.ModelFactory;
            ExcludeTypes = config.ExcludeTypes;
            return this;
        }
    }
    
}


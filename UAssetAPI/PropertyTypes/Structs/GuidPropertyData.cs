using System;
using System.IO;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.UnrealTypes;
using UAssetAPI.ExportTypes;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace UAssetAPI.PropertyTypes.Structs
{
    /// <summary>
    /// Describes a 128-bit <see cref="Guid"/>.
    /// </summary>
    public class GuidPropertyData : PropertyData<Guid>
    {
        public GuidPropertyData(FName name) : base(name)
        {

        }

        public GuidPropertyData()
        {

        }

        private static readonly FString CurrentPropertyType = new FString("Guid");
        public override bool HasCustomStructSerialization { get { return true; } }
        public override FString PropertyType { get { return CurrentPropertyType; } }

        public override void Read(AssetBinaryReader reader, bool includeHeader, long leng1, long leng2 = 0)
        {
            if (includeHeader)
            {
                PropertyGuid = reader.ReadPropertyGuid();
            }

            Value = new Guid(reader.ReadBytes(16));

            var GuidUpdateJsonFile = "GuidUpdates.json";
            if(File.Exists(GuidUpdateJsonFile))
            {
                var GuidUpdateJson = File.ReadAllText(GuidUpdateJsonFile);
                var GuidUpdates = JsonConvert.DeserializeObject<List<GuidUpdate>>(GuidUpdateJson);

                int count = 0;
                
                GuidUpdate update = GuidUpdates.Find(e => 
                    e.GuidUpdateComponents.Find(f => !String.IsNullOrEmpty(f.DummyGuid) && f.DummyGuid == Value.ConvertToString().ToUpper()) != null);

                if (update != null)
                {
                    var updateEntry = update.GuidUpdateComponents.Find(f => !String.IsNullOrEmpty(f.DummyGuid) && f.DummyGuid == Value.ConvertToString().ToUpper());

                    string Log = "[" + DateTime.Now.ToString() + "]["
                        + update.ClassName + "]["
                        + updateEntry.ComponentName + "] "
                        + updateEntry.DummyGuid + " -> "
                        + updateEntry.ProductionGuid + Environment.NewLine;
                    Value = new Guid(new Guid(updateEntry.ProductionGuid).ConvertToString());

                    File.AppendAllText("GuidUpdates.log", Log);
                }
            }
        }

        public override int Write(AssetBinaryWriter writer, bool includeHeader)
        {
            if (includeHeader)
            {
                writer.WritePropertyGuid(PropertyGuid);
            }

            writer.Write(Value.ToByteArray());
            return 16;
        }

        public override string ToString()
        {
            return Value.ConvertToString();
        }

        public override void FromString(string[] d, UAsset asset)
        {
            Value = d[0].ConvertToGUID();
        }

        protected override void HandleCloned(PropertyData res)
        {
            GuidPropertyData cloningProperty = (GuidPropertyData)res;

            cloningProperty.Value = new Guid(Value.ToByteArray());
        }
    }
}
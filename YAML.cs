using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Core.Events;
using website.Models;
using System.Text.Json;
using YamlDotNet.Serialization.NodeDeserializers;

namespace website
{

    public class YamlHelper {

        // First, we'll implement a new INodeDeserializer
        // that will decorate another INodeDeserializer with validation:
        public class MetaNodeDeserializer : INodeDeserializer
        {
            private readonly INodeDeserializer _nodeDeserializer;

            public MetaNodeDeserializer(INodeDeserializer nodeDeserializer)
            {
                _nodeDeserializer = nodeDeserializer;
            }

            public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
            {
                if (_nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
                {
                    var context = new ValidationContext(value, null, null);
                    Validator.ValidateObject(value, context, true);
                    return true;
                }
                return false;
            }
        }

        public static Meta LoadMeta(string rawYaml) {
            // Load the stream
            var deserializer = new DeserializerBuilder()
                .WithNodeDeserializer(inner => new MetaNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
                .IgnoreUnmatchedProperties()
                .Build();

            var meta = deserializer.Deserialize<Meta>(rawYaml);

            return meta;
        }

        public static Meta LoadMetaFromFile(string filePath) {
            // Setup the input
            var input = new StringReader(File.ReadAllText(filePath));

            // Load the stream
            var deserializer = new DeserializerBuilder()
                .WithNodeDeserializer(inner => new MetaNodeDeserializer(inner), s => s.InsteadOf<ObjectNodeDeserializer>())
                .IgnoreUnmatchedProperties()
                .Build();

            var meta = deserializer.Deserialize<Meta>(input);

            return meta;
        }
    }
}

// Source: BinSerialization.cs
// Patrik Lundin Nov 2011
// patrik@lundin.info

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System;

namespace IOUtilities
{
    /// <summary>
    /// This class handles binary serialization and deserialization of objects to file.
    /// </summary>
    public class BinSerialization
    {
        /// <summary>
        /// Serializes the object to the specified file.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        /// <param name="filename">The filename to write to</param>
        /// <typeparam name="T">The type of the object to be serialized</typeparam>
        /// <remarks>Object must be marked serializable</remarks>
        public static void Serialize<T>( T o, string filename ) 
		{
            using (FileStream oStream = File.Open(filename, FileMode.Create, FileAccess.Write))
			{			
				BinaryFormatter serializer = new BinaryFormatter();
				serializer.Serialize(oStream, o); 
			}
		}

        /// <summary>
        /// Deserializes an object from the specified file.
        /// </summary>
        /// <param name="filename">The filename to read from</param>
        /// <typeparam name="T">The type of the object that is expected</typeparam>
        /// <returns>The object that was deserialized from the file</returns>
        /// <remarks>Object must be cast to the expected type</remarks>
        public static T Deserialize<T>(string filename)
        {
            using (FileStream oStream = File.Open(filename, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                return ((T)serializer.Deserialize(oStream));
            }
        }
    }
}

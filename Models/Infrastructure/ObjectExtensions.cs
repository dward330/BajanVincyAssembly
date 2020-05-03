using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BajanVincyAssembly.Models.Infrastructure
{
    /// <summary>
    /// Collection of extension methods for all objects
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Performs and returns deep clone copy of an object
        /// </summary>
        /// <typeparam name="T">Class Type</typeparam>
        /// <param name="currentObject"> Object to clone</param>
        /// <returns></returns>
        public static T DeepClone<T> (this T currentObject)
        {
            if (currentObject == null)
            {
                return default(T);
            }

            var objectSerialized = JsonConvert.SerializeObject(currentObject);
            var objectDeserialized = JsonConvert.DeserializeObject<T>(objectSerialized);

            return objectDeserialized;
        }
    }
}

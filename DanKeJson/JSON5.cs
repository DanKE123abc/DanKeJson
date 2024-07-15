#region Header

/*
* MIT License

Copyright (c) 2023 - present DanKe

* DanKeJson
a simple Json library for the .Net
JSON : https://json.org

* About Author
Name : DanKe
Address : Guangzhou City , Guangdong Province , China
Mail : danke1024@foxmail.com
Github : https://github.com/DanKE123abc
  */

#endregion

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace DanKeJson
{

    /// <summary>
    /// DanKeJson : Serialization and Deserialization
    /// </summary>
    public static class JSON5
    {
        /// <summary>
        /// Serializing Json5(String) to JsonData
        /// About Json5 : https://json5.org
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="useComments">Using CommentParser.cs to Use Comments</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text, bool useComments = false, bool skipFileCheck = false)
        {
            return JSON.ToData(text, useComments, skipFileCheck);
        }

        /// <summary>
        /// Serializing Json5(String) to Class
        /// About Json5 : https://json5.org
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="useComments">Using CommentParser.cs to Use Comments</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        public static T ToData<T>(string text, bool useComments = false, bool skipFileCheck = false) where T : class, new()
        {
            return JSON.ToData<T>(text, useComments, skipFileCheck);
        }
    }
    
}
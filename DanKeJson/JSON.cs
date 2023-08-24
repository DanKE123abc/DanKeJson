/*
    DanKeJson
a simple Json library for the .Net

JSON : https://json.org

    MIT License

Copyright (c) 2023-forever DanKe

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
 
    About Author
Name : DanKe
Address : Guangzhou City , Guangdong Provice , China
Mail : danke1024@foxmail.com
Github : https://github.com/DanKE123abc
  */

using System.Text;

namespace DanKeJson
{
    #pragma warning disable CS8603
   
    /// <summary>
    /// DanKeJson : Serialization and Deialization
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// Serializing Json(String) to JsonData
        /// About Json : https://json.org
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text)
        {
            int index = 0;
            JsonData json = JsonMapper.ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return json;
            }

            return null;
        }

        /// <summary>
        /// Deserializing JsonData to Json(String)
        /// </summary>
        /// <param name="json">the JsonData</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(JsonData json)
        {
            if (json == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();

            JsonMapper.ProcessData(json,stringBuilder);
            
            return stringBuilder.ToString();
        }
        
    }
}


#pragma warning disable CS8618
#pragma warning disable CS8603
#pragma warning disable CS8600
#pragma warning disable CS1591

using System;
using System.Collections.Generic;

namespace DanKeJson
{

    /// <summary>
    /// DanKeJson : JsonData
    /// </summary>
    public class JsonData
    {
        public enum Type
        {
            Object,//class
            Array,//list
            Number,//int...and so on
            Boolean,//bool
            String,//string
            None,//null
        }

        public Type type { get; set; }
        public string json { get; set; }
        
        public Dictionary<string, JsonData> map;
        public List<JsonData> array;

        public JsonData(Type type)
        {
            this.type = type;
            if (type == Type.Object)
            {
                map = new Dictionary<string, JsonData>();
            }
            else if (type == Type.Array)
            {
                array = new List<JsonData>();
            }
            else if (type == Type.None)
            {
                json = "null";
            }
        }
        
        #region string

        public static implicit operator JsonData(string value)
        {
            return new JsonData(Type.String)
            {
                json = "\"" + value + "\""
            };
        }

        public static implicit operator string(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.String)
            {
                return default;
            }
            return jsonData.json[1..^1];
        }

        #endregion

        #region bool

        public static implicit operator JsonData(bool value)
        {
            return new JsonData(Type.Boolean)
            {
                json = value.ToString().ToLower()
            };
        }

        public static implicit operator bool(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Boolean || !bool.TryParse(jsonData.json, out bool value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region int

        public static implicit operator JsonData(int value)
        {
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator int(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number || !int.TryParse(jsonData.json, out int value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region long

        public static implicit operator JsonData(long value)
        {
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator long(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number || !long.TryParse(jsonData.json, out long value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region float

        public static implicit operator JsonData(float value)
        {
            if (value == float.NaN)
            {
                return new JsonData(Type.Number)
                {
                    json = "NaN"
                };
            }
            else if (value == float.PositiveInfinity)
            {
                return new JsonData(Type.Number)
                {
                    json = "Infinity"
                };
            }
            else if (value == float.NegativeInfinity)
            {
                return new JsonData(Type.Number)
                {
                    json = "-Infinity"
                };
            }
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator float(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number)
            {
                return default;
            }
            if (jsonData.json == "NaN")
            {
                return float.NaN;
            }
            else if(jsonData.json == "Infinity")
            {
                return float.PositiveInfinity;
            }
            else if(jsonData.json== "-Infinity")
            {
                return float.NegativeInfinity;
            }
            if (float.TryParse(jsonData.json, out float value))
            {
                return value;
            }
            return default;
        }

        #endregion

        #region double

        public static implicit operator JsonData(double value)
        {
            if (value == double.NaN)
            {
                return new JsonData(Type.Number)
                {
                    json = "NaN"
                };
            }
            else if (value == double.PositiveInfinity)
            {
                return new JsonData(Type.Number)
                {
                    json = "Infinity"
                };
            }
            else if (value == double.NegativeInfinity)
            {
                return new JsonData(Type.Number)
                {
                    json = "-Infinity"
                };
            }
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }
        
        public static implicit operator double(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number)
            {
                return default;
            }
            if (jsonData.json == "NaN")
            {
                return double.NaN;
            }
            else if(jsonData.json == "Infinity")
            {
                return double.PositiveInfinity;
            }
            else if(jsonData.json== "-Infinity")
            {
                return double.NegativeInfinity;
            }
            if (double.TryParse(jsonData.json, out double value))
            {
                return value;
            }
            return default;
        }

        #endregion

        #region sbyte

        public static implicit operator JsonData(sbyte value)
        {
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator sbyte(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number || !sbyte.TryParse(jsonData.json, out sbyte value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region short

        public static implicit operator JsonData(short value)
        {
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator short(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number || !short.TryParse(jsonData.json, out short value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region uint

        public static implicit operator JsonData(uint value)
        {
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator uint(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number || !uint.TryParse(jsonData.json, out uint value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region ulong

        public static implicit operator JsonData(ulong value)
        {
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator ulong(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number || !ulong.TryParse(jsonData.json, out ulong value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region ushort

        public static implicit operator JsonData(ushort value)
        {
            return new JsonData(Type.Number)
            {
                json = value.ToString()
            };
        }

        public static implicit operator ushort(JsonData jsonData)
        {
            if (jsonData == null || jsonData.type != Type.Number || !ushort.TryParse(jsonData.json, out ushort value))
            {
                return default;
            }

            return value;
        }

        #endregion

        #region array

        public void Add(JsonData jsonData)
        {
            if (jsonData == null || array == null)
            {
                return;
            }
            array.Add(jsonData);
        }

        public JsonData this[int index]
        {
            get
            {
                if (array == null || index < 0 || index >= array.Count)
                {
                    return null;
                }

                return array[index];
            }
            set
            {
                if (array == null || index < 0 || index >= array.Count)
                {
                    return;
                }

                array[index] = value;
            }
        }

        #endregion

        #region object

        public bool HasKey(string key)
        {
            if (map == null)
            {
                return false;
            }

            return map.ContainsKey(key);
        }

        public JsonData this[string key]
        {
            get
            {
                if (map == null)
                {
                    return null;
                }

                if (map.TryGetValue(key, out JsonData json))
                {
                    return json;
                }
                return null;
            }
            set
            {
                if (map == null)
                {
                    return;
                }

                if (map.ContainsKey(key))
                {
                    map[key] = value;
                }
                else
                {
                    map.Add(key,value);
                }
                
            }
        }

        #endregion
        
        
    }

}


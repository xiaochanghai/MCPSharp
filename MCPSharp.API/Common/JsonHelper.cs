using System.Text.Json;

namespace MCPSharp.API;

public class JsonHelper
{
    /// <summary>
    /// Serializes an object to JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="isUseTextJson">Whether to use TextJson (currently unused).</param>
    /// <returns>Returns the JSON string.</returns>
    public static string ObjToJson(object obj, bool isUseTextJson = false) => JsonSerializer.Serialize(obj);

    /// <summary>
    /// Deserializes a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T">The target type for deserialization.</typeparam>
    /// <param name="strJson">The JSON string.</param>
    /// <param name="isUseTextJson">Whether to use TextJson (currently unused).</param>
    /// <returns>Returns the deserialized object.</returns>
    public static T JsonToObj<T>(string strJson, bool isUseTextJson = false) => JsonSerializer.Deserialize<T>(strJson);

    /// <summary>
    /// Converts an object to JSON-formatted string using DataContractJsonSerializer.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>JSON data in string format.</returns>
    public static string GetJSON<T>(object obj)
    {
        string result = String.Empty;
        try
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
            new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                result = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        catch (Exception)
        {
            throw;
        }
        return result;
    }

    /// <summary>
    /// Converts a List<T> to concatenated JSON strings using DataContractJsonSerializer.
    /// </summary>
    /// <typeparam name="T">The element type of the list.</typeparam>
    /// <param name="vals">The list of values.</param>
    /// <returns>Concatenated JSON-formatted data.</returns>
    public string JSON<T>(List<T> vals)
    {
        System.Text.StringBuilder st = new System.Text.StringBuilder();
        try
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer s = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));

            foreach (T city in vals)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    s.WriteObject(ms, city);
                    st.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                }
            }
        }
        catch (Exception)
        {
        }

        return st.ToString();
    }

    /// <summary>
    /// Deserializes a JSON string into an object of type T using DataContractJsonSerializer.
    /// </summary>
    /// <typeparam name="T">The target type.</typeparam>
    /// <param name="jsonStr">The JSON string.</param>
    /// <returns>The deserialized object.</returns>
    public static T ParseFormByJson<T>(string jsonStr)
    {
        T obj = Activator.CreateInstance<T>();
        using (MemoryStream ms =
        new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonStr)))
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer =
            new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(ms);
        }
    }

    public string JSON1<SendData>(List<SendData> vals)
    {
        System.Text.StringBuilder st = new System.Text.StringBuilder();
        try
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer s = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(SendData));

            foreach (SendData city in vals)
            {
                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {
                    s.WriteObject(ms, city);
                    st.Append(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
                }
            }
        }
        catch (Exception)
        {
        }

        return st.ToString();
    }

    private static bool IsJsonStart(ref string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            json = json.Trim('\r', '\n', ' ');
            if (json.Length > 1)
            {
                char s = json[0];
                char e = json[json.Length - 1];
                return (s == '{' && e == '}') || (s == '[' && e == ']');
            }
        }
        return false;
    }

    public static bool IsJson(object json)
    {
        int errIndex;
        return IsJson(json.ToString(), out errIndex);
    }

    public static bool IsJson(string json, out int errIndex)
    {
        errIndex = 0;
        if (IsJsonStart(ref json))
        {
            CharState cs = new CharState();
            char c;
            for (int i = 0; i < json.Length; i++)
            {
                c = json[i];
                if (SetCharState(c, ref cs) && cs.childrenStart)// Update symbol state.
                {
                    string item = json.Substring(i);
                    int err;
                    int length = GetValueLength(item, true, out err);
                    cs.childrenStart = false;
                    if (err > 0)
                    {
                        errIndex = i + err;
                        return false;
                    }
                    i = i + length - 1;
                }
                if (cs.isError)
                {
                    errIndex = i;
                    return false;
                }
            }

            return !cs.arrayStart && !cs.jsonStart;
        }
        return false;
    }

    /// <summary>
    /// Gets the length of a value (when nested JSON starts with "{" or "[").
    /// </summary>
    private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
    {
        errIndex = 0;
        int len = 0;
        if (!string.IsNullOrEmpty(json))
        {
            CharState cs = new CharState();
            char c;
            for (int i = 0; i < json.Length; i++)
            {
                c = json[i];
                if (!SetCharState(c, ref cs))// Update symbol state.
                {
                    if (!cs.jsonStart && !cs.arrayStart)// JSON ended and not in array, exit.
                    {
                        break;
                    }
                }
                else if (cs.childrenStart)// In value state.
                {
                    int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex);// Recursive call for nested value.
                    cs.childrenStart = false;
                    cs.valueStart = 0;
                    i = i + length - 1;
                }
                if (breakOnErr && cs.isError)
                {
                    errIndex = i;
                    return i;
                }
                if (!cs.jsonStart && !cs.arrayStart)// Record current end position.
                {
                    len = i + 1;
                    break;
                }
            }
        }
        return len;
    }

    /// <summary>
    /// Sets the character state (returns true if it's a keyword symbol; false for normal character processing).
    /// </summary>
    private static bool SetCharState(char c, ref CharState cs)
    {
        cs.CheckIsError(c);
        switch (c)
        {
            case '{'://[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                #region Brace Open
                if (cs.keyStart <= 0 && cs.valueStart <= 0)
                {
                    cs.keyStart = 0;
                    cs.valueStart = 0;
                    if (cs.jsonStart && cs.state == 1)
                    {
                        cs.childrenStart = true;
                    }
                    else
                    {
                        cs.state = 0;
                    }
                    cs.jsonStart = true;
                    return true;
                }
                #endregion
                break;
            case '}':
                #region Brace Close
                if (cs.keyStart <= 0 && cs.valueStart < 2 && cs.jsonStart)
                {
                    cs.jsonStart = false;
                    cs.state = 0;
                    cs.keyStart = 0;
                    cs.valueStart = 0;
                    cs.setDicValue = true;
                    return true;
                }
                #endregion
                break;
            case '[':
                #region Bracket Open
                if (!cs.jsonStart)
                {
                    cs.arrayStart = true;
                    return true;
                }
                else if (cs.jsonStart && cs.state == 1)
                {
                    cs.childrenStart = true;
                    return true;
                }
                #endregion
                break;
            case ']':
                #region Bracket Close
                if (cs.arrayStart && !cs.jsonStart && cs.keyStart <= 2 && cs.valueStart <= 0)
                {
                    cs.keyStart = 0;
                    cs.valueStart = 0;
                    cs.arrayStart = false;
                    return true;
                }
                #endregion
                break;
            case '"':
            case '\'':
                #region Quote
                if (cs.jsonStart || cs.arrayStart)
                {
                    if (cs.state == 0)
                    {
                        if (cs.keyStart <= 0)
                        {
                            cs.keyStart = (c == '"' ? 3 : 2);
                            return true;
                        }
                        else if ((cs.keyStart == 2 && c == '\'') || (cs.keyStart == 3 && c == '"'))
                        {
                            if (!cs.escapeChar)
                            {
                                cs.keyStart = -1;
                                return true;
                            }
                            else
                            {
                                cs.escapeChar = false;
                            }
                        }
                    }
                    else if (cs.state == 1 && cs.jsonStart)
                    {
                        if (cs.valueStart <= 0)
                        {
                            cs.valueStart = (c == '"' ? 3 : 2);
                            return true;
                        }
                        else if ((cs.valueStart == 2 && c == '\'') || (cs.valueStart == 3 && c == '"'))
                        {
                            if (!cs.escapeChar)
                            {
                                cs.valueStart = -1;
                                return true;
                            }
                            else
                            {
                                cs.escapeChar = false;
                            }
                        }
                    }
                }
                #endregion
                break;
            case ':':
                #region Colon
                if (cs.jsonStart && cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 0)
                {
                    if (cs.keyStart == 1)
                    {
                        cs.keyStart = -1;
                    }
                    cs.state = 1;
                    return true;
                }
                #endregion
                break;
            case ',':
                #region Comma
                if (cs.jsonStart)
                {
                    if (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1)
                    {
                        cs.state = 0;
                        cs.keyStart = 0;
                        cs.valueStart = 0;
                        cs.setDicValue = true;
                        return true;
                    }
                }
                else if (cs.arrayStart && cs.keyStart <= 2)
                {
                    cs.keyStart = 0;
                    return true;
                }
                #endregion
                break;
            case ' ':
            case '\r':
            case '\n':
            case '\0':
            case '\t':
                if (cs.keyStart <= 0 && cs.valueStart <= 0)
                {
                    return true;
                }
                break;
            default:
                if (c == '\\')
                {
                    if (cs.escapeChar)
                    {
                        cs.escapeChar = false;
                    }
                    else
                    {
                        cs.escapeChar = true;
                        return true;
                    }
                }
                else
                {
                    cs.escapeChar = false;
                }
                if (cs.jsonStart || cs.arrayStart)
                {
                    if (cs.keyStart <= 0 && cs.state == 0)
                    {
                        cs.keyStart = 1;
                    }
                    else if (cs.valueStart <= 0 && cs.state == 1 && cs.jsonStart)
                    {
                        cs.valueStart = 1;
                    }
                }
                break;
        }
        return false;
    }
}

/// <summary>
/// Represents the state of parsing a character during JSON validation.
/// </summary>
public class CharState
{
    internal bool jsonStart = false;        // Indicates "{" has started.
    internal bool setDicValue = false;      // Indicates dictionary value can be set.
    internal bool escapeChar = false;       // Indicates escape character "\" is active.
    /// <summary>
    /// Array started [only counts at top level]; nested values are marked by [childrenStart].
    /// </summary>
    internal bool arrayStart = false;       // Indicates "[" has started.
    internal bool childrenStart = false;    // Indicates nested child object/array started.
    /// <summary>
    /// [0: Initial state or after ","] [1: After ":"] 
    /// </summary>
    internal int state = 0;

    /// <summary>
    /// [-1: Value ended] [0: Not started] [1: No quote] [2: Single quote started] [3: Double quote started]
    /// </summary>
    internal int keyStart = 0;

    /// <summary>
    /// [-1: Value ended] [0: Not started] [1: No quote] [2: Single quote started] [3: Double quote started]
    /// </summary>
    internal int valueStart = 0;

    internal bool isError = false;          // Indicates syntax error.

    internal void CheckIsError(char c)
    {
        if (keyStart > 1 || valueStart > 1)
        {
            return;
        }

        switch (c)
        {
            case '{':
                isError = jsonStart && state == 0;
                break;
            case '}':
                isError = !jsonStart || (keyStart != 0 && state == 0);
                break;
            case '[':
                isError = arrayStart && state == 0;
                break;
            case ']':
                isError = !arrayStart || jsonStart;
                break;
            case '"':
            case '\'':
                isError = !(jsonStart || arrayStart);
                if (!isError)
                {
                    isError = (state == 0 && keyStart == -1) || (state == 1 && valueStart == -1);
                }
                if (!isError && arrayStart && !jsonStart && c == '\'')
                {
                    isError = true;
                }
                break;
            case ':':
                isError = !jsonStart || state == 1;
                break;
            case ',':
                isError = !(jsonStart || arrayStart);
                if (!isError)
                {
                    if (jsonStart)
                    {
                        isError = state == 0 || (state == 1 && valueStart > 1);
                    }
                    else if (arrayStart)
                    {
                        isError = keyStart == 0 && !setDicValue;
                    }
                }
                break;
            case ' ':
            case '\r':
            case '\n':
            case '\0':
            case '\t':
                break;
            default:
                isError = (!jsonStart && !arrayStart) || (state == 0 && keyStart == -1) || (valueStart == -1 && state == 1);
                break;
        }
    }
}
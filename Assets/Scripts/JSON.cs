using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PaintApp
{
    public static class JSON
    {
        public interface ISeralizable
        {
            /// <summary>
            /// Сериализация в JSON
            /// </summary>
            /// <param name="parent">Родительская нода, запись будет произведена в неё</param>
            /// <param name="forceName">Опциональный аргумент, не все объекты восприимчивы к нему при сериализации. Позволяет снаружи задать имя генерируемой ноды.</param>
            void JsonWriteTo(ANode parent, string forceName = "");

            /// <summary>
            /// Десериализация из JSON
            /// </summary>
            /// <param name="arg">Нода, содержащая данные, которые будут десериализованы</param>
            /// <param name="outerName">Опциональный аргумент, не все объекты восприимчивы к нему. 
            /// В случае если JSON представление не хранит имя объекта, при десериализации оно берется отсюда.</param>
            void JsonReadFrom(ANode node, string outerName = "");
        }

        public enum BinaryTag
        {
            Array = 1,
            Class = 2,
            Value = 3,
            IntValue = 4,
            DoubleValue = 5,
            BoolValue = 6,
            FloatValue = 7,
        }

        public static ANode Parse(string aJSON)
        {
            return ANode.Parse(aJSON);
        }

        #region Reading and writing unity types

        public static bool TryRead(ANode node, out JSON.ANode variable, params string[] names)
        {
            variable = null;
            foreach (string s in names)
            {
                ANode n = node[s];
                if (n == null)
                {
                    continue;
                }
                variable = n;
                return true;
            }
            return false;
        }

        public static bool TryReadStrings(ANode node, ref string[] variable, params string[] names)
        {
            foreach (string s in names)
            {
                ANode n = node[s];
                if (n == null)
                {
                    continue;
                }
                if (n is Array)
                {
                    variable = n.AsArray.Children.Select(x => x.Value).ToArray();
                }
                else if (n is Data)
                {
                    variable = new[] { n.Value };
                }
                else
                {
                    Debug.LogError("Dev.JSON.TryReadStrings: Error at \"" + s + "\": Array or Data expected!");
                    continue;
                }
                return true;
            }
            return false;
        }

        public static void ReadStrings(ANode node, ref string[] variable)
        {
            if (node == null)
            {
                Debug.LogError("Dev.JSON.ReadStrings: Error at \"" + node.ToJSON() + "\": Null!");
            }
            else if (node is Array)
            {
                variable = node.AsArray.Children.Select(x => x.Value).ToArray();
            }
            else if (node is Data)
            {
                variable = new[] { node.Value };
            }
            else
            {
                Debug.LogError("Dev.JSON.TryReadStrings: Error at \"" + node.ToJSON() + "\": Array or Data expected!");
            }
        }

        public static bool TryReadInts(ANode node, ref int[] variable, params string[] names)
        {
            foreach (string s in names)
            {
                ANode n = node[s];
                if (n == null)
                {
                    continue;
                }
                if (n is Array)
                {
                    variable = n.AsArray.Children.Select(x => x.AsInt).ToArray();
                }
                else if (n is Data)
                {
                    variable = new[] { n.AsInt };
                }
                else
                {
                    Debug.LogError("Dev.JSON.TryReadInts: Error at \"" + s + "\": Array or Data expected!");
                    continue;
                }
                return true;
            }
            return false;
        }

        public static bool TryReadFloat(ANode node, ref float variable, params string[] names)
        {
            foreach (string s in names)
            {
                ANode n = node[s];
                if (n == null)
                {
                    continue;
                }
                variable = n.AsFloat;
                return true;
            }
            return false;
        }

        public static bool TryReadInt(ANode node, ref int variable, params string[] names)
        {
            foreach (string s in names)
            {
                ANode n = node[s];
                if (n == null)
                {
                    continue;
                }
                variable = n.AsInt;
                return true;
            }
            return false;
        }

        public static bool TryReadBool(ANode node, ref bool variable, params string[] names)
        {
            foreach (string s in names)
            {
                ANode n = node[s];
                if (n == null)
                {
                    continue;
                }
                variable = n.AsBool;
                return true;
            }
            return false;
        }

        public static bool TryReadString(ANode node, ref string variable, params string[] names)
        {
            foreach (string s in names)
            {
                ANode n = node[s];
                if (n == null)
                {
                    continue;
                }
                variable = n.Value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Получение Quaternion из json
        /// </summary>
        /// <param name="node">Узел JSON</param>
        /// <returns> Quaternion </returns>
        public static Quaternion ReadQuaternion(JSON.ANode node)
        {
            float x = -1, y = -1, z = -1, w = -1;
            if (TryReadFloat(node, ref x, "x") && TryReadFloat(node, ref y, "y") && TryReadFloat(node, ref z, "z"))
            {
                return TryReadFloat(node, ref w, "w") ? new Quaternion(x, y, z, w) : Quaternion.Euler(x, y, z);
            }
            Debug.LogError("Dev.JSON.ReadQuaternion: Bad Quaternion: " + node.ToJSON());
            return Quaternion.identity;
        }

        /// <summary>
        /// Получение Vector3 из json
        /// </summary>
        /// <param name="node">Узел JSON</param>
        /// <returns> Vector3 </returns>
        public static Vector3 ReadVector3(JSON.ANode node)
        {
            float x = -1, y = -1, z = -1;
            if (TryReadFloat(node, ref x, "x") && TryReadFloat(node, ref y, "y") && TryReadFloat(node, ref z, "z"))
            {
                return new Vector3(x, y, z);
            }
            Debug.LogError("Dev.JSON.ReadVector3: Bad Vector3: " + node.ToJSON());
            return Vector3.zero;
        }

        /// <summary>
        /// Получение Vector2 из json
        /// </summary>
        /// <param name="node">Узел JSON</param>
        /// <returns> Vector2 </returns>
        public static Vector2 ReadVector2(JSON.ANode node)
        {
            return new Vector2(node["x"].AsFloat, node["y"].AsFloat);
        }

        public static AnimationEvent ReadAnimationEvent(JSON.ANode node)
        {
            AnimationEvent e = new AnimationEvent
            {
                time = node["time"].AsFloat,
                functionName = node.GetOneOf("functionName").Value
            };

            ANode sPar = node.GetOneOf("stringParameter", "stringParam");
            if (sPar != null)
            {
                e.stringParameter = sPar.Value;
            }

            ANode fPar = node.GetOneOf("floatParameter", "floatParam");
            if (fPar != null)
            {
                e.floatParameter = fPar.AsFloat;
            }

            ANode iPar = node.GetOneOf("intParameter", "intParam");
            if (iPar != null)
            {
                e.intParameter = iPar.AsInt;
            }

            return e;
        }

        public static bool TryReadClassesChildren(ANode suposedClass, Dictionary<string, ANode> destination, IEnumerable<string> exceptions = null)
        {
            Class nodeC = suposedClass as Class;
            if (nodeC == null)
            {
                Debug.LogError("Dev.JSON.GetClassesChildrenExcept: Argument is not a class!");
                return false;
            }
            bool res = false;
            for (int i = 0; i < nodeC.Count; ++i)
            {
                string name = nodeC.GetChildNameAt(i);
                if (exceptions == null || !exceptions.Contains(name))
                {
                    res = true;
                    if (destination.ContainsKey(name))
                    {
                        destination[name] = nodeC.GetChildAt(i);
                    }
                    else
                    {
                        destination.Add(name, nodeC.GetChildAt(i));
                    }
                }
            }
            return res;
        }

        #endregion

        public abstract class ANode
        {
            #region common interface

            public virtual ANode Add(string aKey, ANode aItem, bool allowNulls = true)
            {
                return this;
            }

            public virtual ANode this[int aIndex] { get { return null; } set { } }

            public virtual ANode this[string aKey] { get { return null; } set { } }

            public virtual string Value { get { return ""; } set { } }

            public virtual int Count
            {
                get { return 0; }
            }

            public virtual ANode Add(ANode aItem)
            {
                return Add("", aItem);
            }

            public virtual ANode Remove(string aKey)
            {
                return null;
            }

            public virtual ANode Remove(int aIndex)
            {
                return null;
            }

            public virtual ANode Remove(ANode aNode)
            {
                return aNode;
            }

            public ANode GetOneOf(params string[] keys)
            {
                return keys.Select(s => this[s]).FirstOrDefault(n => n != null);
            }

            public virtual IEnumerable<ANode> Children
            {
                get
                {
                    yield break;
                }
            }

            public IEnumerable<ANode> DeepChildren
            {
                get { return Children.SelectMany(C => C.DeepChildren); }
            }

            public override string ToString()
            {
                return "JSONNode";
            }

            public virtual string ToString(string aPrefix)
            {
                return "JSONNode";
            }

            public abstract string ToJSON(int prefix = 0);

            #endregion common interface

            #region typecasting properties

            public virtual BinaryTag Tag { get; set; }

            public virtual int AsInt
            {
                get
                {
                    int v;
                    return int.TryParse(Value, out v) ? v : 0;
                }
                set
                {
                    Value = value.ToString();
                    Tag = BinaryTag.IntValue;
                }
            }

            public virtual float AsFloat
            {
                get
                {
                    float v;
                    return float.TryParse(Value, out v) ? v : 0.0f;
                }
                set
                {
                    Value = value.ToString().Replace(',', '.');
                    Tag = BinaryTag.FloatValue;
                }
            }

            public virtual double AsDouble
            {
                get
                {
                    double v;
                    return double.TryParse(Value, out v) ? v : 0;
                }
                set
                {
                    Value = value.ToString();
                    Tag = BinaryTag.DoubleValue;
                }
            }

            public virtual bool AsBool
            {
                get
                {
                    bool b;
                    if (bool.TryParse(Value, out b))
                    {
                        return b;
                    }
                    int v;
                    if (int.TryParse(Value, out v))
                    {
                        return v != 0;
                    }
                    return !string.IsNullOrEmpty(Value);
                }
                set
                {
                    Value = value ? "true" : "false";
                    Tag = BinaryTag.BoolValue;
                }
            }

            public virtual Array AsArray { get { return this as Array; } }

            [Obsolete("Use AsClass instead")]
            public virtual Class AsObject { get { return this as Class; } }

            public virtual Class AsClass { get { return this as Class; } }

            #endregion typecasting properties

            #region operators

            public static implicit operator string(ANode d)
            {
                return d == null ? "null" : d.Value;
            }

            public static bool operator ==(ANode a, object b)
            {
                if (b == null)
                {
                    if (a is FakeNode)
                    {
                        return true;
                    }
                }
                return ReferenceEquals(a, b);
            }

            public static bool operator !=(ANode a, object b)
            {
                return !(a == b);
            }

            public override bool Equals(object obj)
            {
                return ReferenceEquals(this, obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            #endregion operators

            internal static string Escape(string aText)
            {
                string result = "";
                foreach (char c in aText)
                {
                    switch (c)
                    {
                        case '\\':
                            result += "\\\\";
                            break;
                        case '\"':
                            result += "\\\"";
                            break;
                        case '\n':
                            result += "\\n";
                            break;
                        case '\r':
                            result += "\\r";
                            break;
                        case '\t':
                            result += "\\t";
                            break;
                        case '\b':
                            result += "\\b";
                            break;
                        case '\f':
                            result += "\\f";
                            break;
                        default:
                            result += c;
                            break;
                    }
                }
                return result;
            }

            private static Data Numberize(string token, string tokenName = "")
            {
                int integer;
                if (int.TryParse(token, out integer))
                {
                    return new Data(integer);
                }
                double real;
                if (double.TryParse(token, out real))
                {
                    return new Data(real);
                }
                bool flag;
                if (bool.TryParse(token, out flag))
                {
                    return new Data(flag);
                }

                if (token == "null")
                {
                    return null;
                }

                Debug.LogError("Dev.JSON: Failed to read JSON Data node. Token: \"" + token + "\", Token name: \"" + tokenName + "\"");
                return new Data(-1);
            }

            private static void AddElement(ANode ctx, string token, string tokenName, bool tokenIsString)
            {
                if (tokenIsString)
                {
                    if (token == "@#$%EMPTYSTRING")
                        token = "";
                    if (ctx is Array)
                    {
                        ctx.Add(new Data(token));
                    }
                    else
                    {
                        ctx.Add(tokenName, new Data(token)); // assume dictionary/object
                    }
                }
                else
                {
                    Data number = Numberize(token, tokenName);
                    if (ctx is Array)
                    {
                        ctx.Add(number);
                    }
                    else
                    {
                        ctx.Add(tokenName, number);
                    }
                }
            }

            public static ANode Parse(string aJSON)
            {
                Stack<ANode> stack = new Stack<ANode>();
                ANode ctx = null;
                int i = 0;
                string Token = "";
                string TokenName = "";
                bool QuoteMode = false;
                bool TokenIsString = false;
                while (i < aJSON.Length)
                {
                    switch (aJSON[i])
                    {
                        case '{':
                            if (QuoteMode)
                            {
                                Token += aJSON[i];
                                break;
                            }
                            stack.Push(new Class());
                            if (ctx != null)
                            {
                                TokenName = TokenName.Trim();
                                if (ctx is Array)
                                    ctx.Add(stack.Peek());
                                else if (TokenName != "")
                                    ctx.Add(TokenName, stack.Peek());
                            }
                            TokenName = "";
                            Token = "";
                            ctx = stack.Peek();
                            break;

                        case '[':
                            if (QuoteMode)
                            {
                                Token += aJSON[i];
                                break;
                            }

                            stack.Push(new Array());
                            if (ctx != null)
                            {
                                TokenName = TokenName.Trim();

                                if (ctx is Array)
                                    ctx.Add(stack.Peek());
                                else if (TokenName != "")
                                    ctx.Add(TokenName, stack.Peek());
                            }
                            TokenName = "";
                            Token = "";
                            ctx = stack.Peek();
                            break;

                        case '}':
                        case ']':
                            if (QuoteMode)
                            {
                                Token += aJSON[i];
                                break;
                            }
                            if (stack.Count == 0)
                                throw new Exception("JSON Parse: Too many closing brackets");

                            stack.Pop();
                            if (Token != "")
                            {
                                TokenName = TokenName.Trim();
                                AddElement(ctx, Token, TokenName, TokenIsString);
                                TokenIsString = false;
                            }
                            TokenName = "";
                            Token = "";
                            if (stack.Count > 0)
                                ctx = stack.Peek();
                            break;

                        case ':':
                            if (QuoteMode)
                            {
                                Token += aJSON[i];
                                break;
                            }
                            TokenName = Token;
                            Token = "";
                            TokenIsString = false;
                            break;

                        case '"':
                            QuoteMode ^= true;
                            TokenIsString = QuoteMode || TokenIsString;
                            if (QuoteMode == false && Token == "")
                                Token = "@#$%EMPTYSTRING";
                            break;

                        case ',':
                            if (QuoteMode)
                            {
                                Token += aJSON[i];
                                break;
                            }
                            if (Token != "")
                            {
                                AddElement(ctx, Token, TokenName, TokenIsString);
                            }
                            TokenName = "";
                            Token = "";
                            TokenIsString = false;
                            break;

                        case '\r':
                        case '\n':
                            break;

                        case ' ':
                        case '\t':
                            if (QuoteMode)
                            {
                                Token += aJSON[i];
                            }
                            break;

                        case '\\':
                            ++i;
                            if (QuoteMode)
                            {
                                char C = aJSON[i];
                                switch (C)
                                {
                                    case 't':
                                        Token += '\t';
                                        break;
                                    case 'r':
                                        Token += '\r';
                                        break;
                                    case 'n':
                                        Token += '\n';
                                        break;
                                    case 'b':
                                        Token += '\b';
                                        break;
                                    case 'f':
                                        Token += '\f';
                                        break;
                                    case 'u':
                                        {
                                            string s = aJSON.Substring(i + 1, 4);
                                            Token += (char)int.Parse(s, NumberStyles.AllowHexSpecifier);
                                            i += 4;
                                            break;
                                        }
                                    default:
                                        Token += C;
                                        break;
                                }
                            }
                            break;

                        default:
                            Token += aJSON[i];
                            break;
                    }
                    ++i;
                }
                if (QuoteMode)
                {
                    Debug.LogError("JSON Parse: Quotation marks seems to be messed up.");
                }
                return ctx;
            }

            public virtual void Serialize(System.IO.BinaryWriter aWriter)
            {
            }

            public void SaveToStream(System.IO.Stream aData)
            {
                BinaryWriter W = new System.IO.BinaryWriter(aData);
                Serialize(W);
            }

            public void SaveToFile(string aFileName)
            {
                System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
                using (FileStream F = System.IO.File.OpenWrite(aFileName))
                {
                    SaveToStream(F);
                }
            }

            public string SaveToBase64()
            {
                using (MemoryStream stream = new System.IO.MemoryStream())
                {
                    SaveToStream(stream);
                    stream.Position = 0;
                    return System.Convert.ToBase64String(stream.ToArray());
                }
            }

            public static ANode Deserialize(System.IO.BinaryReader aReader)
            {
                BinaryTag type = (BinaryTag)aReader.ReadByte();
                switch (type)
                {
                    case BinaryTag.Array:
                        {
                            int count = aReader.ReadInt32();
                            Array tmp = new Array();
                            for (int i = 0; i < count; i++)
                                tmp.Add(Deserialize(aReader));
                            return tmp;
                        }
                    case BinaryTag.Class:
                        {
                            int count = aReader.ReadInt32();
                            Class tmp = new Class();
                            for (int i = 0; i < count; i++)
                            {
                                string key = aReader.ReadString();
                                var val = Deserialize(aReader);
                                tmp.Add(key, val);
                            }
                            return tmp;
                        }
                    case BinaryTag.Value:
                        {
                            return new Data(aReader.ReadString());
                        }
                    case BinaryTag.IntValue:
                        {
                            return new Data(aReader.ReadInt32());
                        }
                    case BinaryTag.DoubleValue:
                        {
                            return new Data(aReader.ReadDouble());
                        }
                    case BinaryTag.BoolValue:
                        {
                            return new Data(aReader.ReadBoolean());
                        }
                    case BinaryTag.FloatValue:
                        {
                            return new Data(aReader.ReadSingle());
                        }

                    default:
                        {
                            throw new Exception("Error deserializing  Unknown tag: " + type);
                        }
                }
            }

            public static ANode LoadFromStream(System.IO.Stream aData)
            {
                using (BinaryReader R = new System.IO.BinaryReader(aData))
                {
                    return Deserialize(R);
                }
            }

            public static ANode LoadFromFile(string aFileName)
            {
                using (FileStream F = System.IO.File.OpenRead(aFileName))
                {
                    return LoadFromStream(F);
                }
            }

            public static ANode LoadFromBase64(string aBase64)
            {
                var tmp = System.Convert.FromBase64String(aBase64);
                MemoryStream stream = new System.IO.MemoryStream(tmp)
                {
                    Position = 0
                };
                return LoadFromStream(stream);
            }
        }
        // End of JSONNode

        public class Array : ANode, IEnumerable
        {
            private readonly List<ANode> m_List;
            private readonly bool _expand;

            public Array(bool expand = true)
            {
                _expand = expand;
                m_List = new List<ANode>();
            }

            public Array(IEnumerable<ANode> arg)
                : this()
            {
                m_List.AddRange(arg);
            }

            public override ANode this[int aIndex]
            {
                get
                {
                    if (aIndex < 0 || aIndex >= m_List.Count)
                        return null;
                    return m_List[aIndex];
                }
                set
                {
                    if (aIndex < 0 || aIndex >= m_List.Count)
                        m_List.Add(value);
                    else
                        m_List[aIndex] = value;
                }
            }

            public override ANode this[string aKey]
            {
                get { return null; }
                set { m_List.Add(value); }
            }

            public override int Count { get { return m_List.Count; } }

            public override ANode Add(string aKey, ANode aItem, bool allowNulls = true)
            {
                if (aItem == null)
                {
                    if (allowNulls)
                    {
                        aItem = new Data("null");
                    }
                    else
                    {
                        Debug.LogError("Dev.JSON.ANode.Add: Nulls are not allowed as values! Key: " + aKey);
                        return this;
                    }
                }
                m_List.Add(aItem);
                return this;
            }

            public override ANode Remove(int aIndex)
            {
                if (aIndex < 0 || aIndex >= m_List.Count)
                    return null;
                ANode tmp = m_List[aIndex];
                m_List.RemoveAt(aIndex);
                return tmp;
            }

            public override ANode Remove(ANode aNode)
            {
                m_List.Remove(aNode);
                return aNode;
            }

            public override IEnumerable<ANode> Children
            {
                get
                {
                    foreach (ANode N in m_List)
                        yield return N;
                }
            }

            public IEnumerator GetEnumerator()
            {
                return m_List.GetEnumerator();
            }

            public override string ToString()
            {
                string result = "[ ";
                foreach (ANode N in m_List)
                {
                    if (result.Length > 2)
                        result += ", ";
                    result += N.ToString();
                }
                result += " ]";
                return result;
            }

            public override string ToString(string aPrefix)
            {
                string result = "[ ";
                foreach (ANode N in m_List)
                {
                    if (result.Length > 3)
                        result += ", ";
                    result += "\n" + aPrefix + "   ";
                    result += N.ToString(aPrefix + "   ");
                }
                result += "\n" + aPrefix + "]";
                return result;
            }

            public override string ToJSON(int prefix = 0)
            {
                string s = new string('\t', prefix + 1);
                string ret = "[ ";
                foreach (ANode n in m_List)
                {
                    if (ret.Length > 3)
                        ret += ", ";
                    ret += (_expand ? "\n" : "") + s;
                    ret += n.ToJSON(prefix + 1);

                }
                ret += (_expand ? "\n" + new string('\t', prefix) : " ") + "]";
                return ret;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)BinaryTag.Array);
                aWriter.Write(m_List.Count);
                foreach (ANode t in m_List)
                {
                    t.Serialize(aWriter);
                }
            }
        }
        // End of JSONArray

        public class Class : ANode, IEnumerable
        {
            private readonly Dictionary<string, ANode> _dict;
            private readonly bool _expand;

            public Class(bool expand = true)
                : base()
            {
                _dict = new Dictionary<string, ANode>();
                _expand = expand;
            }

            public override ANode this[string aKey]
            {
                get
                {
                    ANode res;
                    return _dict.TryGetValue(aKey, out res) ? res : new FakeNode(aKey, this);
                }
                set
                {
                    if (_dict.ContainsKey(aKey))
                        _dict[aKey] = value;
                    else
                        _dict.Add(aKey, value);
                }
            }

            public string GetChildNameAt(int arg)
            {
                return _dict.ElementAt(arg).Key;
            }

            public JSON.ANode GetChildAt(int arg)
            {
                return _dict.ElementAt(arg).Value;
            }

            public string[] GetNamesOfAllChildren()
            {
                return _dict.Keys.ToArray();
            }

            public override ANode this[int aIndex]
            {
                get
                {
                    if (aIndex < 0 || aIndex >= _dict.Count)
                        return null;
                    return _dict.ElementAt(aIndex).Value;
                }
                set
                {
                    if (aIndex < 0 || aIndex >= _dict.Count)
                        return;
                    string key = _dict.ElementAt(aIndex).Key;
                    _dict[key] = value;
                }
            }

            public override int Count { get { return _dict.Count; } }

            public override ANode Add(string aKey, ANode aItem, bool allowNulls = true)
            {
                if (aItem == null)
                {
                    if (allowNulls)
                    {
                        aItem = new Data("null");
                    }
                    else
                    {
                        Debug.LogError("Dev.JSON.ANode.Add: Nulls are not allowed as values! Key: " + aKey);
                        return this;
                    }
                }
                if (!string.IsNullOrEmpty(aKey))
                {
                    if (_dict.ContainsKey(aKey))
                        _dict[aKey] = aItem;
                    else
                        _dict.Add(aKey, aItem);
                }
                else
                    _dict.Add(Guid.NewGuid().ToString(), aItem);
                return this;
            }

            public override ANode Remove(string aKey)
            {
                if (!_dict.ContainsKey(aKey))
                    return null;
                ANode tmp = _dict[aKey];
                _dict.Remove(aKey);
                return tmp;
            }

            public override ANode Remove(int aIndex)
            {
                if (aIndex < 0 || aIndex >= _dict.Count)
                    return null;
                var item = _dict.ElementAt(aIndex);
                _dict.Remove(item.Key);
                return item.Value;
            }

            public override ANode Remove(ANode aNode)
            {
                try
                {
                    var item = _dict.First(k => k.Value == aNode);
                    _dict.Remove(item.Key);
                    return aNode;
                }
                catch
                {
                    return null;
                }
            }

            public override IEnumerable<ANode> Children
            {
                get { return _dict.Select(N => N.Value); }
            }

            public IEnumerator GetEnumerator()
            {
                return _dict.GetEnumerator();
            }

            public override string ToString()
            {
                string result = "{";
                foreach (KeyValuePair<string, ANode> N in _dict)
                {
                    if (result.Length > 2)
                        result += ", ";
                    result += "\"" + Escape(N.Key) + "\":" + N.Value.ToString();
                }
                result += "}";
                return result;
            }

            public override string ToString(string aPrefix)
            {
                string result = "{ ";
                foreach (KeyValuePair<string, ANode> N in _dict)
                {
                    if (result.Length > 3)
                        result += ", ";
                    result += "\n" + aPrefix + "   ";
                    result += "\"" + Escape(N.Key) + "\" : " + N.Value.ToString(aPrefix + "   ");
                }
                result += "\n" + aPrefix + "}";
                return result;
            }

            public override string ToJSON(int prefix = 0)
            {
                string s = _expand ? new string('\t', prefix + 1) : "";
                string ret = "{ ";

                foreach (KeyValuePair<string, ANode> n in _dict)
                {
                    if (ret.Length > 3)
                        ret += ", ";
                    ret += (_expand ? "\n" : "") + s;
                    ret += string.Format("\"{0}\":{1}", n.Key, n.Value.ToJSON(prefix + 1));
                }

                ret += (_expand ? "\n" + new string('\t', prefix) : " ") + "}";
                return ret;
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                aWriter.Write((byte)BinaryTag.Class);
                aWriter.Write(_dict.Count);
                foreach (string K in _dict.Keys)
                {
                    aWriter.Write(K);
                    _dict[K].Serialize(aWriter);
                }
            }
        }
        // End of JSONClass

        public class Data : ANode
        {
            private string m_Data;

            public override string Value
            {
                get { return m_Data; }
                set
                {
                    m_Data = value;
                    Tag = BinaryTag.Value;
                }
            }

            public Data(string aData)
            {
                m_Data = aData;
                Tag = BinaryTag.Value;
            }

            public Data(float aData)
            {
                AsFloat = aData;
            }

            public Data(double aData)
            {
                AsDouble = aData;
            }

            public Data(bool aData)
            {
                AsBool = aData;
            }

            public Data(int aData)
            {
                AsInt = aData;
            }

            public override string ToString()
            {
                return "\"" + Escape(m_Data) + "\"";
            }

            public override string ToString(string aPrefix)
            {
                return "\"" + Escape(m_Data) + "\"";
            }

            public override string ToJSON(int prefix = 0)
            {
                switch (Tag)
                {
                    case BinaryTag.DoubleValue:
                    case BinaryTag.FloatValue:
                    case BinaryTag.IntValue:
                    case BinaryTag.BoolValue:
                        return m_Data;
                    case BinaryTag.Value:
                        return string.Format("\"{0}\"", Escape(m_Data));
                    default:
                        throw new NotSupportedException("This shouldn't be here: " + Tag.ToString());
                }
            }

            public override void Serialize(System.IO.BinaryWriter aWriter)
            {
                Data tmp = new Data("");

                tmp.AsInt = AsInt;
                if (tmp.m_Data == this.m_Data)
                {
                    aWriter.Write((byte)BinaryTag.IntValue);
                    aWriter.Write(AsInt);
                    return;
                }
                tmp.AsFloat = AsFloat;
                if (tmp.m_Data == this.m_Data)
                {
                    aWriter.Write((byte)BinaryTag.FloatValue);
                    aWriter.Write(AsFloat);
                    return;
                }
                tmp.AsDouble = AsDouble;
                if (tmp.m_Data == this.m_Data)
                {
                    aWriter.Write((byte)BinaryTag.DoubleValue);
                    aWriter.Write(AsDouble);
                    return;
                }

                tmp.AsBool = AsBool;
                if (tmp.m_Data == this.m_Data)
                {
                    aWriter.Write((byte)BinaryTag.BoolValue);
                    aWriter.Write(AsBool);
                    return;
                }
                aWriter.Write((byte)BinaryTag.Value);
                aWriter.Write(m_Data);
            }
        }
        // End of JSONData

        internal class FakeNode : ANode
        {
            private readonly string _key;
            private readonly ANode _node;

            public FakeNode(string key, ANode node)
            {
                _key = key;
                _node = node;
            }

            private void LogError()
            {
                Debug.LogError("Dev.JSON: Key \"" + _key + "\" not found in: " + _node.ToJSON());
            }

            public override string Value
            {
                get
                {
                    LogError();
                    return "";
                }
                set { LogError(); }
            }

            public override string ToJSON(int prefix = 0)
            {
                return "";
            }

            public override Array AsArray
            {
                get
                {
                    LogError();
                    return null;
                }
            }

            [Obsolete]
            public override Class AsObject
            {
                get
                {
                    LogError();
                    return null;
                }
            }
        }
    }
}

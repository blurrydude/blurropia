using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Server.Customs.JsonSystem
{
    public class RangeDouble
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(Min);
            writer.Write(Max);
        }

        public void Deserialize(GenericReader reader)
        {
            Min = reader.ReadDouble();
            Max = reader.ReadDouble();
        }
    }
    public class RangeInt
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(Min);
            writer.Write(Max);
        }

        public void Deserialize(GenericReader reader)
        {
            Min = reader.ReadInt();
            Max = reader.ReadInt();
        }
    }
    public static class DictionarySerializer
    {
        public static void Serialize(Dictionary<string, object> dictionary, GenericWriter writer)
        {
            writer.Write(dictionary.Count);
            foreach (var kvp in dictionary)
            {
                writer.Write(kvp.Key);
                switch (kvp.Value)
                {
                    case string s:
                        writer.Write(0);
                        writer.Write(s);
                        break;
                    case int i:
                        writer.Write(1);
                        writer.Write(i);
                        break;
                    case bool b:
                        writer.Write(2);
                        writer.Write(b);
                        break;
                    case double d:
                        writer.Write(3);
                        writer.Write(d);
                        break;
                    case decimal d:
                        writer.Write(4);
                        writer.Write(d);
                        break;
                    case int[] a:
                        writer.Write(5);
                        writer.Write(a.Length);
                        foreach (var x in a)
                        {
                            writer.Write(x);
                        }
                        break;
                    case string[] a:
                        writer.Write(6);
                        writer.Write(a.Length);
                        foreach (var x in a)
                        {
                            writer.Write(x);
                        }
                        break;
                    case List<int> l:
                        writer.Write(7);
                        writer.Write(l.Count);
                        foreach (var x in l)
                        {
                            writer.Write(x);
                        }
                        break;
                    case List<string> l:
                        writer.Write(8);
                        writer.Write(l.Count);
                        foreach (var x in l)
                        {
                            writer.Write(x);
                        }
                        break;
                    case Point2D p:
                        writer.Write(9);
                        writer.Write(p.X);
                        writer.Write(p.Y);
                        break;
                    case Point3D p:
                        writer.Write(10);
                        writer.Write(p.X);
                        writer.Write(p.Y);
                        writer.Write(p.Z);
                        break;
                    case RangeDouble r:
                        writer.Write(11);
                        r.Serialize(writer);
                        break;
                }
            }
        }

        public static Dictionary<string, object> Deserialize(GenericReader reader)
        {
            var output = new Dictionary<string, object>();
            var count = reader.ReadInt();
            for(var i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                var type = reader.ReadInt();
                switch (type)
                {
                    case 0: output[key] = reader.ReadString(); break;
                    case 1: output[key] = reader.ReadInt(); break;
                    case 2: output[key] = reader.ReadBool(); break;
                    case 3: output[key] = reader.ReadDouble(); break;
                    case 4: output[key] = reader.ReadDecimal(); break;
                    case 5:
                        var li = new List<int>();
                        var ci = reader.ReadInt();
                        for (var x = 0; x < ci; x++)
                        {
                            li.Add(reader.ReadInt());
                        }
                        output[key] = li.ToArray();
                        break;
                    case 6:
                        var ls = new List<string>();
                        var cs = reader.ReadInt();
                        for (var x = 0; x < cs; x++)
                        {
                            ls.Add(reader.ReadString());
                        }
                        output[key] = ls.ToArray();
                        break;
                    case 7:
                        var lil = new List<int>();
                        var cil = reader.ReadInt();
                        for (var x = 0; x < cil; x++)
                        {
                            lil.Add(reader.ReadInt());
                        }
                        output[key] = lil;
                        break;
                    case 8:
                        var lsl = new List<string>();
                        var csl = reader.ReadInt();
                        for (var x = 0; x < csl; x++)
                        {
                            lsl.Add(reader.ReadString());
                        }
                        output[key] = lsl;
                        break;
                    case 9:
                        var p2 = new Point2D
                        {
                            X = reader.ReadInt(),
                            Y = reader.ReadInt()
                        };
                        output[key] = p2;
                        break;
                    case 10:
                        var p3 = new Point3D
                        {
                            X = reader.ReadInt(),
                            Y = reader.ReadInt(),
                            Z = reader.ReadInt()
                        };
                        output[key] = p3;
                        break;
                    case 11:
                        var rd = new RangeDouble
                        {
                            Min = reader.ReadDouble(),
                            Max = reader.ReadDouble()
                        };
                        output[key] = rd;
                        break;
                }
            }

            return output;
        }
    }
}

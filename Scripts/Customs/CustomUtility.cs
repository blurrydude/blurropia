using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server.Customs
{
    public static class CustomUtility
    {
        public static void ExceptionIgnore(Exception e)
        {

        }
        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            T[] dest = new T[source.Length - 1];
            if( index > 0 )
                Array.Copy(source, 0, dest, 0, index);

            if( index < source.Length - 1 )
                Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

            return dest;
        }
        public static Point2D GetGumpImageSize(int gumpId)
        {
            var image = Ultima.Gumps.GetGump(gumpId);
            return new Point2D(image.Width, image.Height);
        }
        public static Point2D GetStaticImageSize(int staticId)
        {
            var image = Ultima.Art.GetStatic(staticId);
            return new Point2D(image.Width, image.Height);
        }
        public static string TypePath(Type type)
        {
            var typePath = type?.ToString().Split('.').Last();

            var baseType = type?.BaseType;
            while (baseType != null)
            {
                var baseTypeName = baseType.ToString().Split('.').Last() ?? string.Empty;
                if (baseTypeName != "Object")
                {
                    typePath = $"{baseTypeName}.{typePath}";
                }

                baseType = baseType.BaseType;
            }

            return typePath;
        }
    }
}

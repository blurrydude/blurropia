using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerUtilityExtensions;


namespace Server.Customs
{
    public static class CustomUtility
    {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace PGx.KB.Common.Util
{

    public class PathUtils
    {
        private PathUtils()
        {
        }

        public static String getFilename(string file)
        {
            //Preconditions.checkNotNull(file);

            return Path.GetFileName(file.ToString());
            //file.getName(file.getNameCount() - 1).toString();
        }
        public static String getBaseFilename(string file)
        {

            return Path.GetFileNameWithoutExtension(file.ToString());
        }



    }
}
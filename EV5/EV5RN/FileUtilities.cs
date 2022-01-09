using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV5RN
{
    class FileUtilities
    {
        internal static readonly char[] DotChar = { '.' };
        internal static readonly char[] ForwardSlashBackslash = { '/', '\\' };

        internal static string FixFilePath(string path)
        {
                return string.IsNullOrEmpty(path) || Path.DirectorySeparatorChar == '\\' ? path : path.Replace('\\', '/');//.Replace("//", "/");
        }

        internal static string SearchFilePath(string path)
        {
            return string.IsNullOrEmpty(path)?string.Empty : path.Replace('\\', '/');//.Replace("//", "/");
        }

        /// <summary>
        /// This method is provided for compatibility with Everett which used to convert parts of resource names into
        /// valid identifiers
        /// </summary>
        public static string MakeValidEverettIdentifier(string name)
        {
            
            if (string.IsNullOrEmpty(name)) { return name; }

            var everettId = new StringBuilder(name.Length);

            // split the name into folder names
            string[] subNames = name.Split(ForwardSlashBackslash);

            // convert every folder name
            MakeValidEverettFolderIdentifier(everettId, subNames[0]);

            for (int i = 1; i < subNames.Length; i++)
            {
                everettId.Append('.');
                MakeValidEverettFolderIdentifier(everettId, subNames[i]);
            }

            return everettId.ToString();
        }
        /// <summary>
        /// Make a folder name into an Everett-compatible identifier
        /// </summary>
        internal static void MakeValidEverettFolderIdentifier(StringBuilder builder, string name)
        {

            if (string.IsNullOrEmpty(name)) { return; }

            // store the original length for use later
            int length = builder.Length;

            // split folder name into subnames separated by '.', if any
            string[] subNames = name.Split(DotChar);

            // convert each subname separately
            MakeValidEverettSubFolderIdentifier(builder, subNames[0]);

            for (int i = 1; i < subNames.Length; i++)
            {
                builder.Append('.');
                MakeValidEverettSubFolderIdentifier(builder, subNames[i]);
            }

            // folder name cannot be a single underscore - add another underscore to it
            if ((builder.Length - length) == 1 && builder[length] == '_')
            {
                builder.Append('_');
            }
        }

        private static void MakeValidEverettSubFolderIdentifier(StringBuilder builder, string subName)
        {

            if (string.IsNullOrEmpty(subName)) { return; }

            // the first character has stronger restrictions than the rest
            if (IsValidEverettIdFirstChar(subName[0]))
            {
                builder.Append(subName[0]);
            }
            else
            {
                builder.Append('_');
                if (IsValidEverettIdChar(subName[0]))
                {
                    // if it is a valid subsequent character, prepend an underscore to it
                    builder.Append(subName[0]);
                }
            }

            // process the rest of the subname
            for (int i = 1; i < subName.Length; i++)
            {
                if (!IsValidEverettIdChar(subName[i]))
                {
                    builder.Append('_');
                }
                else
                {
                    builder.Append(subName[i]);
                }
            }
        }


        /// <summary>
        /// Is the character a valid first Everett identifier character?
        /// </summary>
        private static bool IsValidEverettIdFirstChar(char c)
        {
            return
                char.IsLetter(c) ||
                CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.ConnectorPunctuation;
        }

        /// <summary>
        /// Is the character a valid Everett identifier character?
        /// </summary>
        private static bool IsValidEverettIdChar(char c)
        {
            UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);

            return
                char.IsLetterOrDigit(c) ||
                cat == UnicodeCategory.ConnectorPunctuation ||
                cat == UnicodeCategory.NonSpacingMark ||
                cat == UnicodeCategory.SpacingCombiningMark ||
                cat == UnicodeCategory.EnclosingMark;
        }
    }
}

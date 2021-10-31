﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EV5.Mvc
{
    /// <summary>
    /// AssetManager is a utility class containing helper functions
    /// to extract embedded resources
    /// </summary>
   public class AssetManager
    {
       /// <summary>
       /// Extracts resource string from an assembly defined by the assembly's full name
       /// </summary>
       /// <param name="name">resource's name to extract</param>
       /// <param name="assemblyName">fullname of the assembly</param>
       /// <returns></returns>
        internal static string LoadResourceString(string name, string assemblyName)
        {
            string value;
                var assembly = (from a in AppDomain.CurrentDomain.GetAssemblies()
                                where a.FullName == assemblyName
                                select a).FirstOrDefault();
                if (assembly == null) return string.Empty;
                if (assembly.GetManifestResourceInfo(name) == null) return string.Empty;
                using (var sr = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    value = sr.ReadToEnd();
                }
            return value;
        }

        /// <summary>
        /// Extracts resource string from an assembly defined by the assembly's full name
        /// </summary>
        /// <param name="name">resource's name to extract</param>
        /// <param name="assemblyName">fullname of the assembly</param>
        /// <returns></returns>
        internal static string LoadResourceString(string name)
        {
            string value;
            var assemblies = (from a in AppDomain.CurrentDomain.GetAssemblies()
                            where name.StartsWith(a.GetName().Name)
                           
                            select a);
            //select the longest match
            
            //this is O(2n)
            //int max = assemblies.Max(a=>a.GetName().Name.Length);
            //Assembly assembly = (from a in assemblies
            //                    where a.GetName().Name.Length == max
            //                    select a).First();

            //this is O(n)
            int max = 0;
            Assembly assembly = null;
            foreach (var a in assemblies)
            {
                
                var nm = a.GetName().Name.Length;
                if (nm > max)
                {
                    max = nm;
                    assembly = a;
                }
            }

            if (assembly == null) return string.Empty;
            if (assembly.GetManifestResourceInfo(name) == null) return string.Empty;
            using (var sr = new StreamReader(assembly.GetManifestResourceStream(name)))
            {
                value = sr.ReadToEnd();
            }
            return value;
        }
       /// <summary>
        /// Extracts resource string from an assembly
       /// </summary>
       /// <param name="name">name of the resource</param>
       /// <param name="assembly">Assmebly containing the resource</param>
       /// <returns></returns>
        internal static string LoadResourceString(string name, Assembly assembly)
        {
            string value;
                if (assembly == null) return string.Empty;
            if (assembly.GetManifestResourceInfo(name)==null) return string.Empty;
                using (var sr = new StreamReader(assembly.GetManifestResourceStream(name)))
                {
                    value = sr.ReadToEnd();
                }
            return value;
        }

      
       
    }
}

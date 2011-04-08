using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
using System.Linq.Expressions;

namespace System
{
    /// <summary>
    /// Wetnap extension methods for reflection.
    /// </summary>
    partial class WetnapExtensions
    {

        /// <summary>
        /// Gets the first attribute for the given member.  Searches through base classes as well.  If the attribute isn't found, null is returned.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        public static TAttribute Attribute<TAttribute>(this MemberInfo info) where TAttribute : System.Attribute
        {
            return info.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault().MapIfNotNull(a => (TAttribute)a);
        }

    }

}

using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace System
{

    /// <summary>
    /// Contains various helpful extension methods that Steve Potter has assembled throughout the years.
    /// </summary>
    public static partial class WetnapExtensions
    {

        /// <summary>
        /// A simple way to perform some operation on an object once and have that object be returned for chaining.  This is useful in chainig situations for things  like static variable initializers or anonymous types, like private static Foo bar = new Foo { ... }.ChainOp(b=>b.Event += asdfasdf);  This is the same as Ruby's tap method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public static T Once<T>(this T value, Action<T> op)
        {
            if (op != null)
                op(value);
            return value;
        }

        /// <summary>
        /// Quick way to perform some operation X number of times.  This is the same as the times method in Ruby.
        /// </summary>
        public static void Times(this int count, Action op)
        {
            if (op == null)
                throw new ArgumentNullException("op");
            for (var i = 0; i < count; i++)
            {
                op();
            }
        }

        /// <summary>
        /// Quick way to perform some operation X number of times.  This is the same as the times method in Ruby.
        /// </summary>
        public static void Times(this int count, Action<int> op)
        {
            if (op == null)
                throw new ArgumentNullException("op");
            for (var i = 0; i < count; i++)
            {
                op(i);
            }
        }



        #region Each<>

        /// <summary>
        /// Generic iterator function that is useful to replace a foreach loop with at your discretion.  A provided action is performed on each element.  This is meant to mimick jQuery's each function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action">Function that takes in the current value in the sequence. 
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            return source.Each((value, index) =>
            {
                action(value);
                return true;
            });
        }

        /// <summary>
        /// Generic iterator function that is useful to replace a foreach loop with at your discretion.  A provided action is performed on each element.  This is meant to mimick jQuery's each function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action">Function that takes in the current value and its index in the sequence.  
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            return source.Each((value, index) =>
            {
                action(value, index);
                return true;
            });
        }

        /// <summary>
        /// Generic iterator function that is useful to replace a foreach loop with at your discretion.  A provided action is performed on each element.  This is meant to mimick jQuery's each function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action">Function that takes in the current value in the sequence.  Returns a value indicating whether the iteration should continue.  So return false if you don't want to iterate anymore.</param>
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Func<T, bool> action)
        {
            return source.Each((value, index) =>
            {
                return action(value);
            });
        }

        /// <summary>
        /// Generic iterator function that is useful to replace a foreach loop with at your discretion.  A provided action is performed on each element.  This is meant to mimick jQuery's each function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action">Function that takes in the current value and its index in the sequence.  Returns a value indicating whether the iteration should continue.  So return false if you don't want to iterate anymore.</param>
        /// <returns></returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Func<T, int, bool> action)
        {
            if (source == null)
                return null;

            int index = 0;
            foreach (var sourceItem in source)
            {
                if (!action(sourceItem, index))
                    break;
                index++;
            }
            return source;
        }

        #endregion

        #region Exceptions

        /// <summary>
        /// Creates a detailed exception report for logging and whatnot.
        /// </summary>
        public static string Report(this Exception ex)
        {
            return ex.Report(null);
        }
        /// <summary>
        /// Creates a detailed exception report for logging and whatnot.
        /// </summary>
        public static string Report(this Exception ex, string heading)
        {
            return ex.Report(heading, true, true, true);
        }

        /// <summary>
        /// Creates a detailed exception report for logging and whatnot.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="heading"></param>
        /// <param name="includeInnerExceptions"></param>
        /// <param name="includeExceptionType"></param>
        /// <param name="includeStackTrace"></param>
        /// <returns></returns>
        public static string Report(this Exception ex, string heading, bool includeInnerExceptions, bool includeExceptionType, bool includeStackTrace)
        {
            Exception currEx = ex;
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(heading))
            {
                sb.AppendLine(heading);
            }

            while (true)
            {
                if (includeExceptionType)
                    sb.AppendLine("Exception type: " + currEx.GetType().FullName);
                if (!string.IsNullOrEmpty(currEx.Message))
                    sb.AppendLine("Message: " + currEx.Message);
                if (!string.IsNullOrEmpty(currEx.HelpLink))
                    sb.AppendLine("Help Link: " + currEx.HelpLink);
                if (includeStackTrace)
                    sb.AppendLine("Stack Trace: " + currEx.StackTrace);

                if (currEx.InnerException == null || !includeInnerExceptions)
                {
                    break;
                }
                currEx = currEx.InnerException;
                sb.AppendLine();
                sb.AppendLine("--- Inner Exception ---");
            }
            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// Simply casts the value to a type.  Instead of ((string)objName).ToLower(), you can write objName.CastTo&lt;string&gt;.ToLower().  This is intended to be used for fluent programming to cut down on the wide parenthesis that often is required when casting values during, say, deserialization.  If the value cannot be casted, an error will be thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>Couldn't call this Cast because IEnmerable has an extension method that does this and we didn't want to interfere. </remarks>
        public static T CastTo<T>(this object value)
        {
            return (T)value;
        }

        /// <summary>
        /// A fancy equals method that internally uses EqualityComparer.Default for the given type.  Works with all types of objects like nullable, enums, etc.  Great when working with boxed values, when normal Equals is not always reliable.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool ValueEquals(this object x, object y)
        {
            if (x == null || y == null)
                return x == null && y == null;

            Type valueType = x.GetType();
            if (!valueType.Equals(y.GetType()))
            {
                throw new ArgumentException("objects are of different types.");
            }

            IEqualityComparer comparer;
            if (!DefaultComparers.TryGetValue(valueType, out comparer))
            {
                lock (DefaultComparersLockObj)
                {
                    if (!DefaultComparers.TryGetValue(valueType, out comparer))
                    {
                        var defaultComparerProp = typeof(EqualityComparer<>).MakeGenericType(valueType).GetProperty("Default", Reflection.BindingFlags.Static | Reflection.BindingFlags.Public);
                        comparer = (IEqualityComparer)defaultComparerProp.GetValue(null, null);
                        DefaultComparers.Add(valueType, comparer);
                    }
                }
            }
            return comparer.Equals(x, y);
        }
        /// <summary>
        /// Caches the IEqualityComparers per value type to avoid constant expensive reflection lookups.
        /// </summary>
        private static Dictionary<Type, IEqualityComparer> DefaultComparers = new Dictionary<Type, IEqualityComparer>();
        private static object DefaultComparersLockObj = new object();

        /// <summary>
        /// If the value is not null, the action is performed.  This just saves a little space and avoids possible copy-paste-replace bugs because the value is specified only once.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="actionIfNotNull"></param>
        public static void IfNotNull<T>(this T val, Action<T> actionIfNotNull) where T : class
        {
            if (val != null)
                actionIfNotNull(val);
        }

        /// <summary>
        /// If the value is not null, the action is performed.  This just saves a little space and avoids possible copy-paste-replace bugs because the value is specified only once.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="actionIfNotNull"></param>
        public static void IfNotNull<T>(this T val, Action actionIfNotNull) where T : class
        {
            if (val != null)
                actionIfNotNull();
        }

        /// <summary>
        /// If the value isn't null, it returns the mapped value.  Otherwise, it returns null.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="mapIfHasValue"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static TResult MapIfNotNull<TValue, TResult>(this TValue value, Func<TValue, TResult> mapIfHasValue) where TValue : class
        {
            return MapIfNotNull(value, mapIfHasValue, default(TResult));
        }

        /// <summary>
        /// If the value is null, it returns a supplied default.  Otherwise it returns the result of the given mapping function.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="mapIfHasValue"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static TResult MapIfNotNull<TValue, TResult>(this TValue value, Func<TValue, TResult> mapIfHasValue, TResult defaultVal) where TValue : class
        {
            if (value == null)
                return defaultVal;
            return mapIfHasValue(value);
        }


        /// <summary>
        /// A helper to save some code when dealing with nullable valuess
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="ifHas"></param>
        /// <remarks>
        /// Turns this:
        ///     if (outputRequest.VideoQuality.HasValue)
        ///         output["quality"] = outputRequest.VideoQuality.Value;
        /// Into this:
        ///     outputRequest.VideoQuality.IfHasVal(v => output["quality"] = v);
        /// 
        /// Gets rid of an extra line and the need to have the variable used twice, which helps prevent bugs when copy pasting (especially in code mapping one type to another).
        /// </remarks>
        public static void IfHasVal<T>(this Nullable<T> value, Action<T> ifHas) where T : struct
        {
            if (value.HasValue)
                ifHas(value.Value);
        }


        /// <summary>
        /// A little code saver.  If the value is of the type passed, this will perform an action on that object.  The action is passed the value casted as the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <remarks>
        /// This is a simple replacement for the cumbersome:
        /// 
        /// string asString = obj as string;
        /// if ( asString != null )
        /// {
        ///     do some stuff
        /// }
        /// 
        /// That junk becomes:  obj.IfType[string](asString => do some stuff);
        /// </remarks>
        public static void IfType<T>(this object val, Action<T> actionIfIsType) where T : class
        { 
            if (val == null)
                return;

            T asT = val as T;
            if (asT != null && actionIfIsType != null)
                actionIfIsType(asT);
        }


        #region guid

        public static bool IsEmpty(this Guid value)
        {
            return value == Guid.Empty;
        }

        public static string StripDashes(this Guid value)
        {
            return value.ToString().Strip("-");
        }

        #endregion

    }

}
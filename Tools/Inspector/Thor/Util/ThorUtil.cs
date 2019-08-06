using System;
using System.Collections;
using System.Reflection;
using UnityEditor;

namespace Legendary.Thor.Util
{
    public class ThorUtil
    {
        public static readonly char MEMBER_DELIMITER = '.';
        public static readonly string ARRAY_START_DELIMITER = "[";
        public static readonly string ARRAY_END_DELIMITER = "]";
        public static readonly string UNITY_ARRAY_DELIMITER = ".Array.data[";

        public static object GetInstance(SerializedProperty property)
        {
            object instance = null;

            string simplePath = property.propertyPath.Replace(UNITY_ARRAY_DELIMITER, ARRAY_START_DELIMITER);
            instance = property.serializedObject.targetObject;
            string[] memberNames = simplePath.Split(MEMBER_DELIMITER);

            int index, delimiterIndex = 0;
            string memberName = string.Empty;
            for (int i = 0; i < memberNames.Length; i++)
            {
                if (memberNames[i].Contains(ARRAY_START_DELIMITER))
                {
                    delimiterIndex = memberNames[i].IndexOf(ARRAY_START_DELIMITER);
                    memberName = memberNames[i].Substring(0, delimiterIndex);
                    index = Int32.Parse(memberNames[i].Substring(delimiterIndex)
                        .Replace(ARRAY_START_DELIMITER, string.Empty).Replace(ARRAY_END_DELIMITER, string.Empty));
                    instance = GetValue(instance, memberName, index);
                }
                else
                {
                    instance = GetValue(instance, memberNames[i]);
                }
            }

            return instance;
        }

        public static object GetValue(object instance, string name)
        {
            if (instance == null)
                return null;

            Type type = instance.GetType();

            FieldInfo fieldInfo = type.GetField(name);
            if (fieldInfo == null)
            {
                PropertyInfo propertyInfo = type.GetProperty(name, BindingFlags.IgnoreCase);

                if (propertyInfo == null)
                    return null;

                return propertyInfo.GetValue(instance, null);
            }

            return fieldInfo.GetValue(instance);
        }

        public static object GetValue(object source, string name, int index)
        {
            object instance = null;
            IEnumerable arrayEnumerable = GetValue(source, name) as IEnumerable;

            if (arrayEnumerable != null)
            {
                IEnumerator enumerator = arrayEnumerable.GetEnumerator();
                enumerator.Reset();

                int i = 0;
                while (enumerator.MoveNext())
                {
                    if (i == index)
                    {
                        instance = enumerator.Current;
                        break;
                    }

                    i++;
                }
            }

            return instance;
        }
    }
}
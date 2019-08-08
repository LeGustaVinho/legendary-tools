using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System.Text;

namespace LegendaryTools
{
    public static class LegendaryExtension
    {
        public static void Shuffle<T>(this IList<T> list, System.Random rnd)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static void Swap(this IList list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static void MoveForward(this IList list)
        {
            for (int i = 0; i < list.Count - 1; i++)
                list.Swap(i, i + 1);
        }

        public static void MoveBackwards(this IList list)
        {
            for (int i = list.Count - 1; i > 0; i--)
                list.Swap(i, i - 1);
        }

        public static void Resize<T>(this IList<T> list, int newSize)
        {
            if (newSize == 0)
                list.Clear();
            else
            {
                int delta = newSize - list.Count;

                if (delta > 0)
                {
                    for (int i = 0; i < delta; i++)
                    {
                        if (list.Count > 0)
                            list.Insert(list.Count - 1, default(T)); //insert at last
                        else
                            list.Insert(0, default(T));
                    }
                }
                else if (delta < 0)
                {
                    for (int i = Math.Abs(delta); i >= 0; i--)
                    {
                        if (list.Count > 0)
                            list.RemoveAt(list.Count - 1); //remove last
                    }
                }
            }
        }

        public static T GetRandom<T>(this IList<T> list)
        {
            if (list != null && list.Count > 0)
                return list[UnityEngine.Random.Range(0, list.Count)];

            return default(T);
        }

        public static T FirstOrDefault<T>(this IList<T> list)
        {
            if (list != null)
            {
                if (list.Count > 0)
                    return list[0];
                else
                    return default(T);
            }
            else
                return default(T);
        }

        public static T Last<T>(this IList<T> list)
        {
            if (list != null)
            {
                if (list.Count > 0)
                    return list[list.Count-1];
                else
                    return default(T);
            }
            else
                return default(T);
        }

        public static bool Any<T>(this List<T> list, Predicate<T> match)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (match(list[i]))
                    return true;
            }

            return false;
        }

        public static bool Any<T>(this T[] array, Predicate<T> match)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (match(array[i]))
                    return true;
            }

            return false;
        }

        public static T First<T>(this IEnumerable<T> items)
        {
            using (IEnumerator<T> iter = items.GetEnumerator())
            {
                iter.MoveNext();
                return iter.Current;
            }
        }

        static Slider.SliderEvent emptySliderEvent = new Slider.SliderEvent();
        public static void SetValue(this Slider instance, float value)
        {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = emptySliderEvent;
            instance.value = value;
            instance.onValueChanged = originalEvent;
        }

        static Toggle.ToggleEvent emptyToggleEvent = new Toggle.ToggleEvent();
        public static void SetValue(this Toggle instance, bool value)
        {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = emptyToggleEvent;
            instance.isOn = value;
            instance.onValueChanged = originalEvent;
        }

        static InputField.OnChangeEvent emptyInputFieldEvent = new InputField.OnChangeEvent();
        public static void SetValue(this InputField instance, string value)
        {
            var originalEvent = instance.onValueChanged;
            instance.onValueChanged = emptyInputFieldEvent;
            instance.text = value;
            instance.onValueChanged = originalEvent;
        }

        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (System.Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static StringBuilder Clear(this StringBuilder sb)
        {
            sb.Length = 0;
            return sb;
        }

        public static float Remap(this float value, float low1, float high1, float low2, float high2)
        {
            return low2 + (high2 - low2) * (value - low1) / (high1 - low1);
        }

        public static string Beautify(this TimeSpan bufferTimeSpan, string days = "{0} days", string hours = "{0} hours", string minutes = "{0} mins", string seconds = "{0} sec")
        {
            string result = string.Empty;

            if (bufferTimeSpan.TotalDays.Abs() > 1)
                result += string.Format(days, bufferTimeSpan.TotalDays);
            else if (bufferTimeSpan.TotalHours.Abs() > 1)
                result += string.Format(hours, bufferTimeSpan.TotalHours);
            else if (bufferTimeSpan.TotalMinutes.Abs() > 1)
                result += string.Format(minutes, bufferTimeSpan.TotalMinutes);
            else
                result += string.Format(seconds, bufferTimeSpan.TotalSeconds);

            return result;
        }

        public static string Beautify(this TimeSpan ts, string dateFormat = "d/M/yyyy HH:mm:ss")
        {
            DateTime bufferDateTime = DateTime.MinValue;
            bufferDateTime = bufferDateTime.AddSeconds(ts.TotalSeconds.Abs());
            return ts.TotalSeconds < 0 ? "- " + bufferDateTime.ToString(dateFormat) : bufferDateTime.ToString(dateFormat);
        }
		
		public static bool IsSameDay(this DateTime lhs, DateTime rhs)
        {
            return lhs.Year == rhs.Year && lhs.Month == rhs.Month && lhs.Day == rhs.Day;
        }

        public static Vector3 RandomInsideBox(this Bounds bounds)
        {
            Vector3[] boundsPoints = new Vector3[5];

            boundsPoints[0] = bounds.min;
            boundsPoints[1] = bounds.max;
            boundsPoints[2] = new Vector3(boundsPoints[0].x, boundsPoints[0].y, boundsPoints[1].z);
            boundsPoints[3] = new Vector3(boundsPoints[0].x, boundsPoints[1].y, boundsPoints[0].z);
            boundsPoints[4] = new Vector3(boundsPoints[1].x, boundsPoints[0].y, boundsPoints[0].z);

            Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(boundsPoints[0].x, boundsPoints[4].x), 
                UnityEngine.Random.Range(boundsPoints[0].y, boundsPoints[3].y), 
                UnityEngine.Random.Range(boundsPoints[0].z, boundsPoints[2].z));

            return randomPoint;
        }

        public static Vector3[] BoundsPoints(this Bounds bounds)
        {
            Vector3[] boundsPoints = new Vector3[8];

            boundsPoints[0] = bounds.min;
            boundsPoints[1] = bounds.max;
            boundsPoints[2] = new Vector3(boundsPoints[0].x, boundsPoints[0].y, boundsPoints[1].z);
            boundsPoints[3] = new Vector3(boundsPoints[0].x, boundsPoints[1].y, boundsPoints[0].z);
            boundsPoints[4] = new Vector3(boundsPoints[1].x, boundsPoints[0].y, boundsPoints[0].z);
            boundsPoints[5] = new Vector3(boundsPoints[0].x, boundsPoints[1].y, boundsPoints[1].z);
            boundsPoints[6] = new Vector3(boundsPoints[1].x, boundsPoints[0].y, boundsPoints[1].z);
            boundsPoints[7] = new Vector3(boundsPoints[1].x, boundsPoints[1].y, boundsPoints[0].z);

            return boundsPoints;
        }

        public static Rect RectWorld(this RectTransform rectTransform)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);
            return new Rect(corners[1].x, corners[1].y, Mathf.Abs(corners[2].x - corners[1].x), Mathf.Abs(corners[0].y - corners[1].y));
        }

        public static bool ContainBounds(this Bounds bounds, Bounds target)
        {
            return bounds.Contains(target.min) && bounds.Contains(target.max);
        }

        public static T[] FindObjectsOfType<T>(this UnityEngine.Object unityObj, bool includeInactive = false, params UnityEngine.Transform[] nonActiveParents) where T : UnityEngine.Object
        {
            if (includeInactive)
            {
                List<T> allComponents = new List<T>();
                List<Transform> allGameObjects = new List<Transform>(UnityEngine.Object.FindObjectsOfType<Transform>());
                allGameObjects.AddRange(nonActiveParents);
                allGameObjects.RemoveAll(item => item.parent != null);

                for (int i = 0; i < allGameObjects.Count; i++)
                    allComponents.AddRange(allGameObjects[i].GetComponentsInChildren<T>(includeInactive));

                return allComponents.ToArray();
            }
            else
                return UnityEngine.Object.FindObjectsOfType<T>();
        }
		
        public static bool IsSimilar(this float lhs, float rhs, float threshold)
        {
            return Mathf.Abs(lhs - rhs) < threshold;
        }

        public static bool IsSimilar(this Vector2 lhs, Vector2 rhs, float threshold)
        {
            return lhs.x.IsSimilar(rhs.x, threshold) && lhs.y.IsSimilar(rhs.y, threshold);
        }

        public static bool IsSimilar(this Vector3 lhs, Vector3 rhs, float threshold)
        {
            return lhs.x.IsSimilar(rhs.x, threshold) && lhs.y.IsSimilar(rhs.y, threshold) && lhs.z.IsSimilar(rhs.z, threshold);
        }

        public static bool IsSameOrSubclass(this Type potentialDescendant , Type potentialBase)
        {
            return potentialDescendant.IsSubclassOf(potentialBase)
                   || potentialDescendant == potentialBase;
        }

        public static bool IsFloatFamily(this Type type)
        {
            return type == typeof(Single) || type == typeof(Double) || type == typeof(Decimal);
        }

        public static bool IsIntFamily(this Type type)
        {
            return type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) ||
                type == typeof(UInt16) || type == typeof(UInt32) || type == typeof(Int64);
        }

        public static bool IsNonStringEnumerable(this Type type)
        {
            if (type == null || type == typeof(string))
                return false;

            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static bool IsBasicType(this Type type)
        {
            if (type == typeof(bool) || type == typeof(byte) || type == typeof(char) || type == typeof(decimal) || type == typeof(double) || type.IsEnum
                || type == typeof(float) || type == typeof(int) || type == typeof(long) || type == typeof(sbyte) || type == typeof(short) || type == typeof(uint)
                || type == typeof(ulong) || type == typeof(ushort) || type == typeof(string) || type == typeof(DateTime))
                return true;
            else
                return false;
        }

        public static bool IsUnityBasicType(this Type type)
        {
            if (type == typeof(AnimationCurve) || type == typeof(Bounds) || type == typeof(Color) || type == typeof(Color32) || type == typeof(Quaternion) || type.IsEnum
                || type == typeof(Rect) || type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4) || type == typeof(Matrix4x4) || type == typeof(LayerMask)
                || type == typeof(Gradient) || type == typeof(RectOffset) || type == typeof(GUIStyle))
                return true;
            else
                return false;
        }

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsEnum;
        }

        public static bool IsUnityObject(this Type type)
        {
            return type.IsSameOrSubclass(typeof(UnityEngine.Object));
        }

        public static bool IsNullable(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;

            if (Nullable.GetUnderlyingType(type) != null)
                return true;

            if (type.IsClass)
                return true;

            if (!type.IsValueType)
                return true;

            return false;
        }

        public static bool HasDefaultConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        public static bool HasConstructorForTypes(this Type type, params Type[] types)
        {
            return type.GetConstructor(types) != null;
        }

        public static object Default(this Type type)
        {
            if (type.IsBasicType())
            {
                if (type == typeof(bool))
                    return false;
                else if (type == typeof(byte))
                    return 0;
                else if (type == typeof(char))
                    return '\0';
                else if (type == typeof(decimal))
                    return 0.0M;
                else if (type == typeof(double))
                    return 0.0D;
                else if (type.IsEnum)
                    return Enum.ToObject(type, 0);
                else if (type == typeof(float))
                    return 0.0f;
                else if (type == typeof(int))
                    return 0;
                else if (type == typeof(long))
                    return 0L;
                else if (type == typeof(sbyte))
                    return 0;
                else if (type == typeof(short))
                    return 0;
                else if (type == typeof(uint))
                    return 0;
                else if (type == typeof(ulong))
                    return 0;
                else if (type == typeof(ushort))
                    return 0;
                else if (type == typeof(string))
                    return string.Empty;
                else if (type == typeof(DateTime))
                    return DateTime.MinValue;
                else
                    return default(object);
            }
            else
                return null;
        }

        public static bool HasAttribute(this Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        public static bool CanBeSerializedByUnity(this Type type)
        {
            if (type.IsAbstract)
                return false;

            if (type == typeof(DateTime)) //because DateTime is primitive type and unity cant serialize
                return true;

            if (type.IsPrimitive)
                return true;

            if (type.IsEnum)
                return true;

            if (type.IsUnityBasicType())
                return true;

            if (type.IsArray)
                return type.GetElementType().CanBeSerializedByUnity();

            if(type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof(List<>))
                    return type.GetGenericArguments()[0].CanBeSerializedByUnity();
                else
                    return false;
            }

            if (type.IsClass)
            {
                if (type.IsUnityObject())
                    return true;
                else
                    return type.HasAttribute(typeof(System.SerializableAttribute), false);
            }

            if (type.IsStruct())
                return type.HasAttribute(typeof(System.SerializableAttribute), false);

            return false;
        }

        public static bool Implements(this Type type, Type interfaces)
        {
            return type.GetInterfaces().Any(item => item == interfaces);
        }

        public static bool DeepEquals(this object lhs, object rhs)
        {
            // Check for null on left side.
            if (System.Object.ReferenceEquals(lhs, null))
            {
                if (System.Object.ReferenceEquals(rhs, null)) // null == null = true.
                    return true;

                return false; // Only the left side is null.
            }

            if (lhs.GetType() != rhs.GetType())
                return false;

            if (lhs.GetType().IsValueType != rhs.GetType().IsValueType)
                return false;

            if (lhs.GetType().IsValueType)
            {
                Debug.Log(lhs.GetHashCode() + " == " + rhs.GetHashCode() + "?" + (lhs.GetHashCode() == rhs.GetHashCode()));
                return lhs.GetHashCode() == rhs.GetHashCode();
            }
            else
                return System.Object.ReferenceEquals(lhs, rhs);
        }
		
		public static bool PlayForward(this Animation animation, string name)
        {
            animation[name].speed = 1;

            if (!animation.isPlaying)
                animation[name].normalizedTime = 0;

            return animation.Play(name);
        }

        public static bool PlayBackward(this Animation animation, string name)
        {
            animation[name].speed = -1;

            if (!animation.isPlaying)
                animation[name].normalizedTime = 1;

            return animation.Play(name);
        }
		
		public static double Abs(this double number)
        {
            if (number < 0)
                return number * -1;
            else
                return number;
        }
    }
}
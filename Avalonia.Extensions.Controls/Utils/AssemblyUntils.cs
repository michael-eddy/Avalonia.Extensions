using Avalonia.Controls;
using System;
using System.Reflection;

namespace Avalonia.Extensions.Controls
{
    public static class AssemblyUntils
    {
        public static T CreateInstance<T>(this string assemblyPath, params object[] param)
        {
            Type type = Type.GetType(assemblyPath);
            object obj = Activator.CreateInstance(type, param);
            return (T)obj;
        }
        public static T CreateInstance<T>(this string assemblyString, string className, params object[] param)
        {
            try
            {
                if (param == null || param.Length == 0)
                    return (T)Assembly.Load(assemblyString).CreateInstance(className, false);
                else
                    return (T)Assembly.Load(assemblyString).CreateInstance(className, true, BindingFlags.Default, null, param, null, null);
            }
            catch { }
            return default;
        }
        public static void InvokeMethod(this object obj, string methodName, params object[] param)
        {
            if (obj != null)
            {
                var type = obj.GetType();
                MethodInfo meth = type.GetMethod(methodName);
                meth.Invoke(obj, param);
            }
        }
        public static T InvokeMethod<T>(this object obj, string methodName, params object[] param)
        {
            var type = obj.GetType();
            MethodInfo meth = type.GetMethod(methodName);
            return (T)meth.Invoke(obj, param);
        }
        public static T InvokeStaticMethod<T>(this object obj, string methodName, params object[] param)
        {
            Type type = obj.GetType();
            return (T)type.InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, param);
        }
        public static T InvokeStaticMethod<T>(this string assemblyString, string className, string methodName, params object[] param)
        {
            var assembly = Assembly.Load(assemblyString);
            Type type = assembly.GetType(className);
            return (T)type.InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, param);
        }
        public static T GetPrivateField<T>(this object obj, string fieldName)
        {
            try
            {
                var type = obj.GetType();
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo field = type.GetField(fieldName, flag);
                if (field == null)
                    return type.BaseType.GetPrivateField<T>(obj, fieldName);
                else
                    return (T)field?.GetValue(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static T GetPrivateField<T>(this Type type, object control, string fieldName)
        {
            try
            {
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo field = type.GetField(fieldName, flag);
                if (field == null)
                    return type.BaseType.GetPrivateField<T>(control, fieldName);
                else
                    return (T)field?.GetValue(control);
            }
            catch { }
            return default;
        }
        public static T GetPrivateField<T>(this NameScope scope, string fieldName)
        {
            try
            {
                var type = scope.GetType();
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo field = type.GetField(fieldName, flag);
                return (T)field?.GetValue(scope);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SetPrivateField(this object obj, string fieldName, object fieldValue)
        {
            try
            {
                var type = obj.GetType();
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                var field = type.GetField(fieldName, flag);
                field?.SetValue(obj, fieldValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static T GetPrivateProperty<T>(this object obj, string propertyName)
        {
            try
            {
                var type = obj.GetType();
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                var property = type.GetProperty(propertyName, flag);
                return (T)property?.GetValue(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SetPrivateProperty(this object obj, string propertyName, object propertyValue)
        {
            try
            {
                var type = obj.GetType();
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                var property = type.GetProperty(propertyName, flag);
                property?.SetValue(obj, propertyValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
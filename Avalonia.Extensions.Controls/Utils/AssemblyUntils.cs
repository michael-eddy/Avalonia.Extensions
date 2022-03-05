using Avalonia.Controls;
using Avalonia.Logging;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Avalonia.Extensions.Controls
{
    public static class AssemblyUntils
    {
        public static void ReplaceMethod(MethodInfo methodToReplace, MethodInfo methodToInject)
        {
            try
            {
                RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
                RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);
                unsafe
                {
                    if (IntPtr.Size == 4)
                    {
                        int* inj = (int*)methodToInject.MethodHandle.Value.ToPointer() + 2;
                        int* tar = (int*)methodToReplace.MethodHandle.Value.ToPointer() + 2;
#if DEBUG
                        byte* injInst = (byte*)*inj;
                        byte* tarInst = (byte*)*tar;
                        int* injSrc = (int*)(injInst + 1);
                        int* tarSrc = (int*)(tarInst + 1);
                        *tarSrc = ((int)injInst + 5 + *injSrc) - ((int)tarInst + 5);
#else
                    *tar = *inj;
#endif
                    }
                    else
                    {
                        long* inj = (long*)methodToInject.MethodHandle.Value.ToPointer() + 1;
                        long* tar = (long*)methodToReplace.MethodHandle.Value.ToPointer() + 1;
#if DEBUG
                        byte* injInst = (byte*)*inj;
                        byte* tarInst = (byte*)*tar;
                        int* injSrc = (int*)(injInst + 1);
                        int* tarSrc = (int*)(tarInst + 1);
                        *tarSrc = ((int)injInst + 5 + *injSrc) - ((int)tarInst + 5);
#else
                    *tar = *inj;
#endif
                    }
                }
            }
            catch { }
        }
        public static T CreateInstance<T>(this string assemblyPath, params object[] param)
        {
            try
            {
                Type type = Type.GetType(assemblyPath);
                object obj = Activator.CreateInstance(type, param);
                return (T)obj;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(assemblyPath, ex.Message);
                throw ex;
            }
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
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(assemblyString, ex.Message);
                throw ex;
            }
        }
        public static void InvokeMethod(this object obj, string methodName, params object[] param)
        {
            try
            {
                if (obj != null)
                {
                    var type = obj.GetType();
                    MethodInfo meth = type.GetMethod(methodName);
                    meth.Invoke(obj, param);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(obj, ex.Message);
            }
        }
        public static T InvokeMethod<T>(this object obj, string methodName, params object[] param)
        {
            try
            {
                var type = obj.GetType();
                MethodInfo meth = type.GetMethod(methodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic);
                return (T)meth.Invoke(obj, param);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(obj, ex.Message);
                throw ex;
            }
        }
        public static T InvokeStaticMethod<T>(this object obj, string methodName, params object[] param)
        {
            try
            {
                Type type = obj.GetType();
                return (T)type.InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null, param);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(obj, ex.Message);
                throw ex;
            }
        }
        public static T InvokeStaticMethod<T>(this string assemblyString, string className, string methodName, params object[] param)
        {
            try
            {
                var assembly = Assembly.Load(assemblyString);
                Type type = assembly.GetType(className);
                return (T)type.InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic, null, null, param);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(assemblyString, ex.Message);
                throw ex;
            }
        }
        public static object GetPrivateField(this object obj, string fieldName)
        {
            try
            {
                var type = obj.GetType();
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo field = type.GetField(fieldName, flag);
                if (field == null)
                    return type.BaseType.GetPrivateField(obj, fieldName);
                else
                    return field?.GetValue(obj);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(obj, ex.Message);
                throw ex;
            }
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
                Logger.TryGet(LogEventLevel.Warning, LogArea.Control)?.Log(obj, ex.Message);
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
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(type, ex.Message);
                throw ex;
            }
        }
        public static object GetPrivateField(this Type type, object control, string fieldName)
        {
            try
            {
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo field = type.GetField(fieldName, flag);
                if (field == null)
                    return type.BaseType.GetPrivateField(control, fieldName);
                else
                    return field?.GetValue(control);
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(type, ex.Message);
                throw ex;
            }
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
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(scope, ex.Message);
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
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(obj, ex.Message);
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
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(obj, ex.Message);
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
                Logger.TryGet(LogEventLevel.Error, LogArea.Control)?.Log(obj, ex.Message);
                throw ex;
            }
        }
    }
}
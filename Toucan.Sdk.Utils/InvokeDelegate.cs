using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Toucan.Sdk.Utils;

public static class InvokeDelegate
{
    public static Delegate NoOwnerStaticDelegate<T>(Type? returnType = null, Type[]? types = null, [CallerMemberName()] string name = default!)
    {
        string n = name ?? "NoOwnerStatic";
        DynamicMethod noOwnerStaticDynamicMethod = new(
                n,
                returnType ?? typeof(void),
                types ?? []);
        ILGenerator noOwnerStaticDynamicMethodIl = noOwnerStaticDynamicMethod.GetILGenerator();
        noOwnerStaticDynamicMethodIl.EmitWriteLine($"{n}DynamicMethod");
        noOwnerStaticDynamicMethodIl.Emit(OpCodes.Ret);
        return noOwnerStaticDynamicMethod.CreateDelegate(typeof(T));
    }

    public static Delegate OwnerStaticDelegate<TOwner, T>(Type? returnType = null, Type[]? types = null, [CallerMemberName()] string name = default!)
    {
        string n = name ?? "OwnedStatic";
        DynamicMethod ownedStaticDynamicMethod = new(
               n,
               returnType ?? typeof(void),
               types ?? [],
               typeof(TOwner));
        ILGenerator ownedStaticDynamicMethodIl = ownedStaticDynamicMethod.GetILGenerator();
        ownedStaticDynamicMethodIl.EmitWriteLine($"{n}DynamicMethod");
        ownedStaticDynamicMethodIl.Emit(OpCodes.Ret);
        return ownedStaticDynamicMethod.CreateDelegate(typeof(T));
    }

    public static Delegate OwnerDelegate<TOwner, T>(TOwner owner, Type? returnType = null, Type[]? types = null, [CallerMemberName()] string name = default!)
    {
        string n = name ?? "InstanceMethod";
        DynamicMethod instanceDynamicMethod = new(
                n,
               returnType ?? typeof(void),
                types ?? new Type[]
                {
                    typeof(TOwner)
                },
                typeof(TOwner));
        ILGenerator instanceDynamicMethodIl = instanceDynamicMethod.GetILGenerator();
        instanceDynamicMethodIl.EmitWriteLine($"{n}Method");
        instanceDynamicMethodIl.Emit(OpCodes.Ret);
        return instanceDynamicMethod.CreateDelegate(typeof(T), owner);
    }



    public static object? RunWithArgs(
           IServiceProvider provider,
           Delegate @delegate,
           params object?[] args)
    {
        MethodInfo method = @delegate.GetMethodInfo();

        ParameterInfo[] parameters = method.GetParameters();

        // When delegate Method is static the delegate was created using
        // Expression.Lambda or DynamicMethod
        //
        // (because compiler generated delegates are always instance methods).
        if (method.IsStatic)
        {
            // If the Target isn't null Then we need to skip first parameter.
            //
            // (because DynamicInvoke would bind Target as first argument automatically).
            if (@delegate.Target != null)
                parameters = parameters.Skip(1).ToArray();
        }

        object?[] arguments = parameters
           .Select(pi =>
           {
               object? arg = args.FirstOrDefault(x => x != null && x.GetType().IsAssignableTo(pi.ParameterType));
               if (arg != null)
                   return arg;
               object? resolved = provider.GetService(pi.ParameterType);
               if (resolved != null)
                   return resolved;
               return null;
           })
           .ToArray();

        return @delegate.DynamicInvoke(arguments);
    }


    public static object Run(
           IServiceProvider provider,
           Delegate @delegate)
    {
        MethodInfo method = @delegate.GetMethodInfo();

        ParameterInfo[] parameters = method.GetParameters();

        // When delegate Method is static the delegate was created using
        // Expression.Lambda or DynamicMethod
        //
        // (because compiler generated delegates are always instance methods).
        if (method.IsStatic)
        {
            // If the Target isn't null Then we need to skip first parameter.
            //
            // (because DynamicInvoke would bind Target as first argument automatically).
            if (@delegate.Target != null)
                parameters = parameters.Skip(1).ToArray();
        }

        object[] arguments = parameters
           .Select(pi => provider.GetService(pi.ParameterType) ?? throw new NullReferenceException($"Cannot resolve service for {pi.ParameterType}"))
           .ToArray();

        return @delegate.DynamicInvoke(arguments)!;
    }

    public static Task RunAsync(
        IServiceProvider provider,
        Delegate @delegate)
    {
        MethodInfo method = @delegate.GetMethodInfo();

        if (method.ReturnParameter != null)
        {
            if (typeof(Task).IsAssignableFrom(method.ReturnType))
                return (Task)Run(provider, @delegate);

            if (typeof(ValueTask) == method.ReturnType)
                return ((ValueTask)Run(provider, @delegate)).AsTask();

            if (method.ReturnType.IsGenericType)
            {
                if (typeof(ValueTask<>) == method.ReturnType.GetGenericTypeDefinition())
                {
                    Expression<Func<Task>> expression = () => InvokeDelegateValueTaskAsync<int>(null!, null!);

                    MethodInfo m = ((MethodCallExpression)expression.Body)
                       .Method
                       .GetGenericMethodDefinition()
                       .MakeGenericMethod(method.ReturnType.GetGenericArguments());

                    return (Task)m.Invoke(
                        null,
                        new object[]
                        {
                                provider,
                                @delegate
                        })!;
                }
            }
        }

        Run(provider, @delegate);

        return Task.CompletedTask;
    }

    public static Task InvokeDelegateValueTaskAsync<T>(
        IServiceProvider provider,
        Delegate @delegate) => ((ValueTask<T>)Run(provider, @delegate)).AsTask();
}


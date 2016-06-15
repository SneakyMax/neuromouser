using System;
using JetBrains.Annotations;

namespace Assets._Scripts
{
    [MeansImplicitUse(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Itself)]
    [AttributeUsage(AttributeTargets.Method)]
    public class UnityMessageAttribute : Attribute
    {
    }

    [MeansImplicitUse(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class AssignedInUnityAttribute : Attribute
    {

    }

    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [AttributeUsage(AttributeTargets.Class)]
    public class UnityComponentAttribute : Attribute
    {

    }

    [MeansImplicitUse(ImplicitUseKindFlags.Access, ImplicitUseTargetFlags.Itself)]
    [AttributeUsage(AttributeTargets.Method)]
    public class CalledFromUnityAttribute : Attribute
    {

    }

    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectedAttribute : Attribute
    {

    }
}
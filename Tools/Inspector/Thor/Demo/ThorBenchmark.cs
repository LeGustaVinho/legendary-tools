using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum BenchmarkEnum
{
    A,
    B,
    C,
    D,
}

[Flags]
public enum BenchmarkFlag
{
    A = 1,
    B = 2,
    C = 4,
    D = 8,
    E = 16,
}

public interface IBenchmarkInterface
{

}


public class InterfaceImplClass : IBenchmarkInterface
{

}

public class BenchmarkBaseClass
{

}

public class BenchmarClassA : BenchmarkBaseClass
{

}

public class BenchmarClassB : BenchmarkBaseClass
{

}

public class ThorBenchmark : MonoBehaviour
{
    public Int16 Int16Var;
    public Int32 Int32Var;
    public Int64 Int64Var;

    public Single SingleVar;
    public Double DoubleVar;
    public Decimal DecimalVar;

    public Char CharVar;
    public String StringVar;

    public Boolean BoolVar;
    public Byte ByteVar;
    public SByte SByteVar;

    public Type TypeVar;

    public BenchmarkEnum EnumVar;
    public BenchmarkFlag EnumFlagVar;

    public AnimationCurve AnimationCurveVar;
    public Gradient GradientVar;
    public LayerMask LaskMaskVar;
    public Color ColorVar;
    public Vector2 Vector2Var;
    public Vector3 Vector3Var;
    public Quaternion QuaternionVar;
    public GameObject GameObjectVar;
    public Texture TextureVar;
    public MonoBehaviour MonoBehaviourVar;

    public Dictionary<string, int> DictionaryVar;

    public Action ActionVar;

    public BenchmarkBaseClass BenchmarkBaseClassVar;

    public InterfaceImplClass InterfaceVar;

    public Int16? NullableInt16Var;
    public Int32? NullableInt32Var;
    public Int64? NullableInt64Var;
    public Single? NullableSingleVar;
    public Double? NullableDoubleVar;

    //============= ARRAY ============

    public Int16[] Int16VarArray;
    public Int32[] Int32VarArray;
    public Int64[] Int64VarArray;

    public Single[] SingleVarArray;
    public Double[] DoubleVarArray;
    public Decimal[] DecimalVarArray;

    public Char[] CharVarArray;
    public String[] StringVarArray;

    public Boolean[] BoolVarArray;
    public Byte[] ByteVarArray;
    public SByte[] SByteVarArray;

    public Type[] TypeVarArray;

    public BenchmarkEnum[] EnumVarArray;
    public BenchmarkFlag[] EnumFlagVarArray;

    public AnimationCurve[] AnimationCurveVarArray;
    public Gradient[] GradientVarArray;
    public LayerMask[] LaskMaskVarArray;
    public Color[] ColorVarArray;
    public Vector2[] Vector2VarArray;
    public Vector3[] Vector3VarArray;
    public Quaternion[] QuaternionVarArray;
    public GameObject[] GameObjectVarArray;
    public Texture[] TextureVarArray;
    public MonoBehaviour[] MonoBehaviourVarArray;

    public Dictionary<string, int>[] DictionaryVarArray;

    public Action[] ActionVarArray;

    public BenchmarkBaseClass[] BenchmarkBaseClassVarArray;

    public InterfaceImplClass[] InterfaceVarArray;

    public Int16?[] NullableInt16VarArray;
    public Int32?[] NullableInt32VarArray;
    public Int64?[] NullableInt64VarArray;
    public Single?[] NullableSingleVarArray;
    public Double?[] NullableDoubleVarArray;

    //============= PROPERTY ============

    private Int16 int16PropertyVar;
    private Int32 int32PropertyVar;
    private Int64 int64PropertyVar;

    private Single singlePropertyVar;
    private Double doublePropertyVar;
    private Decimal decimalPropertyVar;

    private Char charPropertyVar;
    private String stringPropertyVar;

    private Boolean boolPropertyVar;
    private Byte bytePropertyVar;
    private SByte sBytePropertyVar;

    private Type typePropertyVar;

    private BenchmarkEnum enumPropertyVar;
    private BenchmarkFlag enumFlagPropertyVar;

    private AnimationCurve animationCurvePropertyVar;
    private Gradient gradientPropertyVar;
    private LayerMask laskMaskPropertyVar;
    private Color colorPropertyVar;
    private Vector2 vector2PropertyVar;
    private Vector3 vector3PropertyVar;
    private Quaternion quaternionPropertyVar;
    private GameObject gameObjectPropertyVar;
    private Texture texturePropertyVar;
    private MonoBehaviour monoBehaviourPropertyVar;

    private Dictionary<string, int> dictionaryPropertyVar;

    private Action actionPropertyVar;

    private BenchmarkBaseClass benchmarkBaseClassPropertyVar;

    private InterfaceImplClass interfacePropertyVar;

    private Int16? nullableInt16PropertyVar;
    private Int32? nullableInt32PropertyVar;
    private Int64? nullableInt64PropertyVar;
    private Single? nullableSinglePropertyVar;
    private Double? nullableDoublePropertyVar;

    public short Int16PropertyVar { get => int16PropertyVar; set => int16PropertyVar = value; }
    public int Int32PropertyVar { get => int32PropertyVar; set => int32PropertyVar = value; }
    public long Int64PropertyVar { get => int64PropertyVar; set => int64PropertyVar = value; }
    public float SinglePropertyVar { get => singlePropertyVar; set => singlePropertyVar = value; }
    public double DoublePropertyVar { get => doublePropertyVar; set => doublePropertyVar = value; }
    public decimal DecimalPropertyVar { get => decimalPropertyVar; set => decimalPropertyVar = value; }
    public char CharPropertyVar { get => charPropertyVar; set => charPropertyVar = value; }
    public string StringPropertyVar { get => stringPropertyVar; set => stringPropertyVar = value; }
    public bool BoolPropertyVar { get => boolPropertyVar; set => boolPropertyVar = value; }
    public byte BytePropertyVar { get => bytePropertyVar; set => bytePropertyVar = value; }
    public sbyte SBytePropertyVar { get => sBytePropertyVar; set => sBytePropertyVar = value; }
    public Type TypePropertyVar { get => typePropertyVar; set => typePropertyVar = value; }
    public BenchmarkEnum EnumPropertyVar { get => enumPropertyVar; set => enumPropertyVar = value; }
    public BenchmarkFlag EnumFlagPropertyVar { get => enumFlagPropertyVar; set => enumFlagPropertyVar = value; }
    public AnimationCurve AnimationCurvePropertyVar { get => animationCurvePropertyVar; set => animationCurvePropertyVar = value; }
    public Gradient GradientPropertyVar { get => gradientPropertyVar; set => gradientPropertyVar = value; }
    public LayerMask LaskMaskPropertyVar { get => laskMaskPropertyVar; set => laskMaskPropertyVar = value; }
    public Color ColorPropertyVar { get => colorPropertyVar; set => colorPropertyVar = value; }
    public Vector2 Vector2PropertyVar { get => vector2PropertyVar; set => vector2PropertyVar = value; }
    public Vector3 Vector3PropertyVar { get => vector3PropertyVar; set => vector3PropertyVar = value; }
    public Quaternion QuaternionPropertyVar { get => quaternionPropertyVar; set => quaternionPropertyVar = value; }
    public GameObject GameObjectPropertyVar { get => gameObjectPropertyVar; set => gameObjectPropertyVar = value; }
    public Texture TexturePropertyVar { get => texturePropertyVar; set => texturePropertyVar = value; }
    public MonoBehaviour MonoBehaviourPropertyVar { get => monoBehaviourPropertyVar; set => monoBehaviourPropertyVar = value; }
    public Dictionary<string, int> DictionaryPropertyVar { get => dictionaryPropertyVar; set => dictionaryPropertyVar = value; }
    public Action ActionPropertyVar { get => actionPropertyVar; set => actionPropertyVar = value; }
    public BenchmarkBaseClass BenchmarkBaseClassPropertyVar { get => benchmarkBaseClassPropertyVar; set => benchmarkBaseClassPropertyVar = value; }
    public InterfaceImplClass InterfacePropertyVar { get => interfacePropertyVar; set => interfacePropertyVar = value; }
    public short? NullableInt16PropertyVar { get => nullableInt16PropertyVar; set => nullableInt16PropertyVar = value; }
    public int? NullableInt32PropertyVar { get => nullableInt32PropertyVar; set => nullableInt32PropertyVar = value; }
    public long? NullableInt64PropertyVar { get => nullableInt64PropertyVar; set => nullableInt64PropertyVar = value; }
    public float? NullableSinglePropertyVar { get => nullableSinglePropertyVar; set => nullableSinglePropertyVar = value; }
    public double? NullableDoublePropertyVar { get => nullableDoublePropertyVar; set => nullableDoublePropertyVar = value; }

    //============= STATIC =================

    public static Int16 Int16StaticVar;
    public static Int32 Int32StaticVar;
    public static Int64 Int64StaticVar;

    public static Single SingleStaticVar;
    public static Double DoubleStaticVar;
    public static Decimal DecimalStaticVar;

    public static Char CharStaticVar;
    public static String StringStaticVar;

    public static Boolean BoolStaticVar;
    public static Byte ByteStaticVar;
    public static SByte SByteStaticVar;

    public static Type TypeStaticVar;

    public static BenchmarkEnum EnumStaticVar;
    public static BenchmarkFlag EnumFlagStaticVar;

    public static AnimationCurve AnimationCurveStaticVar;
    public static Gradient GradientStaticVar;
    public static LayerMask LaskMaskStaticVar;
    public static Color ColorStaticVar;
    public static Vector2 Vector2StaticVar;
    public static Vector3 Vector3StaticVar;
    public static Quaternion QuaternionStaticVar;
    public static GameObject GameObjectStaticVar;
    public static Texture TextureStaticVar;
    public static MonoBehaviour MonoBehaviourStaticVar;

    public static Dictionary<string, int> DictionaryStaticVar;

    public static Action ActionStaticVar;

    public static BenchmarkBaseClass BenchmarkBaseClassStaticVar;

    public static InterfaceImplClass InterfaceStaticVar;

    public static Int16? NullableInt16StaticVar;
    public static Int32? NullableInt32StaticVar;
    public static Int64? NullableInt64StaticVar;
    public static Single? NullableSingleStaticVar;
    public static Double? NullableDoubleStaticVar;
}
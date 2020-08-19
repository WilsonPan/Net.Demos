# 背景

今天在维护一个旧项目的时候，看到一个方法把`string` 转换为 `byte[]` 用的是写入内存流的，然后`ToArray()`，因为平常都是用`System.Text.Encoding.UTF8.GetBytes(string)` ,刚好这里遇到一个安全的问题，就想把它重构了。

由于这个是已经找不到原来开发的人员，所以也无从问当时为什么要这么做，我想就算找到应该他也不知道当时为什么要这么做。

由于这个是线上跑了很久的项目，所以需要做一下测试，万一真里面真的是有历史原因呢！于是就有了这篇文章。


# 重构过程

1. 需要一个比较`byte`数组的函数(确保重构前后一致)，没找到有系统自带，所以写了一个
2. 重构方法（使用Encoding）
3. 单元测试
4. 基准测试（或许之前是为了性能考虑，因为这个方法调用次数也不少）

## 字节数组比较方法：`BytesEquals`
> 比较字节数组是否完全相等，方法比较简单，就不做介绍

```cs
public static bool BytesEquals(byte[] array1, byte[] array2)
{
    if (array1 == null && array2 == null) return true;

    if (Array.ReferenceEquals(array1, array2)) return true;

    if (array1?.Length != array2?.Length) return false;

    for (int i = 0; i < array1.Length; i++)
    {
        if (array1[i] != array2[i]) return false;
    }
    return true;
}
```

## 重构方法

> 原始方法（使用StreamWriter）
```cs
public static byte[] StringToBytes(string value)
{
    if (value == null) throw new ArgumentNullException(nameof(value));

    using (var ms = new System.IO.MemoryStream())
    using (var streamWriter = new System.IO.StreamWriter(ms, System.Text.Encoding.UTF8))
    {
        streamWriter.Write(value);
        streamWriter.Flush();

        return ms.ToArray();
    }
}
```
> 重构（使用Encoidng）
```cs
public static byte[] StringToBytes(string value)
{
    if (value == null) throw new ArgumentNullException(nameof(value));

    return System.Text.Encoding.UTF8.GetBytes(value);
}
```

## 单元测试

- **BytesEquals 单元测试**

1. 新建单元测试项目
```cs
dotnet new xunit -n 'Demo.StreamWriter.UnitTests' 
```

2. 编写单元测试
```cs
[Fact]
public void BytesEqualsTest_Equals_ReturnTrue()
{
    ...
}

[Fact]
public void BytesEqualsTest_NotEquals_ReturnFalse()
{
    ...
}

[Fact]
public void StringToBytes_Equals_ReturnTrue()
{
    ...
}

```

3. 执行单元测试
```cs
dotnet test
```

4. `StringToBytes_Equals_ReturnTrue` 未能通过单元测试

这个未能通过，重构后的生成的字节数组与原始不一致

## 排查过程

1. 调试`StringToBytes_Equals_ReturnTrue` , 发现`bytesWithStream` 比 `bytesWithEncoding` 在数组头多了三个字节（很多人都能猜到这个是UTF8的BOM）
   
``` diff
+ bytesWithStream[0] = 239
+ bytesWithStream[1] = 187
+ bytesWithStream[2] = 191
bytesWithStream[3] = 72
bytesWithStream[4] = 101

bytesWithEncoding[0] = 72
bytesWithEncoding[0] = 101

```
不了解BOM，可以看看这篇文章[Byte order mark](https://en.wikipedia.org/wiki/Byte_order_mark)

从文章可以明确多出来字节就是UTF8-BOM，问题来了，为什么`StreamWriter`会多出来BOM，而`Encoding.UTF8` 没有，都是用同一个编码

## 查看源码

`StreamWriter`

```cs
public StreamWriter(Stream stream)
    : this(stream, UTF8NoBOM, 1024, leaveOpen: false)
{
}

public StreamWriter(Stream stream, Encoding encoding)
    : this(stream, encoding, 1024, leaveOpen: false)
{
}
```

```cs
private static Encoding UTF8NoBOM => EncodingCache.UTF8NoBOM;

internal static readonly Encoding UTF8NoBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
```

可以看到`StreamWriter`, 默认是使用`UTF8NoBOM` , 但是在这里指定了`System.Text.Encoding.UTF8`，根据`encoderShouldEmitUTF8Identifier`这个参数决定是否写入BOM，最终是在`Flush`写入

```cs
private void Flush(bool flushStream, bool flushEncoder)
{
    ...
    if (!_haveWrittenPreamble)
    {
        _haveWrittenPreamble = true;
        ReadOnlySpan<byte> preamble = _encoding.Preamble;
        if (preamble.Length > 0)
        {
            _stream.Write(preamble);
        }
    }
    int bytes = _encoder.GetBytes(_charBuffer, 0, _charPos, _byteBuffer, 0, flushEncoder);
    _charPos = 0;
    if (bytes > 0)
    {
        _stream.Write(_byteBuffer, 0, bytes);
    }
    ...
}
```

`Flush`最终也是使用`_encoder.GetBytes`获取字节数组写入流中，而`System.Text.Encoding.UTF8.GetBytes()`最终也是使用这个方法。

`System.Text.Encoding.UTF8.GetBytes`

```cs
public virtual byte[] GetBytes(string s)
{
    if (s == null)
    {
        throw new ArgumentNullException("s", SR.ArgumentNull_String);
    }
    int byteCount = GetByteCount(s);
    byte[] array = new byte[byteCount];
    int bytes = GetBytes(s, 0, s.Length, array, 0);
    return array; 
}
```


如果要达到和原来一样的效果，只需要在最终返回结果加上`UTF8.Preamble`， 修改如下

``` diff
public static byte[] StringToBytes(string value)
{
    if (value == null) throw new ArgumentNullException(nameof(value));

-   return System.Text.Encoding.UTF8.GetBytes(value);

+   var bytes = System.Text.Encoding.UTF8.GetBytes(value);

+   var result = new byte[bytes.Length + 3];
+   Array.Copy(Encoding.UTF8.GetPreamble(), result, 3);
+   Array.Copy(bytes, 0, result, 3, bytes.Length);

+   return result;
}
```

但是对于这样修改感觉是没必要，因为这个最终是传给一个对外接口，所以只能对那个接口做测试，最终结果也是不需要这个BOM

## 基准测试

排除了`StreamWriter`没有做特殊处理，可以用`System.Text.Encoding.UTF8.GetBytes()`重构。还有就是效率问题，虽然直观上看到使用`StreamWriter` 最终都是使用`Encoder.GetBytes` 方法，而且还多了两次资源对申请和释放。但是还是用基准测试才能直观看出其中差别。
基准测试使用BenchmarkDotNet，[BenchmarkDotNet](https://www.cnblogs.com/WilsonPan/p/12904664.html)这里之前有介绍过

1. 添加BenchmarkDotNet

```cs
dotnet add package BenchmarkDotNet
```

2. 


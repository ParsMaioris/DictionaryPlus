using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MultiMaps.Core;

namespace MultiMaps.Tests.Validation;

[TestClass]
[TestCategory("Validation")]
public class ArgumentValidationTests
{
    [TestMethod]
    public void Add_NullKey_ShouldThrowArgumentNullException()
    {
        var map = new MultiMap<string, int>();
        Assert.ThrowsException<ArgumentNullException>(() => map.Add(null!, 5));
    }

    [TestMethod]
    public void GetValues_NullKey_ShouldThrowArgumentNullException()
    {
        var map = new MultiMap<string, int>();
        Assert.ThrowsException<ArgumentNullException>(() => map.GetValues(null!));
    }

    [TestMethod]
    public void RemoveValue_NullKey_ShouldThrowArgumentNullException()
    {
        var map = new MultiMap<string, int>();
        Assert.ThrowsException<ArgumentNullException>(() => map.RemoveValue(null!, 10));
    }

    [TestMethod]
    public void RemoveKey_NullKey_ShouldThrowArgumentNullException()
    {
        var map = new MultiMap<string, int>();
        Assert.ThrowsException<ArgumentNullException>(() => map.RemoveKey(null!));
    }
}
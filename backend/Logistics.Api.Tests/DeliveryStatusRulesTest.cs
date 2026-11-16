
using Logistics.Api.Services;
using Xunit;

namespace Logistics.Api.Tests;

public class DeliveryStatusRulesTests
{
    [Theory]
    [InlineData("Assigned", "In transit", true)]
    [InlineData("In transit", "Delivered", true)]
    [InlineData("Assigned", "Delivered", false)]
    [InlineData("Delivered", "In transit", false)]
    [InlineData("In transit", "Assigned", false)]
    public void CanTransition_ReturnsExpectedResult(
        string currentStatus,
        string newStatus,
        bool expected)
    {
        bool actual = DeliveryStatusRules.CanTransition(
            currentStatus,
            newStatus);

        Assert.Equal(expected, actual);
    }
}

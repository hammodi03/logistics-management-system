
namespace Logistics.Api.Services;

public static class DeliveryStatusRules
{
    public static bool CanTransition(
        string currentStatus,
        string newStatus)
    {
        return
            (currentStatus == "Assigned" &&
             newStatus == "In transit") ||
            (currentStatus == "In transit" &&
             newStatus == "Delivered");
    }
}

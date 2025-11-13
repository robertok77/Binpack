namespace Binpack.Shared.Models;

public record PortDto(AnchorageSizeDto AnchorageSize, FleetDto[] Fleets)
{
    public static PortDto Default { get; } = new(new AnchorageSizeDto(12, 15),
    [
        new FleetDto(new SingleShipDimensionsDto(6, 5), "LNG Unit", 2),
        new FleetDto(new SingleShipDimensionsDto(3, 12), "Science & Engineering Ship", 5)
    ]);
}
public record AnchorageSizeDto(int Width, int Height);
public record FleetDto(SingleShipDimensionsDto SingleShipDimensions, string ShipDesignation, int ShipCount);
public record SingleShipDimensionsDto(int Width, int Height);
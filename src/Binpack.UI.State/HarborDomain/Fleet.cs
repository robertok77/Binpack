using Binpack.Shared.Models;
using static Binpack.UI.State.HarborDomain.Vessel;

namespace Binpack.UI.State.HarborDomain;

public class Fleet
{
    private readonly List<Vessel> vessels = [];
    public IReadOnlyCollection<Vessel> Vessels => vessels.AsReadOnly();
    public bool IsAllVesselsInAnchorage => vessels.All(x => x.State.InAnchorage);
    public IEnumerable<Vessel> VesselsInAnchorage => Vessels.Where(x => x.State.InAnchorage);
    
    private Fleet(IEnumerable<VesselBuilder> vessels) =>
        this.vessels.AddRange(vessels.Select(builder => builder.WithFleet(this).Build()));

    public static Fleet Create(IEnumerable<FleetDto> fleetsDto) => new(BuildVessels(fleetsDto));
    public static Fleet Default { get; } = new([]);

    public IEnumerable<(string ShipDesignation, IEnumerable<Vessel> Vessels)> VesselsByDesignation()
    {
        var lookup = vessels.Where(x => !x.State.InAnchorage).ToLookup(x => x.ShipDesignation);
        var keys = vessels.Select(x => x.ShipDesignation).Distinct().OrderBy(x => x);
        return keys.Select(x => (x, lookup[x]));
    }

    internal void Reset()
    {
        foreach (var v in Vessels) v.ResetState();
    }
    internal bool OverlapsAny(Vessel subject) =>
        (from other in VesselsInAnchorage
         where other != subject
         select
             (subject.State.X + subject.CurrentWidth) <= other.State.X ||
             (other.State.X + other.CurrentWidth) <= subject.State.X ||
             (subject.State.Y + subject.CurrentHeight) <= other.State.Y ||
             (other.State.Y + other.CurrentHeight) <= subject.State.Y)
        .Any(separated => !separated);

    private static IEnumerable<VesselBuilder> BuildVessels(IEnumerable<FleetDto> fleetDto)
    {
        List<VesselBuilder> vessels = [];
        var fleet = fleetDto.ToArray();
        FleetDto f;
        for (var i = 0; i < fleet.Length; i++)
        {
            f = fleet[i];
            for (var j = 0; j < f.ShipCount; j++)
            {
                vessels.Add(CreateBuilder($"Ship {i}-{j}", f.SingleShipDimensions.Width, f.SingleShipDimensions.Height, f.ShipDesignation));
            }
        }
        return vessels;
    }
}
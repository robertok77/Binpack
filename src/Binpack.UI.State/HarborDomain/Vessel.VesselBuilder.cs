namespace Binpack.UI.State.HarborDomain;

public partial class Vessel
{
    internal sealed class VesselBuilder
    {
        private readonly Vessel _vessel;
        internal VesselBuilder(string name, double width, double height, string shipDesignation)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 0) throw new ArgumentOutOfRangeException(nameof(height));
            if (string.IsNullOrWhiteSpace(shipDesignation)) throw new ArgumentNullException(nameof(shipDesignation));

            _vessel = new Vessel(name, width, height, shipDesignation);
        }
        internal VesselBuilder WithFleet(Fleet fleet)
        {
            _vessel._fleet = fleet;
            return this;
        }
        internal Vessel Build() => _vessel._fleet != Fleet.Default
            ? _vessel
            : throw new InvalidOperationException("Fleet must be set before building a Vessel.");
    }
}
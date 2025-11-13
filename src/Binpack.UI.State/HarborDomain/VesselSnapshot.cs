namespace Binpack.UI.State.HarborDomain;

internal class VesselSnapshot(Vessel v) 
{
    private readonly VesselState state = v.CloneState;
    public void Revert() => v.SetState(state);
}

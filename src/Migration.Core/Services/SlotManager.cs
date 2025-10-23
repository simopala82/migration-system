namespace Migration.Core.Services;

public interface ISlotManager
{
    Task<bool> AcquireSlotAsync(CancellationToken cancellationToken);
    void ReleaseSlot();
    int GetAvailableSlots();
    int MaxSlots { get; }
}

public class SlotManager : ISlotManager
{
    private readonly SemaphoreSlim _semaphore;
    public int MaxSlots { get; }

    private int _slotsInUse;
    private readonly Lock _lock = new();

    public SlotManager(int maxSlots = 5)
    {
        MaxSlots = maxSlots;
        _semaphore = new SemaphoreSlim(maxSlots, maxSlots);
    }

    public async Task<bool> AcquireSlotAsync(CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        lock (_lock)
        {
            _slotsInUse++;
        }

        return true;
    }

    public void ReleaseSlot()
    {
        _semaphore.Release();
        lock (_lock)
        {
            _slotsInUse--;
        }
    }

    public int GetAvailableSlots()
    {
        return MaxSlots - _slotsInUse;
    }
}

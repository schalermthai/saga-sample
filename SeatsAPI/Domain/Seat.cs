namespace SBWorkflow.Seats.Domain;

public class Seat
{
    public string SeatLabel { get; private set; }
    public SeatState State { get; private set; }

    public Seat(string seatLabel)
    {
        SeatLabel = seatLabel;
        State = SeatState.Initialized;
    }

    public void Create()
    {
        if (State != SeatState.Initialized)
        {
            throw new InvalidOperationException("Seat can only be created from the Initialized state.");
        }

        State = SeatState.Open;
    }

    public void Reserve()
    {
        if (State != SeatState.Open)
        {
            throw new InvalidOperationException("Seat can only be reserved from the Open state.");
        }

        State = SeatState.Reserved;
    }

    public void Release()
    {
        if (State != SeatState.Reserved)
        {
            throw new InvalidOperationException("Seat can only be released from the Reserved state.");
        }

        State = SeatState.Open;
    }

    public void Commit()
    {
        if (State != SeatState.Reserved)
        {
            throw new InvalidOperationException("Seat can only be committed from the Reserved state.");
        }

        State = SeatState.Committed;
    }

    public void ReOpen()
    {
        if (State != SeatState.Committed)
        {
            throw new InvalidOperationException("Seat can only be reopened from the Committed state.");
        }

        State = SeatState.ReOpen;
    }

    public void CheckIn()
    {
        if (State != SeatState.Committed)
        {
            throw new InvalidOperationException("Seat can only be checked in from the Committed state.");
        }

        State = SeatState.CheckedIn;
    }

    public void Timeout()
    {
        if (State != SeatState.Committed)
        {
            throw new InvalidOperationException("Seat can only timeout from the Committed state.");
        }

        State = SeatState.ReOpen;
    }
}

public enum SeatState
{
    Initialized,
    Open,
    Reserved,
    Committed,
    CheckedIn,
    ReOpen
}

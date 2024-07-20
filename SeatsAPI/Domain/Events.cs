namespace SeatsInventory.Domain;

public class SeatReserved
{
    public string SeatLabel { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SeatReleased
{
    public string SeatLabel { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SeatCommitted
{
    public string SeatLabel { get; set; }
    public DateTime Timestamp { get; set; }
}

public class SeatReOpened
{
    public string SeatLabel { get; set; }
    public DateTime Timestamp { get; set; }
}

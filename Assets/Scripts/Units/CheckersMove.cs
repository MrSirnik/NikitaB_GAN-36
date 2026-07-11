namespace Tactics
{
    public readonly struct CheckersMove
    {
        public readonly Cell Destination;
        public readonly Cell CapturedCell;

        public CheckersMove(Cell destination, Cell capturedCell)
        {
            Destination = destination;
            CapturedCell = capturedCell;
        }

        public bool IsCapture => CapturedCell != null;
    }
}

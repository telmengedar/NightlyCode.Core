namespace NightlyCode.Core.ComponentModel
{
    public abstract class BinaryNode
    {
        IBinaryBounds bounds;

        public BinaryNode(IBinaryBounds bounds)
        {
            this.bounds = bounds;
        }

        public IBinaryBounds Bounds
        {
            get { return bounds; }
        }
    }
}

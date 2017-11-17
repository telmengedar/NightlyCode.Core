namespace NightlyCode.Core.ComponentModel
{
    public class BinaryRoot : BinaryNode
    {
        BinaryNode firstnode;
        BinaryNode secondnode;


        public BinaryRoot(IBinaryBounds bounds, BinaryNode firstnode, BinaryNode secondnode)
            : base(bounds)
        {
            this.firstnode = firstnode;
            this.secondnode = secondnode;
        }

        public BinaryNode FirstNode
        {
            get { return firstnode; }
        }

        public BinaryNode SecondNode
        {
            get { return secondnode; }
        }
    }
}

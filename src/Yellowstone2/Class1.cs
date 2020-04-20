using System;

namespace TheXDS.Vulcanium.Yellowstone
{
    public class Class1 : GraphicsComponent, ITextLogic
    {
        public int CursorOffset { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Print(string text)
        {
            
        }

        public void Scroll(ushort lines)
        {
            throw new NotImplementedException();
        }
    }

}


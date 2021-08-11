using System.Drawing;

namespace System.App.Utility.Helpers
{
    public static class ColorHelper
    {
        public static Color GetColor(int index)
        {
            switch (index)
            {
                case 1: 
                    return Color.FromArgb(198, 99, 99);
                case 2:
                    return Color.Navy;
                case 3:
                    return Color.Olive;
                case 4:
                    return Color.Crimson;
                case 5:
                    return Color.Teal;
                default:
                    return Color.FromArgb(RandomNumberHelper.GetRandomNumber(0, 0xFFFFFF));
            }
            
        }        
    }
}
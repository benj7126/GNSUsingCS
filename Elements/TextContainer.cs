using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GNSUsingCS.Elements
{
    internal abstract class TextContainer : Element
    {
        [ConfigAttributes.Int]
        public int FontSize = 24;
        [ConfigAttributes.Vector2]
        public Vector2 Padding = new Vector2(6, 6);
        [ConfigAttributes.SingleLineString] // might want to be able to select between all possible font types instead
        public string FontType = ""; // "" = default
        [ConfigAttributes.Float]
        public float Spacing = 1f;
        [ConfigAttributes.Float]
        public float LineSpacing = 1f;
        [ConfigAttributes.Color]
        public Color Color = Color.Black;
        [ConfigAttributes.MultiLineString]
        public string Text = """
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec leo erat, porttitor ac tempus sed, ultricies quis massa. Proin finibus mollis dolor. Ut iaculis magna sit amet libero imperdiet, iaculis laoreet nibh sagittis. Morbi metus augue, porttitor nec faucibus vitae, sodales malesuada ex. Vivamus imperdiet pulvinar justo vel lobortis. Donec justo lacus, volutpat non nunc ac, euismod tincidunt turpis. Etiam cursus eros lorem, sit amet pulvinar neque efficitur non. In hac habitasse platea dictumst. Nunc fringilla dolor ac est luctus dictum.

            Aliquam vitae dapibus enim. Donec vel tortor ut tortor commodo congue. Sed at urna a metus dapibus rhoncus et a felis. Nam ut ex vel metus imperdiet vestibulum in nec libero. Praesent nec nibh id ante iaculis laoreet ac eu nunc. Mauris tincidunt tempus odio, eu imperdiet nibh efficitur ac. Fusce ac tellus nec elit mattis molestie. Duis nec est sapien. Mauris at blandit magna. Nullam faucibus, diam sit amet feugiat dignissim, justo ipsum posuere arcu, nec facilisis felis nisl et libero. Cras mi tellus, aliquam ut est non, sagittis mattis arcu. Proin et justo massa. Nunc aliquam ut turpis eget scelerisque.

            Duis feugiat vitae velit nec vulputate. Cras leo ante, porta eu augue nec, dictum luctus neque. Donec congue venenatis sapien, imperdiet venenatis augue auctor ac. Sed lacus turpis, volutpat sit amet finibus ut, sodales tempor mauris. Praesent elit quam, pretium quis semper eget, ornare eget leo. In tristique cursus nisi. Etiam tempor orci sit amet accumsan fringilla. Nulla ultrices magna quam, quis mattis urna feugiat sed. Aliquam ac massa ut sem gravida ultricies sed a quam.

            Phasellus porttitor convallis quam vitae congue. Proin quis feugiat justo. Nullam elementum lacus id orci euismod, in cursus mauris porttitor. Aliquam a consectetur diam, quis mollis lacus. Ut eu scelerisque ipsum. Pellentesque fringilla sem a accumsan malesuada. Nullam quis mi eleifend, condimentum nunc dictum, tincidunt libero. Nulla convallis bibendum condimentum. Ut egestas tincidunt urna, nec posuere metus elementum et. Aliquam feugiat scelerisque est interdum vestibulum. Quisque sed orci lacus. Pellentesque sodales sodales lacus, quis aliquam felis blandit id. Quisque sit amet posuere nisi, sit amet eleifend sapien.

            Ut bibendum fringilla odio, vel dignissim orci dignissim convallis. Integer nec vestibulum erat, at mattis purus. Vestibulum ut accumsan eros, quis auctor felis. Nunc porta feugiat nisl in finibus. Sed a lorem id diam imperdiet pretium. Ut pharetra rhoncus sodales. Curabitur iaculis sapien ac fringilla euismod.
            """;

        internal override void SaveValues(ref SaveObject so)
        {
            so.Write(Text);
        }

        internal override void LoadValues(ref LoadObject lo)
        {
            Text = lo.Read();
        }
    }
}

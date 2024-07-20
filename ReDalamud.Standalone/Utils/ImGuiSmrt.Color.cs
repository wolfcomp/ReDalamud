using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDalamud.Standalone.Utils;
public partial class ImGuiSmrt
{
    public class Color : IDisposable
    {
        public static List<(ImGuiCol idx, Standalone.Color color)> Stack = new();

        private int _count;

        public Color Push(ImGuiCol idx, Standalone.Color color)
        {
            Stack.Add((idx, color));
            ImGui.PushStyleColor(idx, (Vector4)color);
            _count++;
            return this;
        }

        public Color PushTextColor(Standalone.Color color) => Push(ImGuiCol.Text, color);

        public void Pop(int num = 1)
        {
            num = Math.Min(num, _count);
            _count -= num;
            ImGui.PopStyleColor(num);
            Stack.RemoveRange(Stack.Count - num, num);
        }

        public void Dispose() => Pop(_count);
    }
}
